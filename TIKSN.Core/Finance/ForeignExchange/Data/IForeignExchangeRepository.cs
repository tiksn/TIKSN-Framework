using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data
{
    public interface IForeignExchangeRepository : IQueryRepository<ForeignExchangeEntity, int>,
        IRepository<ForeignExchangeEntity>
    {
    }
}
