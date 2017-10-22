using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Bank
{
    public class CentralBankOfArmenia : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string RSS = "https://www.cba.am/_layouts/rssreader.aspx?rss=280F57B8-763C-4EE4-90E0-8136C13E47DA";
        private static readonly CurrencyInfo AMD = new CurrencyInfo(new RegionInfo("hy-AM"));
        private DateTimeOffset lastFetchDate;
        private Dictionary<CurrencyInfo, decimal> oneWayRates;
        private DateTimeOffset? publicationDate;
        private readonly ICurrencyFactory _currencyFactory;

        public CentralBankOfArmenia(ICurrencyFactory currencyFactory)
        {
            this.oneWayRates = new Dictionary<CurrencyInfo, decimal>();

            this.lastFetchDate = DateTimeOffset.MinValue;
            _currencyFactory = currencyFactory;
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
        {
            CurrencyPair pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

            decimal rate = await GetExchangeRateAsync(pair, asOn);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync(asOn);

            var rates = new List<CurrencyPair>();

            foreach (var rate in this.oneWayRates)
            {
                rates.Add(new CurrencyPair(rate.Key, AMD));
                rates.Add(new CurrencyPair(AMD, rate.Key));
            }

            return rates;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
        {
            await this.FetchOnDemandAsync(asOn);

            if (pair.CounterCurrency == AMD)
            {
                if (this.oneWayRates.ContainsKey(pair.BaseCurrency))
                    return this.oneWayRates[pair.BaseCurrency];
            }
            else
            {
                if (this.oneWayRates.ContainsKey(pair.CounterCurrency))
                    return decimal.One / this.oneWayRates[pair.CounterCurrency];
            }

            throw new ArgumentException("Currency pair was not found.");
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn)
        {
            ValodateDate(asOn);

            var result = new List<ExchangeRate>();

            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(RSS);

                var xdoc = XDocument.Load(responseStream);

                lock (this.oneWayRates)
                {
                    foreach (var item in xdoc.Element("rss").Element("channel").Elements("item"))
                    {
                        var title = item.Element("title");
                        var pubDate = item.Element("pubDate");

                        string[] titleParts = title.Value.Split('-');

                        string currencyCode = titleParts[0].Trim().ToUpper();
                        decimal baseUnit = decimal.Parse(titleParts[1]);
                        decimal counterUnit = decimal.Parse(titleParts[2]);

                        if (currencyCode == "BRC")
                            currencyCode = "BRL";

                        if (currencyCode == "LVL")
                            continue;

                        if (string.Equals(currencyCode, "SDR", StringComparison.OrdinalIgnoreCase))
                            currencyCode = "XDR";

                        var publicationDate = DateTimeOffset.Parse(pubDate.Value, new CultureInfo("en-US"));

                        if (baseUnit != decimal.Zero && counterUnit != decimal.Zero)
                        {
                            var rate = counterUnit / baseUnit;

                            var currency = _currencyFactory.Create(currencyCode);
                            oneWayRates[currency] = rate;
                            result.Add(new ExchangeRate(new CurrencyPair(AMD, currency), publicationDate, rate));
                            result.Add(new ExchangeRate(new CurrencyPair(currency, AMD), publicationDate, baseUnit / counterUnit));
                        }

                        if (!this.publicationDate.HasValue)
                        {
                            this.publicationDate = publicationDate;
                        }
                    }

                    this.lastFetchDate = DateTimeOffset.Now; // this should stay at the end
                }
            }

            return result;
        }

        private async Task FetchOnDemandAsync(DateTimeOffset asOn)
        {
            if (!this.publicationDate.HasValue)
            {
                await this.GetExchangeRatesAsync(asOn);
            }
            else if (DateTimeOffset.Now - lastFetchDate > TimeSpan.FromDays(1d))
            {
                await this.GetExchangeRatesAsync(asOn);
            }
        }

        private void ValodateDate(DateTimeOffset asOn)
        {
            if (asOn > DateTimeOffset.Now)
                throw new ArgumentException("Exchange rate forecasting are not supported.");

            if (asOn < this.publicationDate.Value)
                throw new ArgumentException("Exchange rate history are not supported.");
        }
    }
}