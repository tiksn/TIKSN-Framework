using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TIKSN.Globalization;
using TIKSN.Time;

namespace TIKSN.Finance.ForeignExchange.Cumulative
{
    public class CurrencylayerDotCom : ICurrencyConverter, IExchangeRateProvider, IExchangeRatesProvider
    {
        private const string HistoricalBaseURL = "https://apilayer.net/api/historical?";
        private const string LiveBaseURL = "https://apilayer.net/api/live?";
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly string accessKey;

        public CurrencylayerDotCom(ICurrencyFactory currencyFactory, ITimeProvider timeProvider, string accessKey)
        {
            if (string.IsNullOrEmpty(accessKey))
            {
                throw new ArgumentNullException(nameof(accessKey));
            }

            this.accessKey = accessKey;
            this._currencyFactory = currencyFactory;
            this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var rates = await this.GetRatesAasync(baseMoney.Currency, counterCurrency, asOn, cancellationToken);

            var rate = rates.Values.Single();

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var pairsWithRates = await this.GetRatesAasync(null, null, asOn, cancellationToken);

            var currencies = pairsWithRates.Keys.Select(item => item.CounterCurrency);

            var pairs = new List<CurrencyPair>();

            foreach (var currency1 in currencies)
            {
                foreach (var currency2 in currencies)
                {
                    if (currency1 != currency2)
                    {
                        pairs.Add(new CurrencyPair(currency1, currency2));
                    }
                }
            }

            return pairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var rates = await this.GetRatesAasync(pair.BaseCurrency, pair.CounterCurrency, asOn, cancellationToken);

            return rates.Values.Single();
        }

        public async Task<ExchangeRate> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var rates = await this.GetRatesAasync(baseCurrency, counterCurrency, asOn, cancellationToken);

            return new ExchangeRate(new CurrencyPair(baseCurrency, counterCurrency), asOn, rates.Single().Value);
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            var rates = await this.GetRatesAasync(null, null, asOn, cancellationToken);

            return rates.Select(item =>
                    new ExchangeRate(new CurrencyPair(item.Key.BaseCurrency, item.Key.CounterCurrency), asOn,
                        item.Value))
                .ToArray();
        }

        private async Task<IDictionary<CurrencyPair, decimal>> GetRatesAasync(CurrencyInfo baseCurrency,
            CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                string requestUrl;

                var difference = this._timeProvider.GetCurrentTime() - asOn;

                if (difference < TimeSpan.FromDays(1.0))
                {
                    requestUrl = LiveBaseURL;
                }
                else
                {
                    requestUrl = HistoricalBaseURL;
                    requestUrl += string.Format("date={0}&", asOn.ToString("yyyy-MM-dd"));
                }

                requestUrl += string.Format("access_key={0}", this.accessKey);

                if (baseCurrency != null)
                {
                    requestUrl += string.Format("&source={0}", baseCurrency.ISOCurrencySymbol);
                }

                if (counterCurrency != null)
                {
                    requestUrl += string.Format("&currencies={0}", counterCurrency.ISOCurrencySymbol);
                }

                var response = await client.GetAsync(requestUrl, cancellationToken);

                response.EnsureSuccessStatusCode();

                var responseJsonString = await response.Content.ReadAsStringAsync();

                var jsonSerializerSettings = new JsonSerializerSettings { Culture = CultureInfo.InvariantCulture };

                var responseJsonObject =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJsonString,
                        jsonSerializerSettings);

                var success = (bool)responseJsonObject["success"];

                if (success)
                {
                    var result = new Dictionary<CurrencyPair, decimal>();

                    var quotes =
                        JsonConvert.DeserializeObject<Dictionary<string, decimal>>(
                            responseJsonObject["quotes"].ToString(), jsonSerializerSettings);

                    foreach (var quote in quotes)
                    {
                        var quoteBaseCurrencyCode = quote.Key.Substring(0, 3);
                        var quoteCounterCurrencyCode = quote.Key.Substring(3);

                        if (this.IsSupportedCurrency(quoteBaseCurrencyCode) &&
                            this.IsSupportedCurrency(quoteCounterCurrencyCode) &&
                            quoteBaseCurrencyCode.ToUpperInvariant() != quoteCounterCurrencyCode.ToUpperInvariant())
                        {
                            var quoteBaseCurrency = this._currencyFactory.Create(quoteBaseCurrencyCode);
                            var quoteCounterCurrency = this._currencyFactory.Create(quoteCounterCurrencyCode);

                            var quotePair = new CurrencyPair(quoteBaseCurrency, quoteCounterCurrency);

                            result.Add(quotePair, quote.Value);
                        }
                    }

                    return result;
                }

                var errorDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJsonObject["error"].ToString());

                throw new Exception(errorDictionary["info"].ToString());
            }
        }

        private bool IsSupportedCurrency(string currencyCode)
        {
            switch (currencyCode.ToUpperInvariant())
            {
                case "BTC":
                case "GGP": // It is actually a Pound Sterling
                case "IMP": // It is actually a Pound Sterling
                case "JEP": // It is actually a Pound Sterling
                    return false;

                default:
                    return true;
            }
        }
    }
}
