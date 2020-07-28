using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Finance.Helpers;

namespace TIKSN.Finance
{
    public class SingleEntryCurrencyConversionCompositionStrategy : ICurrencyConversionCompositionStrategy
    {
        public async Task<Money> ConvertCurrencyAsync(Money baseMoney, IEnumerable<ICurrencyConverter> converters, CurrencyInfo counterCurrency, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var filteredConverters = await CurrencyHelper.FilterConverters(converters, baseMoney.Currency, counterCurrency, asOn, cancellationToken);

            var converter = filteredConverters.Single();

            return await converter.ConvertCurrencyAsync(baseMoney, counterCurrency, asOn, cancellationToken);
        }

        public async Task<decimal> GetExchangeRateAsync(IEnumerable<ICurrencyConverter> converters, CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken)
        {
            var filteredConverters = await CurrencyHelper.FilterConverters(converters, pair, asOn, cancellationToken);

            var converter = filteredConverters.Single();

            return await converter.GetExchangeRateAsync(pair, asOn, cancellationToken);
        }
    }
}