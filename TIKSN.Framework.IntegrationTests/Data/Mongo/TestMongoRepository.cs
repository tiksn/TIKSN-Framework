using System;
using TIKSN.Data.Mongo;

namespace TIKSN.Framework.IntegrationTests.Data.Mongo
{
    public class TestMongoRepository : MongoRepository<TestMongoEntity, Guid>, ITestMongoRepository
    {
        public TestMongoRepository(
            IMongoClientSessionProvider mongoClientSessionProvider,
            IMongoDatabaseProvider mongoDatabaseProvider) : base(mongoClientSessionProvider, mongoDatabaseProvider,
            "Tests")
        {
        }
    }
}
