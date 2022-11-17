using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public interface IExchangeRateRepository : IQueryRepository<ExchangeRateEntity, Guid>,
        IRepository<ExchangeRateEntity>
    {
        Task<IReadOnlyList<ExchangeRateEntity>> SearchAsync(
            Guid foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTime dateFrom,
            DateTime dateTo,
            CancellationToken cancellationToken);
    }
}
