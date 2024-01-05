using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB;

public interface IForeignExchangeDataRepository : ILiteDbRepository<ForeignExchangeDataEntity, Guid>;
