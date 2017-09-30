using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange
{
	public class MyCurrencyDotNet : ICurrencyConverter
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

		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var rate = await GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn);

			return new Money(counterCurrency, baseMoney.Amount * rate);
		}

		public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			if (DateTimeOffset.Now.Date != asOn.Date)
				throw new ArgumentOutOfRangeException(nameof(asOn));

			var exchangeRates = await GetExchangeRatesAsync();

			return exchangeRates.Select(item => new CurrencyPair(USD, item.currency)).Concat(exchangeRates.Select(item => new CurrencyPair(item.currency, USD))).ToArray();
		}

		public Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			return GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn);
		}

		private async Task<decimal> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			if (DateTimeOffset.Now.Date != asOn.Date)
				throw new ArgumentOutOfRangeException(nameof(asOn));

			var exchangeRates = await GetExchangeRatesAsync();
			decimal exchangeRate;
			if (baseCurrency == USD)
				exchangeRate = exchangeRates.First(item => item.currency == counterCurrency).rate;
			else if (counterCurrency == USD)
				exchangeRate = decimal.One / exchangeRates.First(item => item.currency == baseCurrency).rate;
			else
				throw new NotSupportedException($"Currency pair {baseCurrency}/{counterCurrency} is not supported");

			return exchangeRate;
		}

		private async Task<(CurrencyInfo currency, decimal rate)[]> GetExchangeRatesAsync()
		{
			using (var httpClient = new HttpClient())
			{
				var jsonExchangeRates = await httpClient.GetStringAsync(ResourceUrl);

				var exchangeRates = JsonConvert.DeserializeObject<ExchangeRate[]>(jsonExchangeRates);

				return exchangeRates.Select(item => (currency: _currencyFactory.Create(item.CurrencyCode), rate: item.Rate)).ToArray();
			}
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
