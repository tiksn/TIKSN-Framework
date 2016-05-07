using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange
{
	/// <summary>
	/// Exchange rate converter of National Bank of Ukraine
	/// </summary>
	/// <seealso cref="TIKSN.Finance.ICurrencyConverter"/>
	public class NationalBankOfUkraine : ICurrencyConverter
	{
		private const string WebServiceUrlFormat = "http://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={0:yyyyMMdd}&json";

		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn)
		{
			throw new NotImplementedException();
		}

		public async Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn)
		{
			throw new NotImplementedException();
		}

		private async Task<IEnumerable<ExchangeRateRecord>> FetchExchangeRatesAsync(DateTimeOffset asOn)
		{
			var httpClient = new HttpClient();
			var response = await httpClient.GetStringAsync(string.Format(WebServiceUrlFormat, asOn));
			return JsonConvert.DeserializeObject<List<ExchangeRateRecord>>(response);
		}

		private class ExchangeRateRecord
		{
		}
	}
}