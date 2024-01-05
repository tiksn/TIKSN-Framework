using TIKSN.Data;

namespace TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

public interface IForeignExchangeDataRepository
    : IRepository<ForeignExchangeDataEntity>
    , IQueryRepository<ForeignExchangeDataEntity, Guid>
    , IStreamRepository<ForeignExchangeDataEntity>;
