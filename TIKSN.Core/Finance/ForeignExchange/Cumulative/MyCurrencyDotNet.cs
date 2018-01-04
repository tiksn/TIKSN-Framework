using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange.Cumulative
{
    public class MyCurrencyDotNet : ICurrencyConverter, IExchangeRatesProvider
    {
        private const string ResourceUrl = "http://www.mycurrency.net/service/rates";
        private readonly ICurrencyFactory _currencyFactory;
        private readonly CurrencyInfo USD;

        public MyCurrencyDotNet(ICurrencyFactory currencyFactory, IRegionFactory regionFactory)
        {
            var enUS = regionFactory.Create("en-US");
            USD = currencyFactory.Create(enUS);
            _currencyFactory = currencyFactory;
        }

        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var rate = await GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn, cancellationToken);

            return new Money(counterCurrency, baseMoney.Amount * rate);
        }

        public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            if (DateTimeOffset.Now.Date != asOn.Date)
                throw new ArgumentOutOfRangeException(nameof(asOn));

            var exchangeRates = await GetExchangeRatesAsync(asOn, cancellationToken);

            return exchangeRates.Select(item => item.Pair).ToArray();
        }

        public Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            return GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn, cancellationToken);
        }

        public async Task<IEnumerable<ForeignExchange.ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            ValidateDate(asOn);

            using (var httpClient = new HttpClient())
            {
                var jsonExchangeRates = await httpClient.GetStringAsync(ResourceUrl);

                var exchangeRates = JsonConvert.DeserializeObject<ExchangeRate[]>(jsonExchangeRates);

                var rates = exchangeRates
                    .Select(item => (currency: _currencyFactory.Create(item.CurrencyCode), rate: item.Rate))
                    .Where(item => item.currency != USD)
                    .ToArray();

                return rates
                    .Select(item => new ForeignExchange.ExchangeRate(new CurrencyPair(USD, item.currency), asOn, item.rate))
                    .Concat(rates
                        .Select(item => new ForeignExchange.ExchangeRate(new CurrencyPair(item.currency, USD), asOn, decimal.One / item.rate)))
                    .ToArray();
            }
        }

        private static void ValidateDate(DateTimeOffset asOn)
        {
            if (DateTimeOffset.Now.Date != asOn.Date)
                throw new ArgumentOutOfRangeException(nameof(asOn));
        }

        private async Task<decimal> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            ValidateDate(asOn);

            var exchangeRates = await GetExchangeRatesAsync(asOn, cancellationToken);

            var exchangeRate = exchangeRates.SingleOrDefault(item => item.Pair.BaseCurrency == baseCurrency && item.Pair.CounterCurrency == counterCurrency);

            if (exchangeRate == null)
                throw new NotSupportedException($"Currency pair {baseCurrency}/{counterCurrency} is not supported");

            return exchangeRate.Rate;
        }

        public class ExchangeRate
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("currency_code")]
            public string CurrencyCode { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("rate")]
            public decimal Rate { get; set; }
        }
    }
}