using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
    public interface ICurrencyConverter
    {
        Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn);

        Task<IEnumerable<CurrencyPair>> GetCurrencyPairsAsync(DateTimeOffset asOn);

        Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn);
    }
}