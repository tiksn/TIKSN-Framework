using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TIKSN.Globalization;

namespace TIKSN.Finance.ForeignExchange
{
	public class CurrencyConverterApiDotCom : ICurrencyConverter
	{
		private const string PaidVersionApiBaseAddress = "https://www.currencyconverterapi.com/";
		private const string FreeVersionApiBaseAddress = "http://free.currencyconverterapi.com/";
		private const string CurrencyListApiEndpointFormat = "api/v3/currencies?apiKey={0}";
		private const string ConverterEndpointFormat = "api/v3/convert?q={0}_{1}&compact=ultra&apiKey={2}";

		private readonly Uri _apiBaseAddress;
		private readonly ICurrencyFactory _currencyFactory;
		private readonly string _apiKey;

		public CurrencyConverterApiDotCom(ICurrencyFactory currencyFactory, bool useFreeVersion = true, string apiKey = "")
		{
			_currencyFactory = currencyFactory;

			if (useFreeVersion)
				_apiBaseAddress = new Uri(FreeVersionApiBaseAddress);
			else
				_apiBaseAddress = new Uri(PaidVersionApiBaseAddress);
			_apiKey = apiKey;
		}

		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var rate = await GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn);

			return new Money(counterCurrency, baseMoney.Amount * rate);
		}

		public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = _apiBaseAddress;

				var currenciesJson = await httpClient.GetStringAsync(string.Format(CurrencyListApiEndpointFormat, _apiKey));

				var currencyList = JsonConvert.DeserializeObject<CurrencyList>(currenciesJson);

				var currencies = currencyList.Results.Keys
					.Where(item => !string.Equals(item, "BTC", StringComparison.OrdinalIgnoreCase))
					.Select(item => _currencyFactory.Create(item))
					.Distinct()
					.ToArray();

				var pairs = new List<CurrencyPair>();

				for (int i = 0; i < currencies.Length; i++)
				{
					for (int j = 0; j < currencies.Length; j++)
					{
						if (i != j)
							pairs.Add(new CurrencyPair(currencies[i], currencies[j]));
					}
				}

				return pairs;
			}
		}

		public Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			return GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn);
		}

		private async Task<decimal> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			if (DateTimeOffset.Now.Date != asOn.Date)
				throw new ArgumentOutOfRangeException(nameof(asOn));

			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = _apiBaseAddress;

				var exchangeRateJson = await httpClient.GetStringAsync(string.Format(ConverterEndpointFormat, baseCurrency.ISOCurrencySymbol, counterCurrency.ISOCurrencySymbol, _apiKey));

				var exchangeRates = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(exchangeRateJson);

				return exchangeRates.Values.Single();
			}
		}

		public class CurrencyList
		{
			[JsonProperty("results")]
			public Dictionary<string, CurrencyRecord> Results { get; set; }

			public class CurrencyRecord
			{
				[JsonProperty("currencyName")]
				public string CurrencyName { get; set; }

				[JsonProperty("id")]
				public string Id { get; set; }
			}
		}
	}
}
