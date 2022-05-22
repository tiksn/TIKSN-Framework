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
    public class CurrencyConverterApiDotCom : ICurrencyConverter, IExchangeRateProvider
    {
        private const string ConverterEndpointFormat = "api/v7/convert?q={0}_{1}&compact=ultra&apiKey={2}";
        private const string CurrencyListApiEndpointFormat = "api/v7/currencies?apiKey={0}";
        private readonly ICurrencyFactory _currencyFactory;
        private readonly Plan _plan;
        private readonly ITimeProvider _timeProvider;

        public CurrencyConverterApiDotCom(
            ICurrencyFactory currencyFactory,
            ITimeProvider timeProvider,
            Plan plan)
        {
            this._currencyFactory = currencyFactory ?? throw new ArgumentNullException(nameof(currencyFactory));
            this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            this._plan = plan ?? throw new ArgumentNullException(nameof(plan));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var rate = (await this.GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken).ConfigureAwait(false))
                .Rate;

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = this._plan.BaseAddress;

            var requestUri = string.Format(
                CultureInfo.InvariantCulture,
                CurrencyListApiEndpointFormat,
                this._plan.ApiKey);

            var currenciesJson =
                await httpClient.GetStringAsync(requestUri).ConfigureAwait(false);

            var currencyList = JsonConvert.DeserializeObject<CurrencyList>(currenciesJson);

            var currencies = currencyList.Results.Keys
                .Where(item => !string.Equals(item, "BTC", StringComparison.OrdinalIgnoreCase))
                .Select(item => this._currencyFactory.Create(item))
                .Distinct()
                .ToArray();

            var pairs = new List<CurrencyPair>();

            for (var i = 0; i < currencies.Length; i++)
            {
                for (var j = 0; j < currencies.Length; j++)
                {
                    if (i != j)
                    {
                        pairs.Add(new CurrencyPair(currencies[i], currencies[j]));
                    }
                }
            }

            return pairs;
        }

        public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken) =>
            (await this.GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn, cancellationToken).ConfigureAwait(false)).Rate;

        public async Task<ExchangeRate> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            if (this._timeProvider.GetCurrentTime().Date != asOn.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(asOn));
            }

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = this._plan.BaseAddress;

            var exchangeRateJson = await httpClient.GetStringAsync(
                string.Format(ConverterEndpointFormat,
                    baseCurrency.ISOCurrencySymbol,
                    counterCurrency.ISOCurrencySymbol, this._plan.ApiKey)).ConfigureAwait(false);

            var exchangeRates = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(exchangeRateJson);

            return new ExchangeRate(new CurrencyPair(baseCurrency, counterCurrency), asOn.Date,
                exchangeRates.Values.Single());
        }

        public abstract class Plan
        {
            protected Plan(string apiKey, string hostName)
            {
                this.ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
                this.BaseAddress = new UriBuilder(Uri.UriSchemeHttps, hostName).Uri;
            }

            public string ApiKey { get; }
            public Uri BaseAddress { get; }
        }

        public sealed class PremiumPlan : Plan
        {
            public PremiumPlan(string apiKey) : base(apiKey, "api.currconv.com")
            {
            }
        }

        public sealed class PrepaidPlan : Plan
        {
            public PrepaidPlan(string apiKey) : base(apiKey, "prepaid.currconv.com")
            {
            }
        }

        public sealed class FreePlan : Plan
        {
            public FreePlan(string apiKey) : base(apiKey, "free.currconv.com")
            {
            }
        }

        public sealed class DedicatedPlan : Plan
        {
            public DedicatedPlan(string apiKey, string subdomain) : base(apiKey, $"{subdomain}.currconv.com")
            {
            }
        }

        public class CurrencyList
        {
            [JsonProperty("results")] public Dictionary<string, CurrencyRecord> Results { get; set; }

            public class CurrencyRecord
            {
                [JsonProperty("currencyName")] public string CurrencyName { get; set; }

                [JsonProperty("id")] public string Id { get; set; }
            }
        }
    }
}
