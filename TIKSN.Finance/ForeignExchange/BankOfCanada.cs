using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange
{
    public class BankOfCanada : ICurrencyConverter
    {
        private const string ClosingURL = "http://www.bankofcanada.ca/stats/assets/rates_rss/closing/en_all.xml";
        private const string NoonURL = "http://www.bankofcanada.ca/stats/assets/rates_rss/noon/en_all.xml";
        private static CurrencyInfo CanadianDollar;

        private DateTimeOffset lastFetchDate;
        private Dictionary<CurrencyInfo, Tuple<DateTimeOffset, decimal>> rates;

        static BankOfCanada()
        {
            System.Globalization.RegionInfo Canada = new System.Globalization.RegionInfo("en-CA");
            CanadianDollar = new CurrencyInfo(Canada);
        }

        public BankOfCanada()
        {
            this.rates = new Dictionary<CurrencyInfo, Tuple<DateTimeOffset, decimal>>();

            this.lastFetchDate = DateTimeOffset.Now;
        }

        public async Task<Money> ConvertCurrencyAsync(Money BaseMoney, CurrencyInfo CounterCurrency, DateTimeOffset asOn)
        {
            CurrencyPair pair = new CurrencyPair(BaseMoney.Currency, CounterCurrency);

            decimal rate = await this.GetExchangeRateAsync(pair, asOn);

            return new Money(CounterCurrency, BaseMoney.Amount * rate);
        }

        public async Task FetchAsync()
        {
            var noonRates = await this.FetchRatesAsync(NoonURL);
            var closingRates = await this.FetchRatesAsync(ClosingURL);

            this.UpdateRates(noonRates);
            this.UpdateRates(closingRates);

            this.lastFetchDate = System.DateTime.Now; // must stay at the end
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync();

            var MinDate = this.rates.Min(R => R.Value.Item1);

            if (asOn > DateTime.Now)
                throw new ArgumentException("Exchange rate forecasting are not supported.");

            if (asOn < MinDate)
                throw new ArgumentException("Exchange rate history not supported.");

            var result = new List<CurrencyPair>();

            foreach (CurrencyInfo against in this.rates.Keys)
            {
                result.Add(new CurrencyPair(CanadianDollar, against));
                result.Add(new CurrencyPair(against, CanadianDollar));
            }

            return result;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair Pair, DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync();

            if (asOn > DateTimeOffset.Now)
                throw new ArgumentException("Exchange rate forecasting not supported.");

            //if (asOn.Date < System.DateTime.Now.Date)
            //    throw new System.ArgumentException("Exchange rate history not supported.");

            Tuple<DateTimeOffset, decimal> rateInfo;

            if (this.IsHomeCurrencyPair(Pair))
            {
                rateInfo = this.rates[Pair.CounterCurrency];
            }
            else
            {
                rateInfo = this.rates[Pair.BaseCurrency];
                rateInfo = new Tuple<DateTimeOffset, decimal>(rateInfo.Item1, decimal.One / rateInfo.Item2);
            }

            if (asOn < rateInfo.Item1)
                throw new ArgumentException("Exchange rate history not supported.");

            return rateInfo.Item2;
        }

        private async Task FetchOnDemandAsync()
        {
            foreach (var rate in rates)
            {
                if (rate.Value.Item2 == decimal.Zero)
                {
                    await this.FetchAsync();

                    return;
                }
            }

            if (DateTimeOffset.Now - this.lastFetchDate > TimeSpan.FromDays(1d))
            {
                await this.FetchAsync();
            }
        }

        private async Task<Dictionary<CurrencyInfo, Tuple<DateTimeOffset, decimal>>> FetchRatesAsync(string RssUrl)
        {
            var result = new Dictionary<CurrencyInfo, Tuple<DateTimeOffset, decimal>>();

            var rawData = await FetchRawDataAsync(RssUrl);

            foreach (var rawItem in rawData)
            {
                if (rawItem.Key == "BSD" || rawItem.Key == "XPF" || rawItem.Key == "FJD" || rawItem.Key == "GHS" || rawItem.Key == "ANG")
                    continue;

                CurrencyInfo currency = this.rates.Keys.Single(C => C.ISOCurrencySymbol == rawItem.Key);

                result.Add(currency, rawItem.Value);
            }

            return result;
        }

        private async Task<Dictionary<string, Tuple<DateTimeOffset, decimal>>> FetchRawDataAsync(string RssUrl)
        {

            HttpWebRequest request = WebRequest.CreateHttp(RssUrl);

            HttpWebResponse response = (HttpWebResponse)await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);

            System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());

            var result = new Dictionary<string, Tuple<DateTimeOffset, decimal>>();

            foreach (var ItemElement in xdoc.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF").Elements("{http://purl.org/rss/1.0/}item"))
            {
                var ExchangeRateElement = ItemElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}statistics").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}exchangeRate");
                var ValueElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}value");
                var BaseCurrencyElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}baseCurrency");
                var TargetCurrencyElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}targetCurrency");
                var ObservationPeriodElement = ExchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.1}observationPeriod");

                Debug.Assert(BaseCurrencyElement.Value == "CAD");

                decimal rate = decimal.Parse(ValueElement.Value);
                var asOn = DateTimeOffset.Parse(ObservationPeriodElement.Value);

                result.Add(TargetCurrencyElement.Value, new Tuple<DateTimeOffset, decimal>(asOn, rate));
            }

            return result;
        }

        private bool IsHomeCurrencyPair(CurrencyPair Pair)
        {
            if (Pair.BaseCurrency == CanadianDollar)
            {
                if (this.rates.Any(R => R.Key == Pair.CounterCurrency))
                    return true;
            }
            else if (Pair.CounterCurrency == CanadianDollar)
            {
                if (this.rates.Any(R => R.Key == Pair.BaseCurrency))
                    return false;
            }

            throw new ArgumentException("Currency pair not supported.");
        }

        private void UpdateRates(Dictionary<CurrencyInfo, Tuple<DateTimeOffset, decimal>> newRates)
        {
            lock (this.rates)
            {
                foreach (var keyValue in newRates)
                {
                    if (keyValue.Value.Item1 > this.rates[keyValue.Key].Item1)
                    {
                        this.rates[keyValue.Key] = keyValue.Value;
                    }
                }
            }
        }
    }
}