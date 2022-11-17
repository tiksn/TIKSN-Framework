using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo
{
    public interface IExchangeRateDataRepository : IMongoRepository<ExchangeRateDataEntity, Guid>
    {
        Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
            Guid foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTime dateFrom,
            DateTime dateTo,
            CancellationToken cancellationToken);
    }
}
