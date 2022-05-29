using System;
using LiteDB;
using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.Data.LiteDB
{
    public class ForeignExchangeRepository : LiteDbRepository<ForeignExchangeEntity, Guid>, IForeignExchangeRepository
    {
        public ForeignExchangeRepository(ILiteDbDatabaseProvider databaseProvider) : base(databaseProvider,
            "ForeignExchanges", x => new BsonValue(x))
        {
        }
    }
}
