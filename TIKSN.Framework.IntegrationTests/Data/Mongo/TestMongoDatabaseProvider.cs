using Microsoft.Extensions.Configuration;
using TIKSN.Data.Mongo;

namespace TIKSN.Framework.IntegrationTests.Data.Mongo
{
    public class TestMongoDatabaseProvider : MongoDatabaseProvider
    {
        public TestMongoDatabaseProvider(IConfiguration configuration) : base(configuration, "Mongo")
        {
        }
    }
}