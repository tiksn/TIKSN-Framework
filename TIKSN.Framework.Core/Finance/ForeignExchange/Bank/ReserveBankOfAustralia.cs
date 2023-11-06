using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ReserveBankOfAustralia : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string RSS = "https://www.rba.gov.au/rss/rss-cb-exchange-rates.xml";

        private static readonly CurrencyInfo AustralianDollar;
        private readonly ICurrencyFactory currencyFactory;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly Dictionary<CurrencyInfo, decimal> rates;
        private readonly ITimeProvider timeProvider;
        private DateTimeOffset lastFetchDate;
        private DateTimeOffset publishedDate;

        static ReserveBankOfAustralia()
        {
            var australia = new RegionInfo("en-AU");
            AustralianDollar = new CurrencyInfo(australia);
        }

        public ReserveBankOfAustralia(
            IHttpClientFactory httpClientFactory,
            ICurrencyFactory currencyFactory,
            ITimeProvider timeProvider)
        {
            this.publishedDate = DateTimeOffset.MinValue;

            this.rates = new Dictionary<CurrencyInfo, decimal>();

            this.lastFetchDate = DateTimeOffset.MinValue;
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
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
            await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

            this.VerifyDate(asOn);

            var pairs = new List<CurrencyPair>();

            pairs.AddRange(this.rates.Keys.Select(r => new CurrencyPair(AustralianDollar, r)));
            pairs.AddRange(this.rates.Keys.Select(r => new CurrencyPair(r, AustralianDollar)));

            return pairs;
        }

        public async Task<decimal> GetExchangeRateAsync(
            CurrencyPair pair,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            await this.FetchOnDemandAsync(cancellationToken).ConfigureAwait(false);

            this.VerifyDate(asOn);

            if (pair.BaseCurrency == AustralianDollar)
            {
                if (this.rates.TryGetValue(pair.CounterCurrency, out var rate))
                {
                    return rate;
                }
            }
            else if (pair.CounterCurrency == AustralianDollar)
            {
                if (this.rates.TryGetValue(pair.BaseCurrency, out var counterRate))
                {
                    return decimal.One / counterRate;
                }
            }

            throw new ArgumentException("Currency pair not supported.");
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var result = new List<ExchangeRate>();

            var httpClient = this.httpClientFactory.CreateClient();
            var responseStream = await httpClient.GetStreamAsync(RSS, cancellationToken).ConfigureAwait(false);

            var xdoc = XDocument.Load(responseStream);

            lock (this.rates)
            {
                foreach (var item in xdoc.Element("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}RDF")
                    .Elements("{http://purl.org/rss/1.0/}item"))
                {
                    var exchangeRateElement =
                        item.Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}statistics")
                            .Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}exchangeRate");
                    var baseCurrencyElement =
                        exchangeRateElement.Element(
                            "{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}baseCurrency");
                    var targetCurrencyElement =
                        exchangeRateElement.Element(
                            "{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}targetCurrency");
                    var observationValueElement = exchangeRateElement
                        .Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observation")
                        .Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}value");
                    var periodElement = exchangeRateElement
                        .Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}observationPeriod")
                        .Element("{http://www.cbwiki.net/wiki/index.php/Specification_1.2/}period");

                    Debug.Assert(string.Equals(baseCurrencyElement.Value, "AUD", StringComparison.Ordinal));

                    var counterCurrencyCode = targetCurrencyElement.Value;

                    var exchangeRate = decimal.Parse(observationValueElement.Value, CultureInfo.InvariantCulture);
                    var period = DateTimeOffset.Parse(periodElement.Value, CultureInfo.InvariantCulture);

                    var foreignCurrency = this.currencyFactory.Create(counterCurrencyCode);

                    this.rates[foreignCurrency] = exchangeRate;

                    this.publishedDate = period;

                    result.Add(new ExchangeRate(new CurrencyPair(AustralianDollar, foreignCurrency), period,
                        exchangeRate));
                }

                this.lastFetchDate = this.timeProvider.GetCurrentTime();
            }

            return result;
        }

        private async Task FetchOnDemandAsync(CancellationToken cancellationToken)
        {
            if (this.timeProvider.GetCurrentTime() - this.lastFetchDate > TimeSpan.FromDays(1d))
            {
                _ = await this.GetExchangeRatesAsync(this.timeProvider.GetCurrentTime(), cancellationToken).ConfigureAwait(false);
            }
        }

        private void VerifyDate(DateTimeOffset asOn)
        {
            if (asOn > this.timeProvider.GetCurrentTime())
            {
                throw new ArgumentException("Exchange rate forecasting are not supported.");
            }

            if (asOn < this.publishedDate)
            {
                throw new ArgumentException("Exchange rate history not supported.");
            }
        }
    }
}
