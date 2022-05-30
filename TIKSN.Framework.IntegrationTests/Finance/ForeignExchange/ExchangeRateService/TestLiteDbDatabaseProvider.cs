using Microsoft.Extensions.Configuration;
using TIKSN.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    public class TestLiteDbDatabaseProvider : LiteDbDatabaseProvider
    {
        public TestLiteDbDatabaseProvider(IConfiguration configuration) : base(configuration, "LiteDB")
        {
        }
    }
}
