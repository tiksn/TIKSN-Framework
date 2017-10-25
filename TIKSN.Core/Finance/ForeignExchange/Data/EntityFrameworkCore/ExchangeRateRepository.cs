using TIKSN.Data.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public class ExchangeRateRepository : EntityQueryRepository<ExchangeRatesContext, ExchangeRateEntity, int>
    {
        public ExchangeRateRepository(ExchangeRatesContext dbContext) : base(dbContext)
        {
        }
    }
}
