using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class FederalReserveSystem : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string DataUrlFormat = "http://www.federalreserve.gov/datadownload/Output.aspx?rel=H10&series=f72f395f2a6b3a4bbc83b2983ad62737&lastObs=7&from={0}&to={1}&filetype=sdmx&label=include&layout=seriescolumn";

        private static readonly CurrencyInfo UnitedStatesDollar;
        private readonly ICurrencyFactory _currencyFactory;

        static FederalReserveSystem()
        {
            UnitedStatesDollar = new CurrencyInfo(new RegionInfo("en-US"));
        }

        public FederalReserveSystem(ICurrencyFactory currencyFactory)
        {
            _currencyFactory = currencyFactory;
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            var rate = await this.GetExchangeRateAsync(pair, asOn);

            return new Money(counterCurrency, rate * baseMoney.Amount);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            ValidateDate(asOn);

            var result = new List<CurrencyPair>();

            var rates = await GetRatesAsync(asOn);

            foreach (var SomeCurrency in rates.Keys)
            {
                result.Add(new CurrencyPair(UnitedStatesDollar, SomeCurrency));
                result.Add(new CurrencyPair(SomeCurrency, UnitedStatesDollar));
            }

            return result;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
        {
            ValidateDate(asOn);

            var rates = await GetRatesAsync(asOn);

            if (pair.BaseCurrency == UnitedStatesDollar)
            {
                if (rates.ContainsKey(pair.CounterCurrency))
                    return rates[pair.CounterCurrency];
            }
            else if (pair.CounterCurrency == UnitedStatesDollar)
            {
                if (rates.ContainsKey(pair.BaseCurrency))
                    return decimal.One / rates[pair.BaseCurrency];
            }

            throw new ArgumentException("Currency pair not supported.");
        }

        private async Task<Dictionary<CurrencyInfo, decimal>> GetRatesAsync(DateTimeOffset asOn)
        {
            var rates = await GetExchangeRatesAsync(asOn);
            var result = new Dictionary<CurrencyInfo, decimal>();

            foreach (var rawRate in rates)
            {
                result.Add(rawRate.Pair.CounterCurrency, rawRate.Rate);
            }

            return result;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn)
        {
            string DataUrl = string.Format(DataUrlFormat, DateTimeOffset.Now.AddDays(-10d).ToString("MM/dd/yyyy"), DateTimeOffset.Now.ToString("MM/dd/yyyy"));

            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(DataUrl);

                var xdoc = XDocument.Load(responseStream);

                var result = new List<ExchangeRate>();

                foreach (var SeriesElement in xdoc.Element("{http://www.SDMX.org/resources/SDMXML/schemas/v1_0/message}MessageGroup").Element("{http://www.federalreserve.gov/structure/compact/common}DataSet").Elements("{http://www.federalreserve.gov/structure/compact/H10_H10}Series"))
                {
                    var CurrencyCode = SeriesElement.Attribute("CURRENCY").Value;
                    var FX = SeriesElement.Attribute("FX").Value;

                    if (CurrencyCode != "NA")
                    {
                        Dictionary<DateTime, decimal> rates = new Dictionary<DateTime, decimal>();

                        foreach (var ObsElement in SeriesElement.Elements("{http://www.federalreserve.gov/structure/compact/common}Obs"))
                        {
                            var ObsValue = decimal.Parse(ObsElement.Attribute("OBS_VALUE").Value);
                            var Period = System.DateTime.Parse(ObsElement.Attribute("TIME_PERIOD").Value);

                            decimal obsValueRate;

                            if (string.Equals(SeriesElement.Attribute("UNIT").Value, "Currency:_Per_USD", System.StringComparison.OrdinalIgnoreCase))
                            {
                                obsValueRate = ObsValue;
                            }
                            else
                            {
                                obsValueRate = decimal.One / ObsValue;
                            }

                            rates.Add(Period, obsValueRate);
                        }

                        var date = rates.Keys.Max();
                        var rate = rates[date];

                        if (FX == "ZAL")
                        {
                            result.Add(new ExchangeRate(new CurrencyPair(UnitedStatesDollar, _currencyFactory.Create(CurrencyCode)), date, rate));
                        }
                        else if (FX == "VEB")
                        {
                            result.Add(new ExchangeRate(new CurrencyPair(UnitedStatesDollar, _currencyFactory.Create("VEF")), date, rate));
                        }
                        else
                        {
                            result.Add(new ExchangeRate(new CurrencyPair(UnitedStatesDollar, _currencyFactory.Create(FX)), date, rate));
                        }
                    }
                }

                return result;
            }
        }

        private static void ValidateDate(DateTimeOffset asOn)
        {
            if (asOn > DateTimeOffset.Now)
                throw new ArgumentException("Exchange rate forecasting are not supported.");
        }
    }
}