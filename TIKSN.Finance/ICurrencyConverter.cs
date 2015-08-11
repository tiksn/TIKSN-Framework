using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
    public interface ICurrencyConverter
    {
        Task<Money> ConvertCurrencyAsync(Money BaseMoney, CurrencyInfo CounterCurrency, DateTimeOffset asOn);

        Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn);

        Task<decimal> GetExchangeRateAsync(CurrencyPair Pair, DateTimeOffset asOn);
    }
}