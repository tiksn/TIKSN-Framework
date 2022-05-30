using Xunit;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    [CollectionDefinition("LiteDbServiceProviderCollection")]
    public class LiteDbServiceProviderCollection : ICollectionFixture<LiteDbServiceProviderFixture>
    {
    }
}
