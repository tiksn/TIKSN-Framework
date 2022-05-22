using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public interface IExchangeRateRepository : IQueryRepository<ExchangeRateEntity, int>,
        IRepository<ExchangeRateEntity>
    {
        Task<ExchangeRateEntity> GetOrDefaultAsync(
            int foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTimeOffset asOn,
            CancellationToken cancellationToken);

        Task<int> GetMaximalIdAsync(CancellationToken cancellationToken);

        Task<IReadOnlyCollection<ExchangeRateEntity>> SearchAsync(
            int foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTimeOffset dateFrom,
            DateTimeOffset dateTo,
            CancellationToken cancellationToken);
    }
}
