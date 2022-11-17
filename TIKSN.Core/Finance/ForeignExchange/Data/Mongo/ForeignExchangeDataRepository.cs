using System;
using TIKSN.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo
{
    public class ForeignExchangeDataRepository : MongoRepository<ForeignExchangeDataEntity, Guid>, IForeignExchangeDataRepository
    {
        public ForeignExchangeDataRepository(
            IMongoClientSessionProvider mongoClientSessionProvider,
            IMongoDatabaseProvider mongoDatabaseProvider) : base(
                mongoClientSessionProvider,
                mongoDatabaseProvider,
                "ForeignExchanges")
        {
        }
    }
}
