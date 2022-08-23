using System;
using TIKSN.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.Data.Mongo
{
    public class ForeignExchangeRepository : MongoRepository<ForeignExchangeEntity, Guid>, IForeignExchangeRepository
    {
        public ForeignExchangeRepository(
            IMongoClientSessionProvider mongoClientSessionProvider,
            IMongoDatabaseProvider mongoDatabaseProvider) : base(
                mongoClientSessionProvider,
                mongoDatabaseProvider,
                "ForeignExchanges")
        {
        }
    }
}
