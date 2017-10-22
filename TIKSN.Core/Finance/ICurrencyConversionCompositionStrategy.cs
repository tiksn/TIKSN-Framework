using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Finance
{
    public interface ICurrencyConversionCompositionStrategy
    {
        Task<Money> ConvertCurrencyAsync(Money baseMoney, IEnumerable<ICurrencyConverter> converters, CurrencyInfo counterCurrency, DateTimeOffset asOn);

        Task<decimal> GetExchangeRateAsync(IEnumerable<ICurrencyConverter> converters, CurrencyPair pair, DateTimeOffset asOn);
    }
}