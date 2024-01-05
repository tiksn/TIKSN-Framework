using TIKSN.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo;

public interface IForeignExchangeDataRepository : IMongoRepository<ForeignExchangeDataEntity, Guid>;
