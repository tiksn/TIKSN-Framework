using LiteDB;
using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB;

public class ForeignExchangeDataRepository : LiteDbRepository<ForeignExchangeDataEntity, Guid>, IForeignExchangeDataRepository
{
    public ForeignExchangeDataRepository(ILiteDbDatabaseProvider databaseProvider) : base(databaseProvider,
        "ForeignExchanges", x => new BsonValue(x))
    {
    }
}
