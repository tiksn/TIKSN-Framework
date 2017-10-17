using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange
{
	public interface IExchangeRateProvider
	{
		Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(DateTimeOffset asOn);
	}
}
