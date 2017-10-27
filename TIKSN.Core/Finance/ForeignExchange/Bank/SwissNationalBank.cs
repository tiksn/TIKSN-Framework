using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class SwissNationalBank : ICurrencyConverter, IExchangeRatesProvider
    {
        private static readonly CurrencyInfo SwissFranc;
        private static string RSSURL = "http://www.snb.ch/selector/en/mmr/exfeed/rss";
        private Dictionary<CurrencyInfo, Tuple<DateTimeOffset, decimal>> foreignRates;
        private readonly ICurrencyFactory _currencyFactory;

        static SwissNationalBank()
        {
            var Switzerland = new RegionInfo("de-CH");

            SwissFranc = new CurrencyInfo(Switzerland);
        }

        public SwissNationalBank(ICurrencyFactory currencyFactory)
        {
            this.foreignRates = new Dictionary<CurrencyInfo, Tuple<DateTimeOffset, decimal>>();
            _currencyFactory = currencyFactory;
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync();

            decimal rate = this.GetRate(baseMoney.Currency, counterCurrency, asOn);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync();

            var pairs = new List<CurrencyPair>();

            var todayCurrencies = this.FilterByDate(asOn);

            foreach (var currency in todayCurrencies)
            {
                pairs.Add(new CurrencyPair(SwissFranc, currency.Key));
                pairs.Add(new CurrencyPair(currency.Key, SwissFranc));
            }

            return pairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync();

            decimal rate = this.GetRate(pair.BaseCurrency, pair.CounterCurrency, asOn);

            return rate;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn)
        {
            var result = new List<ExchangeRate>();

            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(RSSURL);

                var xdoc = XDocument.Load(responseStream);

                lock (this.foreignRates)
                {
                    foreach (var ItemElement in xdoc.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF").Elements("{http://purl.org/rss/1.0/}item"))
                    {
                        var dateElement = ItemElement.Element("{http://purl.org/dc/elements/1.1/}date");
                        var exchangeRateElement = ItemElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}statistics").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}exchangeRate");
                        var valueElement = exchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observation").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}value");
                        var targetCurrencyElement = exchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}targetCurrency");

                        var date = DateTimeOffset.Parse(dateElement.Value);
                        var currencyCode = targetCurrencyElement.Value;
                        decimal rate = decimal.Parse(valueElement.Value);

                        var currency = _currencyFactory.Create(currencyCode);

                        Debug.Assert(exchangeRateElement.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observation").Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}unit").Value == "CHF");

                        this.foreignRates[currency] = new Tuple<DateTimeOffset, decimal>(date, rate);
                        result.Add(new ExchangeRate(new CurrencyPair(currency, SwissFranc), date, rate));
                    }
                }
            }

            return result;
        }

        private async Task FetchOnDemandAsync()
        {
            if (this.foreignRates.Count == 0)
                await this.GetExchangeRatesAsync(DateTimeOffset.Now);
            else if (this.foreignRates.Any(R => R.Value.Item1.Date == DateTimeOffset.Now.Date))
                await this.GetExchangeRatesAsync(DateTimeOffset.Now);
        }

        private System.Collections.Generic.Dictionary<CurrencyInfo, decimal> FilterByDate(DateTimeOffset asOn)
        {
            if (asOn > DateTimeOffset.Now)
                throw new System.ArgumentException("Exchange rate forecasting not supported.");

            var maxDate = this.foreignRates.Max(R => R.Value.Item1);
            var minDate = this.foreignRates.Min(R => R.Value.Item1);

            if (asOn < minDate)
                throw new ArgumentException("Exchange rate history not supported.");

            DateTimeOffset filterDate;

            if (asOn > maxDate)
            {
                filterDate = maxDate;
            }
            else
            {
                filterDate = asOn;
            }

            var filteredResults = this.foreignRates.Where(R => R.Value.Item1.Date == filterDate.Date).Select(R => new System.Tuple<CurrencyInfo, decimal>(R.Key, R.Value.Item2));

            var Results = new Dictionary<CurrencyInfo, decimal>();

            foreach (var FilteredResult in filteredResults)
            {
                Results.Add(FilteredResult.Item1, FilteredResult.Item2);
            }

            return Results;
        }

        private decimal GetRate(CurrencyInfo BaseCurrency, CurrencyInfo CounterCurrency, DateTimeOffset asOn)
        {
            var filtered = this.FilterByDate(asOn);

            if (BaseCurrency == SwissFranc)
            {
                if (filtered.ContainsKey(CounterCurrency))
                    return 1 / filtered[CounterCurrency];
            }
            else if (CounterCurrency == SwissFranc)
            {
                if (filtered.ContainsKey(BaseCurrency))
                    return filtered[BaseCurrency];
            }

            throw new ArgumentException("Currency pair not supported.");
        }
    }
}