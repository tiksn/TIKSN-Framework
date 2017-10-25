using TIKSN.Data.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public class ForeignExchangeQueryRepository : EntityQueryRepository<ExchangeRatesContext, ForeignExchangeEntity, int>
    {
        public ForeignExchangeQueryRepository(ExchangeRatesContext dbContext) : base(dbContext)
        {
        }
    }
}
