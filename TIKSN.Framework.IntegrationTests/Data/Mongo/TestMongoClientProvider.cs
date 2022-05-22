using Microsoft.Extensions.Configuration;
using TIKSN.Data.Mongo;

namespace TIKSN.Framework.IntegrationTests.Data.Mongo
{
    public class TestMongoClientProvider : MongoClientProviderBase
    {
        public TestMongoClientProvider(IConfiguration configuration) : base(configuration, "Mongo")
        {
        }
    }
}
