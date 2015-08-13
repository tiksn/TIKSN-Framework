using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TIKSN.Finance.ForeignExchange
{
    public class EuropeanCentralBank : ICurrencyConverter
    {                                                                                    //TODO: switch to http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml see http://www.ecb.europa.eu/stats/exchange/eurofxref/html/index.en.html (For Developers section)
        private const string DailyRatesUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        private const string Last90DaysRatesUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml";
        private const string Since1999RatesUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml";

        private readonly static CurrencyInfo Euro;

        static EuropeanCentralBank()
        {
            Euro = new CurrencyInfo(new RegionInfo("de-DE"));
        }

        public EuropeanCentralBank()
        {
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
        {
            var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);
            decimal rate = await this.GetExchangeRateAsync(pair, asOn);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            this.VerifyDate(asOn);

            var rates = await GetRatesAsync(asOn);

            var result = new List<CurrencyPair>();

            foreach (var rate in rates)
            {
                result.Add(new CurrencyPair(Euro, rate.Key));
                result.Add(new CurrencyPair(rate.Key, Euro));
            }

            return result;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
        {
            this.VerifyDate(asOn);

            var rates = await GetRatesAsync(asOn);

            if (pair.BaseCurrency == Euro)
            {
                return rates[pair.CounterCurrency];
            }
            else if (pair.CounterCurrency == Euro)
            {
                return decimal.One / rates[pair.BaseCurrency];
            }
            else
            {
                throw new ArgumentException("One of currency pair should be Euro.");
            }
        }

        private static async Task<Dictionary<CurrencyInfo, decimal>> GetRatesAsync(DateTimeOffset asOn)
        {
            string requestURL = GetRatesUrl(asOn);

            HttpWebRequest request = WebRequest.CreateHttp(requestURL);

            HttpWebResponse response = (HttpWebResponse)await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);

            var xdoc = XDocument.Load(response.GetResponseStream());

            var groupsCubes = xdoc.Element("{http://www.gesmes.org/xml/2002-08-01}Envelope").Element("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube").Elements("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube");

            var groupsCubesCollection = groupsCubes.Select(item => new Tuple<XElement, DateTimeOffset>(item, DateTimeOffset.Parse(item.Attribute("time").Value)));

            var closestCubeDateDifference = groupsCubesCollection.Where(item => item.Item2 <= asOn).Min(item => asOn - item.Item2);

            var groupCubes = groupsCubesCollection.First(item => asOn - item.Item2 == closestCubeDateDifference);

            var rates = new Dictionary<CurrencyInfo, decimal>();

            foreach (var rateCube in groupCubes.Item1.Elements("{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube"))
            {
                var currencyCode = rateCube.Attribute("currency").Value;
                var rate = decimal.Parse(rateCube.Attribute("rate").Value);

                rates.Add(new CurrencyInfo(currencyCode), rate);
            }

            return rates;
        }

        private void VerifyDate(DateTimeOffset asOn)
        {
            if (asOn > DateTime.Now)
                throw new ArgumentException("Exchange rate forecasting are not supported.");
        }

        private static string GetRatesUrl(DateTimeOffset asOn)
        {
            if (asOn.Date == DateTimeOffset.Now.Date)
                return DailyRatesUrl;
            else if (asOn.Date >= DateTimeOffset.Now.AddDays(-90))
                return Last90DaysRatesUrl;
            else
                return Since1999RatesUrl;
        }
    }
}