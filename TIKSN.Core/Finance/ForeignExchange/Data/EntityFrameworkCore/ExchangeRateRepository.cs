using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Data.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public class ExchangeRateRepository : EntityQueryRepository<ExchangeRatesContext, ExchangeRateEntity, int>, IExchangeRateRepository
    {
        public ExchangeRateRepository(ExchangeRatesContext dbContext) : base(dbContext)
        {
        }

        public async Task<ExchangeRateEntity> GetAsync(
            int foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTimeOffset asOn,
            CancellationToken cancellationToken)
        {
            return Entities
                .OrderBy(entity => Math.Abs((entity.AsOn - asOn).Ticks))
                .First();
        }

        public async Task<int> GetMaximalIdAsync(CancellationToken cancellationToken)
        {
            var entity = await Entities
                .OrderByDescending(item => item.ID)
                .FirstOrDefaultAsync();

            if (entity == null)
                return 0;

            return entity.ID;
        }

        public async Task<IReadOnlyCollection<ExchangeRateEntity>> SearchAsync(
            int foreignExchangeID,
            string baseCurrencyCode,
            string counterCurrencyCode,
            DateTimeOffset dateFrom,
            DateTimeOffset dateTo,
            CancellationToken cancellationToken)
        {
            return await Entities
                .Where(item =>
                    item.BaseCurrencyCode == baseCurrencyCode &&
                    item.CounterCurrencyCode == counterCurrencyCode &&
                    item.ForeignExchangeID == foreignExchangeID &&
                    item.AsOn >= dateFrom && item.AsOn <= dateTo)
                .ToArrayAsync(cancellationToken);
        }
    }
}