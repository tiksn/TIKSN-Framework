using System;
using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public interface IForeignExchangeRepository : IQueryRepository<ForeignExchangeEntity, Guid>,
        IRepository<ForeignExchangeEntity>
    {
    }
}
