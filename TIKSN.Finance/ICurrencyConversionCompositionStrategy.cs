using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
	public interface ICurrencyConversionCompositionStrategy
	{
		Task<decimal> GetExchangeRateAsync(IEnumerable<ICurrencyConverter> converters, CurrencyPair pair, DateTimeOffset asOn);

		Task<Money> ConvertCurrencyAsync(Money baseMoney, IEnumerable<ICurrencyConverter> converters, CurrencyInfo counterCurrency, DateTimeOffset asOn);
	}
}
