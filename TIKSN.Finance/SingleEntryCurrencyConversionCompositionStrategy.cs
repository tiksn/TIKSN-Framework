using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.Helpers;

namespace TIKSN.Finance
{
	public class SingleEntryCurrencyConversionCompositionStrategy : ICurrencyConversionCompositionStrategy
	{
		public async Task<Money> ConvertCurrencyAsync(Money baseMoney, IEnumerable<ICurrencyConverter> converters, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var filteredConverters = await CurrencyHelper.FilterConverters(converters, baseMoney.Currency, counterCurrency, asOn);

			var converter = filteredConverters.Single();

			return await converter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn);
		}

		public async Task<decimal> GetExchangeRateAsync(IEnumerable<ICurrencyConverter> converters, CurrencyPair pair, DateTimeOffset asOn)
		{
			var filteredConverters = await CurrencyHelper.FilterConverters(converters, pair, asOn);

			var converter = filteredConverters.Single();

			return await converter.GetExchangeRateAsync(pair, asOn);
		}
	}
}