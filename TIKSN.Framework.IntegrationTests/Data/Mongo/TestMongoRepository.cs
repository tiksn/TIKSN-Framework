using System;

namespace TIKSN.Data.Mongo.IntegrationTests;

public class TestMongoRepository : MongoRepository<TestMongoEntity, Guid>, ITestMongoRepository
{
    public TestMongoRepository(
        IMongoClientSessionProvider mongoClientSessionProvider,
        IMongoDatabaseProvider mongoDatabaseProvider) : base(mongoClientSessionProvider, mongoDatabaseProvider,
        "Tests")
    {
    }
}
