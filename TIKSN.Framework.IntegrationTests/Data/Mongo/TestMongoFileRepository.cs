using MongoDB.Bson;
using TIKSN.Data.Mongo;

namespace TIKSN.IntegrationTests.Data.Mongo;

public class TestMongoFileRepository : MongoFileRepository<ObjectId, object>, ITestMongoFileRepository
{
    public TestMongoFileRepository(IMongoDatabaseProvider mongoDatabaseProvider)
      : base(mongoDatabaseProvider, "Test")
    {
    }
}
