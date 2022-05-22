using Xunit;

namespace TIKSN.Framework.IntegrationTests
{
    [CollectionDefinition("ServiceProviderCollection")]
    public class ServiceProviderCollection : ICollectionFixture<ServiceProviderFixture>
    {
    }
}
