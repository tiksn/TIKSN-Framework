using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange
{
    public interface IExchangeRateService
    {
        Task<Money> ConvertCurrencyAsync(Money baseMoney, CurrencyInfo counterCurrency, DateTimeOffset asOn,
            CancellationToken cancellationToken);

        Task<decimal> GetExchangeRateAsync(CurrencyPair pair, DateTimeOffset asOn, CancellationToken cancellationToken);

        Task InitializeAsync(CancellationToken cancellationToken);
    }
}
