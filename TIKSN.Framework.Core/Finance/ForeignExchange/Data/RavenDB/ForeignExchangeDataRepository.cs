using TIKSN.Data.RavenDB;

namespace TIKSN.Finance.ForeignExchange.Data.RavenDB;

public class ForeignExchangeDataRepository : RavenRepository<ForeignExchangeDataEntity, Guid>, IForeignExchangeDataRepository
{
    public ForeignExchangeDataRepository(IRavenSessionProvider sessionProvider)
        : base(sessionProvider, "ForeignExchanges")
    {
    }
}
