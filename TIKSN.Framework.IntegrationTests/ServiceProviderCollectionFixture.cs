using Xunit;

namespace TIKSN.IntegrationTests;

[CollectionDefinition("ServiceProviderCollection")]
public class ServiceProviderCollectionFixture : ICollectionFixture<ServiceProviderFixture>
{
}
