using Xunit;

namespace TIKSN.IntegrationTests;

[CollectionDefinition("ServiceProviderCollection")]
public class ServiceProviderCollection : ICollectionFixture<ServiceProviderFixture>
{
}
