using System;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public interface IExchangeRateRepository : IQueryRepository<ExchangeRateEntity, int>, IRepository<ExchangeRateEntity>
    {
        Task<ExchangeRateEntity> GetAsync(string baseCurrencyCode, string counterCurrencyCode, DateTimeOffset dateFrom, DateTimeOffset dateTo, CancellationToken cancellationToken);
    }
}
