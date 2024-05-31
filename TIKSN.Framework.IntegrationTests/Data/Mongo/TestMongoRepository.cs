using System;
using MongoDB.Driver;
using TIKSN.Data.Mongo;

namespace TIKSN.IntegrationTests.Data.Mongo;

public class TestMongoRepository : MongoRepository<TestMongoEntity, Guid>, ITestMongoRepository
{
    public TestMongoRepository(
        IMongoClientSessionProvider mongoClientSessionProvider,
        IMongoDatabaseProvider mongoDatabaseProvider) : base(mongoClientSessionProvider, mongoDatabaseProvider,
        "Tests")
    {
    }

    protected override SortDefinition<TestMongoEntity> PageSortDefinition
        => Builders<TestMongoEntity>.Sort.Ascending(x => x.ID);
}
