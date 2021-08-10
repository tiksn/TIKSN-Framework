using TIKSN.Data.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public class ForeignExchangeRepository : EntityQueryRepository<ExchangeRatesContext, ForeignExchangeEntity, int>,
        IForeignExchangeRepository
    {
        public ForeignExchangeRepository(ExchangeRatesContext dbContext) : base(dbContext)
        {
        }
    }
}
