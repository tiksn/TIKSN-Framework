using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public interface IExchangeRateDataRepository
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
