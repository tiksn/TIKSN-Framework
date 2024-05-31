using Microsoft.EntityFrameworkCore;
using TIKSN.Data.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public class ExchangeRateDataRepository
    : EntityQueryRepository<ExchangeRatesContext, ExchangeRateDataEntity, Guid>
    , IExchangeRateDataRepository
{
    public ExchangeRateDataRepository(ExchangeRatesContext dbContext) : base(dbContext)
    {
    }

    protected override IOrderedQueryable<ExchangeRateDataEntity> OrderedEntities
        => this.Entities.OrderBy(x => x.AsOn);

    public async Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        Guid foreignExchangeID,
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
        => await this.Entities
            .Where(item =>
                item.BaseCurrencyCode == baseCurrencyCode &&
                item.CounterCurrencyCode == counterCurrencyCode &&
                item.ForeignExchangeID == foreignExchangeID &&
                item.AsOn >= dateFrom && item.AsOn <= dateTo)
            .Include(item => item.ForeignExchange)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<ExchangeRateDataEntity>> SearchAsync(
        string baseCurrencyCode,
        string counterCurrencyCode,
        DateTime dateFrom,
        DateTime dateTo,
        CancellationToken cancellationToken)
        => await this.Entities
        .Where(item =>
            item.BaseCurrencyCode == baseCurrencyCode &&
            item.CounterCurrencyCode == counterCurrencyCode &&
            item.AsOn >= dateFrom && item.AsOn <= dateTo)
        .Include(item => item.ForeignExchange)
        .ToArrayAsync(cancellationToken).ConfigureAwait(false);
}
