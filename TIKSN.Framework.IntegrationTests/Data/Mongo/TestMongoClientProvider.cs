using Microsoft.Extensions.Configuration;
using TIKSN.Data.Mongo;

namespace TIKSN.IntegrationTests.Data.Mongo;

public class TestMongoClientProvider : MongoClientProviderBase
{
    public TestMongoClientProvider(IConfiguration configuration) : base(configuration, "Mongo")
    {
    }
}
