using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Finance.Helpers
{
	internal static class CurrencyHelper
	{
		public static async Task<IEnumerable<ICurrencyConverter>> FilterConverters(IEnumerable<ICurrencyConverter> converters, CurrencyPair pair, DateTimeOffset asOn)
		{
			return await FilterConverters(converters, pair.BaseCurrency, pair.CounterCurrency, asOn);
		}

		public static async Task<IEnumerable<ICurrencyConverter>> FilterConverters(IEnumerable<ICurrencyConverter> converters, CurrencyInfo baseCurrency, CurrencyInfo counterCurrency, DateTimeOffset asOn)
		{
			var filteredConverters = new List<ICurrencyConverter>();

			foreach (var converter in converters)
			{
				var pairs = await converter.GetCurrencyPairsAsync(asOn);

				if (pairs.Any(item => item.BaseCurrency == baseCurrency && item.CounterCurrency == counterCurrency))
				{
					filteredConverters.Add(converter);
				}
			}

			return filteredConverters;
		}
	}
}
