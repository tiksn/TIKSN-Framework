using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange
{
	public class CurrencylayerDotCom : ICurrencyConverter
	{
		private const string LiveBaseURL = "http://apilayer.net/api/live?";
		private const string HistoricalBaseURL = "http://apilayer.net/api/historical?";

		private string accessKey;

		public CurrencylayerDotCom(string accessKey)
		{
			if (string.IsNullOrEmpty(accessKey))
				throw new ArgumentNullException(nameof(accessKey));

			this.accessKey = accessKey;
		}

		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var rates = await GetRatesAasync(baseMoney.Currency, counterCurrency, asOn);

			var rate = rates.Values.Single();

			return new Money(counterCurrency, baseMoney.Amount * rate);
		}

		public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			var pairsWithRates = await GetRatesAasync(null, null, asOn);

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

		public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			var rates = await GetRatesAasync(pair.BaseCurrency, pair.CounterCurrency, asOn);

			return rates.Values.Single();
		}

		private async Task<IDictionary<CurrencyPair, decimal>> GetRatesAasync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var client = new HttpClient();
			string requestUrl;

			var difference = DateTimeOffset.Now - asOn;

			if (difference < TimeSpan.FromDays(1.0))
			{
				requestUrl = LiveBaseURL;
			}
			else
			{
				requestUrl = HistoricalBaseURL;
				requestUrl += string.Format("date={0}&", asOn.ToString("yyyy-MM-dd"));
			}

			requestUrl += string.Format("access_key={0}", accessKey);

			if (baseCurrency != null)
			{
				requestUrl += string.Format("&source={0}", baseCurrency.ISOCurrencySymbol);
			}

			if (counterCurrency != null)
			{
				requestUrl += string.Format("&currencies={0}", counterCurrency.ISOCurrencySymbol);
			}

			var response = await client.GetAsync(requestUrl);

			response.EnsureSuccessStatusCode();

			var responseJsonString = await response.Content.ReadAsStringAsync();

			var responseJsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJsonString);

			var success = (bool)responseJsonObject["success"];

			if (success)
			{
				var result = new Dictionary<CurrencyPair, decimal>();

				var quotes = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJsonObject["quotes"].ToString());

				foreach (var quote in quotes)
				{
					string quoteBaseCurrencyCode = quote.Key.Substring(0, 3);
					string quoteCounterCurrencyCode = quote.Key.Substring(3);

					if (IsSupportedCurrency(quoteBaseCurrencyCode) && IsSupportedCurrency(quoteCounterCurrencyCode) && quoteBaseCurrencyCode.ToUpperInvariant() != quoteCounterCurrencyCode.ToUpperInvariant())
					{
						var quoteBaseCurrency = new CurrencyInfo(quoteBaseCurrencyCode);
						var quoteCounterCurrency = new CurrencyInfo(quoteCounterCurrencyCode);
						var rate = decimal.Parse(quote.Value.ToString(), CultureInfo.InvariantCulture);

						var quotePair = new CurrencyPair(quoteBaseCurrency, quoteCounterCurrency);

						result.Add(quotePair, rate);
					}
				}

				return result;
			}
			else
			{
				var errorDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJsonObject["error"].ToString());

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