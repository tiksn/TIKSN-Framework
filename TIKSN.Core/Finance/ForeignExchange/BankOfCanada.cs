using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange
{
	public class BankOfCanada : ICurrencyConverter
	{
		private const string RestURL = "http://www.bankofcanada.ca/valet/observations/group/FX_RATES_DAILY/json";
		private static CurrencyInfo CanadianDollar;

		private DateTimeOffset lastFetchDate;
		private Dictionary<DateTimeOffset, Dictionary<CurrencyInfo, decimal>> rates;

		static BankOfCanada()
		{
			var Canada = new RegionInfo("en-CA");
			CanadianDollar = new CurrencyInfo(Canada);
		}

		public BankOfCanada()
		{
			rates = new Dictionary<DateTimeOffset, Dictionary<CurrencyInfo, decimal>>();

			lastFetchDate = DateTimeOffset.MinValue;
		}

		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var pair = new CurrencyPair(baseMoney.Currency, counterCurrency);

			var rate = await this.GetExchangeRateAsync(pair, asOn);

			return new Money(counterCurrency, baseMoney.Amount * rate);
		}

		public async Task FetchAsync()
		{
			var ratesList = await FetchRatesAsync(RestURL);

			lock (rates)
			{
				rates.Clear();

				foreach (var perDate in ratesList.GroupBy(item => item.Item2))
				{
					rates.Add(perDate.Key, perDate.ToDictionary(k => k.Item1, v => v.Item3));
				}
			}

			lastFetchDate = DateTime.Now; // must stay at the end
		}

		public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			await FetchOnDemandAsync();

			if (asOn > DateTime.Now)
				throw new ArgumentException("Exchange rate forecasting are not supported.");


			var result = new List<CurrencyPair>();

			foreach (CurrencyInfo against in GetRatesByDate(asOn).Keys)
			{
				result.Add(new CurrencyPair(CanadianDollar, against));
				result.Add(new CurrencyPair(against, CanadianDollar));
			}

			return result;
		}

		public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			await FetchOnDemandAsync();

			if (asOn > DateTimeOffset.Now)
				throw new ArgumentException("Exchange rate forecasting not supported.");

			if (IsHomeCurrencyPair(pair, asOn))
			{
				return GetRatesByDate(asOn)[pair.CounterCurrency];
			}
			else
			{
				return decimal.One / GetRatesByDate(asOn)[pair.BaseCurrency];
			}
		}

		private async Task FetchOnDemandAsync()
		{
			if (DateTimeOffset.Now - lastFetchDate > TimeSpan.FromDays(1d))
			{
				await FetchAsync();
			}
		}

		private async Task<List<Tuple<CurrencyInfo, DateTimeOffset, decimal>>> FetchRatesAsync(string restUrl)
		{
			var result = new List<Tuple<CurrencyInfo, DateTimeOffset, decimal>>();

			var rawData = await FetchRawDataAsync(restUrl);

			foreach (var rawItem in rawData)
			{
				var currency = new CurrencyInfo(rawItem.Item1);

				result.Add(new Tuple<CurrencyInfo, DateTimeOffset, decimal>(currency, rawItem.Item2, rawItem.Item3));
			}

			return result;
		}

		private async Task<List<Tuple<string, DateTimeOffset, decimal>>> FetchRawDataAsync(string restUrl)
		{
			using (var httpClient = new HttpClient())
			{
				var responseStream = await httpClient.GetStreamAsync(restUrl);

				using (var streamReader = new StreamReader(responseStream))
				{
					var jsonDoc = (JObject)JsonConvert.DeserializeObject(await streamReader.ReadToEndAsync());

					var result = new List<Tuple<string, DateTimeOffset, decimal>>();

					foreach (var observation in jsonDoc.Children().Single(item => string.Equals(item.Path, "observations", StringComparison.OrdinalIgnoreCase)).Children().Single().Children())
					{
						var asOn = new DateTimeOffset(observation.Value<DateTime>("d"));

						foreach (JProperty observationProperty in observation.Children())
						{
							if (observationProperty.Name.StartsWith("FX", StringComparison.OrdinalIgnoreCase))
							{
								Debug.Assert(observationProperty.Name.EndsWith("CAD"));

								var targetCurrencyCode = observationProperty.Name.Substring(2, 3);

								var valueObject = observationProperty.Value.Children().OfType<JProperty>().FirstOrDefault(item => string.Equals(item.Name, "v", StringComparison.OrdinalIgnoreCase));

								if (valueObject != null)
								{
									var rate = (decimal)valueObject.Value;

									result.Add(new Tuple<string, DateTimeOffset, decimal>(targetCurrencyCode, asOn, rate));
								}
							}
						}
					}

					return result;
				}
			}
		}

		private Dictionary<CurrencyInfo, decimal> GetRatesByDate(DateTimeOffset asOn)
		{
			var date = asOn.Date;

			if (date.DayOfWeek == DayOfWeek.Saturday)
				date = date.AddDays(-1);
			else if (date.DayOfWeek == DayOfWeek.Sunday)
				date = date.AddDays(-2);

			return rates[date];
		}

		private bool IsHomeCurrencyPair(CurrencyPair Pair, DateTimeOffset asOn)
		{
			if (Pair.BaseCurrency == CanadianDollar)
			{
				if (GetRatesByDate(asOn).Any(R => R.Key == Pair.CounterCurrency))
					return true;
			}
			else if (Pair.CounterCurrency == CanadianDollar)
			{
				if (GetRatesByDate(asOn).Any(R => R.Key == Pair.BaseCurrency))
					return false;
			}

			throw new ArgumentException("Currency pair not supported.");
		}
	}
}