using System;
using TIKSN.Data.EntityFrameworkCore;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore
{
    public class ForeignExchangeRepository : EntityQueryRepository<ExchangeRatesContext, ForeignExchangeEntity, Guid>,
        IForeignExchangeRepository
    {
        public ForeignExchangeRepository(ExchangeRatesContext dbContext) : base(dbContext)
        {
        }
    }
}
