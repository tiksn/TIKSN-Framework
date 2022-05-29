using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIKSN.Data.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public class ExchangeRateRepository : EntityQueryRepository<ExchangeRatesContext, ExchangeRateEntity, Guid>,
        IExchangeRateRepository
    {
        public ExchangeRateRepository(ExchangeRatesContext dbContext) : base(dbContext)
        {
        }

        public Task<ExchangeRateEntity> GetOrDefaultAsync(
            Guid foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTimeOffset asOn,
            CancellationToken cancellationToken) =>
            this.Entities
                .Where(item =>
                    item.ForeignExchangeID == foreignExchangeID && item.BaseCurrencyCode == baseCurrencyCode &&
                    item.CounterCurrencyCode == counterCurrencyCode)
                .OrderBy(entity => Math.Abs((entity.AsOn - asOn).Ticks))
                .Include(item => item.ForeignExchange)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyCollection<ExchangeRateEntity>> SearchAsync(
            Guid foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTimeOffset dateFrom,
            DateTimeOffset dateTo,
            CancellationToken cancellationToken) =>
            await this.Entities
                .Where(item =>
                    item.BaseCurrencyCode == baseCurrencyCode &&
                    item.CounterCurrencyCode == counterCurrencyCode &&
                    item.ForeignExchangeID == foreignExchangeID &&
                    item.AsOn >= dateFrom && item.AsOn <= dateTo)
                .Include(item => item.ForeignExchange)
                .ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }
}
