using Microsoft.Extensions.Configuration;
using TIKSN.Data.Mongo;

namespace TIKSN.Framework.IntegrationTests.Data.Mongo;

public class TestMongoDatabaseProvider : MongoDatabaseProviderBase
{
    public TestMongoDatabaseProvider(
        IMongoClientProvider mongoClientProvider,
        IConfiguration configuration) : base(
        mongoClientProvider,
        configuration,
        "Mongo")
    {
    }
}
