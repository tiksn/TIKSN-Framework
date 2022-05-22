using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TIKSN.Globalization;
using TIKSN.Time;

namespace TIKSN.Finance.ForeignExchange.Cumulative
{
    public class MyCurrencyDotNet : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string ResourceUrl = "https://www.mycurrency.net/US.json";
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public MyCurrencyDotNet(ICurrencyFactory currencyFactory, ITimeProvider timeProvider)
        {
            this._currencyFactory = currencyFactory;
            this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var rate = await this.GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken).ConfigureAwait(false);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            if (this._timeProvider.GetCurrentTime().Date != asOn.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(asOn));
            }

            var exchangeRates = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

            return exchangeRates.Select(item => item.Pair).ToArray();
        }

        public Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn,
            CancellationToken cancellationToken) =>
            this.GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn, cancellationToken);

        public async Task<IEnumerable<ForeignExchange.ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            ValidateDate(asOn, this._timeProvider);

            using var httpClient = new HttpClient();
            var jsonExchangeRates = await httpClient.GetStringAsync(ResourceUrl).ConfigureAwait(false);

            var exchangeResponse = JsonConvert.DeserializeObject<ExchangeResponse>(jsonExchangeRates);
            var exchangeRates = exchangeResponse.Rates;

            var baseCurrency = this._currencyFactory.Create(exchangeResponse.BaseCurrency);

            var rates = exchangeRates
                .Select(item => (currency: this._currencyFactory.Create(item.CurrencyCode), rate: item.Rate))
                .Where(item => item.currency != baseCurrency)
                .ToArray();

            return rates
                .Select(item =>
                    new ForeignExchange.ExchangeRate(new CurrencyPair(baseCurrency, item.currency), asOn,
                        item.rate))
                .Concat(rates
                    .Select(item => new ForeignExchange.ExchangeRate(new CurrencyPair(item.currency, baseCurrency),
                        asOn, decimal.One / item.rate)))
                .ToArray();
        }

        private static void ValidateDate(DateTimeOffset asOn, ITimeProvider timeProvider)
        {
            if (timeProvider.GetCurrentTime().Date != asOn.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(asOn));
            }
        }

        private async Task<decimal> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency,
            DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            ValidateDate(asOn, this._timeProvider);

            var exchangeRates = await this.GetExchangeRatesAsync(asOn, cancellationToken).ConfigureAwait(false);

            var exchangeRate = exchangeRates.SingleOrDefault(item =>
                item.Pair.BaseCurrency == baseCurrency && item.Pair.CounterCurrency == counterCurrency);

            if (exchangeRate == null)
            {
                throw new NotSupportedException($"Currency pair {baseCurrency}/{counterCurrency} is not supported");
            }

            return exchangeRate.Rate;
        }

        public class ExchangeRate
        {
            [JsonProperty("code")] public string Code { get; set; }

            [JsonProperty("currency_code")] public string CurrencyCode { get; set; }

            [JsonProperty("name")] public string Name { get; set; }

            [JsonProperty("rate")] public decimal Rate { get; set; }
        }

        public class ExchangeResponse
        {
            [JsonProperty("baseCurrency")] public string BaseCurrency { get; set; }

            [JsonProperty("rates")] public ExchangeRate[] Rates { get; set; }
        }
    }
}
