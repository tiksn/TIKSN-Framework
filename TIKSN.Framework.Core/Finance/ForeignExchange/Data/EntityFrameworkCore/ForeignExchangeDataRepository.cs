using TIKSN.Data.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public class ForeignExchangeDataRepository : EntityQueryRepository<ExchangeRatesContext, ForeignExchangeDataEntity, Guid>,
    IForeignExchangeDataRepository
{
    public ForeignExchangeDataRepository(ExchangeRatesContext dbContext) : base(dbContext)
    {
    }

    protected override IOrderedQueryable<ForeignExchangeDataEntity> OrderedEntities
        => this.Entities.OrderBy(x => x.ID);
}
