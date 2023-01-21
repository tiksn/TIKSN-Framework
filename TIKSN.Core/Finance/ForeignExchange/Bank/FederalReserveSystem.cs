using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIKSN.Globalization;
using TIKSN.Time;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class FederalReserveSystem : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string DataUrlFormat =
            "https://www.federalreserve.gov/datadownload/Output.aspx?rel=H10&series=f72f395f2a6b3a4bbc83b2983ad62737&lastObs=7&from={0}&to={1}&filetype=sdmx&label=include&layout=seriescolumn";

        private static readonly CurrencyInfo UnitedStatesDollar = new(new RegionInfo("en-US"));
        private readonly ICurrencyFactory currencyFactory;
        private readonly ITimeProvider timeProvider;

        public FederalReserveSystem(ICurrencyFactory currencyFactory, ITimeProvider timeProvider)
        {
            this.currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(
            Money baseMoney,
            CurrencyInfo counterCurrency,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            var rate = await this.GetExchangeRateAsync(pair, asOn, cancellationToken).ConfigureAwait(false);

            return new Money(counterCurrency, rate * baseMoney.Amount);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            ValidateDate(asOn, this.timeProvider);

            var result = new List<CurrencyPair>();

            var rates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

            foreach (var someCurrency in rates.Keys)
            {
                result.Add(new CurrencyPair(UnitedStatesDollar, someCurrency));
                result.Add(new CurrencyPair(someCurrency, UnitedStatesDollar));
            }

            return result;
        }

        public async Task<decimal> GetExchangeRateAsync(
            CurrencyPair pair,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            ValidateDate(asOn, this.timeProvider);

            var rates = await this.GetRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

            if (pair.BaseCurrency == UnitedStatesDollar)
            {
                if (rates.TryGetValue(pair.CounterCurrency, out var rate))
                {
                    return rate;
                }
            }
            else if (pair.CounterCurrency == UnitedStatesDollar)
            {
                if (rates.TryGetValue(pair.BaseCurrency, out var counterRate))
                {
                    return decimal.One / counterRate;
                }
            }

            throw new ArgumentException($"Currency pair '{pair}' not supported.");
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var DataUrl = string.Format(DataUrlFormat,
                this.timeProvider.GetCurrentTime().AddDays(-10d).ToString("MM/dd/yyyy"),
                this.timeProvider.GetCurrentTime().ToString("MM/dd/yyyy"));

            using var httpClient = new HttpClient();
            var responseStream = await httpClient.GetStreamAsync(DataUrl).ConfigureAwait(false);

            var xdoc = XDocument.Load(responseStream);

            var result = new List<ExchangeRate>();

            foreach (var seriesElement in xdoc
                .Element("{http://www.SDMX.org/resources/SDMXML/schemas/v1_0/message}MessageGroup")
                .Element("{http://www.federalreserve.gov/structure/compact/common}DataSet")
                .Elements("{http://www.federalreserve.gov/structure/compact/H10_H10}Series"))
            {
                var currencyCode = seriesElement.Attribute("CURRENCY").Value;
                var fx = seriesElement.Attribute("FX").Value;

                if (currencyCode != "NA")
                {
                    var rates = new Dictionary<DateTime, decimal>();

                    foreach (var ObsElement in seriesElement.Elements(
                        "{http://www.federalreserve.gov/structure/compact/common}Obs"))
                    {
                        var ObsValue = decimal.Parse(ObsElement.Attribute("OBS_VALUE").Value);
                        var Period = DateTime.Parse(ObsElement.Attribute("TIME_PERIOD").Value);

                        decimal obsValueRate;

                        if (string.Equals(seriesElement.Attribute("UNIT").Value, "Currency:_Per_USD",
                            StringComparison.OrdinalIgnoreCase))
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

                    if (fx == "ZAL")
                    {
                        result.Add(new ExchangeRate(
                            new CurrencyPair(UnitedStatesDollar, this.currencyFactory.Create(currencyCode)), date,
                            rate));
                    }
                    else if (fx == "VEB")
                    {
                        result.Add(new ExchangeRate(
                            new CurrencyPair(UnitedStatesDollar, this.currencyFactory.Create("VEF")), date, rate));
                    }
                    else
                    {
                        result.Add(new ExchangeRate(
                            new CurrencyPair(UnitedStatesDollar, this.currencyFactory.Create(fx)), date, rate));
                    }
                }
            }

            return result;
        }

        private async Task<Dictionary<CurrencyInfo, decimal>> GetRatesAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var rates = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);
            var result = new Dictionary<CurrencyInfo, decimal>();

            foreach (var rawRate in rates)
            {
                result.Add(rawRate.Pair.CounterCurrency, rawRate.Rate);
            }

            return result;
        }

        private static void ValidateDate(DateTimeOffset asOn, ITimeProvider timeProvider)
        {
            if (asOn > timeProvider.GetCurrentTime())
            {
                throw new ArgumentException("Exchange rate forecasting are not supported.");
            }
        }
    }
}
