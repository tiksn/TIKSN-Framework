using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TIKSN.Finance.ForeignExchange
{
	/// <summary>
	/// Exchange rate converter of National Bank of Ukraine
	/// </summary>
	/// <seealso cref="TIKSN.Finance.ICurrencyConverter"/>
	public class NationalBankOfUkraine : ICurrencyConverter
	{
		private const string WebServiceUrlFormat = "http://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={0:yyyyMMdd}";
		private static readonly RegionInfo ukraine;
		private static readonly CultureInfo ukrainianCulture;
		private static readonly CurrencyInfo ukrainianHryvnia;

		static NationalBankOfUkraine()
		{
			ukrainianCulture = new CultureInfo("uk-UA");
			ukraine = new RegionInfo("uk-UA");
			ukrainianHryvnia = new CurrencyInfo(ukraine);
		}

		/// <summary>
		/// Converts the currency asynchronous.
		/// </summary>
		/// <param name="baseMoney">The base money.</param>
		/// <param name="counterCurrency">The counter currency.</param>
		/// <param name="asOn">As on.</param>
		/// <returns></returns>
		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var rate = await GetExchangeRateAsync(baseMoney.Currency, counterCurrency, asOn);

			return new Money(counterCurrency, baseMoney.Amount * rate);
		}

		/// <summary>
		/// Gets the currency pairs asynchronous.
		/// </summary>
		/// <param name="asOn">As on.</param>
		/// <returns></returns>
		public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			var records = await FetchExchangeRatesAsync(asOn);

			return records.Select(item => new CurrencyPair(ukrainianHryvnia, item.Item1)).Union(records.Select(item => new CurrencyPair(item.Item1, ukrainianHryvnia)));
		}

		/// <summary>
		/// Gets the exchange rate asynchronous.
		/// </summary>
		/// <param name="pair">The pair.</param>
		/// <param name="asOn">As on.</param>
		/// <returns></returns>
		public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			return await GetExchangeRateAsync(pair.BaseCurrency, pair.CounterCurrency, asOn);
		}

		private Exception CreatePairNotSupportedException(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency)
		{
			var ex = new NotSupportedException($"Currency pair is not supported by National Bank of Ukraine currency converter for given date.");

			ex.Data.Add("Base Currency", baseCurrency?.ISOCurrencySymbol);
			ex.Data.Add("Counter Currency", counterCurrency?.ISOCurrencySymbol);

			return ex;
		}

		private async Task<IEnumerable<Tuple<CurrencyInfo, decimal>>> FetchExchangeRatesAsync(DateTimeOffset asOn)
		{
			var httpClient = new HttpClient();
			var response = await httpClient.GetStringAsync(string.Format(WebServiceUrlFormat, asOn));
			var xdocument = XDocument.Parse(response);
			var result = new List<Tuple<CurrencyInfo, decimal>>();

			foreach (var currencyElement in xdocument.Element("exchange").Elements("currency"))
			{
				var currencyCode = currencyElement.Element("cc").Value;
				var rate = decimal.Parse(currencyElement.Element("rate").Value, CultureInfo.InvariantCulture);

				if (!string.IsNullOrEmpty(currencyCode))
					result.Add(new Tuple<CurrencyInfo, decimal>(new CurrencyInfo(currencyCode), rate));
			}

			return result;
		}

		private async Task<decimal> GetExchangeRateAsync(CurrencyInfo baseCurrency, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			if (counterCurrency == ukrainianHryvnia)
			{
				return await GetExchangeRateAsync(baseCurrency, asOn);
			}
			else if (baseCurrency == ukrainianHryvnia)
			{
				var counterRate = await GetExchangeRateAsync(counterCurrency, asOn);

				return 1m / counterRate;
			}
			else
			{
				throw CreatePairNotSupportedException(baseCurrency, counterCurrency);
			}
		}

		private async Task<decimal> GetExchangeRateAsync(CurrencyInfo currency, DateTimeOffset asOn)
		{
			var records = await FetchExchangeRatesAsync(asOn);
			var record = records.SingleOrDefault(item => item.Item1 == currency);

			if (record == null)
			{
				throw CreatePairNotSupportedException(null, currency);
			}
			else
			{
				return record.Item2;
			}
		}
	}
}