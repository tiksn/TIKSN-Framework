using Microsoft.Extensions.Configuration;
using TIKSN.Data.Mongo;

namespace TIKSN.Framework.IntegrationTests.Data.Mongo
{
    public class TestMongoDatabaseProviderBase : MongoDatabaseProviderBase
    {
        public TestMongoDatabaseProviderBase(
            IMongoClientProvider mongoClientProvider,
            IConfiguration configuration) : base(
            mongoClientProvider,
            configuration,
            "Mongo")
        {
        }
    }
}