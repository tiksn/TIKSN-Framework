using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data;

public interface IForeignExchangeRepository
    : IRepository<ForeignExchangeEntity>
    , IQueryRepository<ForeignExchangeEntity, Guid>
    , IStreamRepository<ForeignExchangeEntity>
{
}
