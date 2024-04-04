using TIKSN.Data.RavenDB;

namespace TIKSN.Finance.ForeignExchange.Data.RavenDB;

public interface IForeignExchangeDataRepository : IRavenRepository<ForeignExchangeDataEntity, Guid>;
