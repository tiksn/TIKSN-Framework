using Microsoft.Extensions.Configuration;
using TIKSN.Data.LiteDB;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class TestLiteDbDatabaseProvider : LiteDbDatabaseProvider
{
    public TestLiteDbDatabaseProvider(IConfiguration configuration) : base(configuration, "LiteDB")
    {
    }
}
