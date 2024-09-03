using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TIKSN.IntegrationTests.Data.Mongo;

[Collection("ServiceProviderCollection")]
public class MongoRepositoryTests
{
    private readonly ServiceProviderFixture serviceProviderFixture;

    public MongoRepositoryTests(ServiceProviderFixture serviceProviderFixture) =>
        this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));

    [Fact]
    public async Task TestCreationAndRetrieval()
    {
        var testRepository = this.serviceProviderFixture.GetServiceProvider().GetRequiredService<ITestMongoRepository>();

        var testEntityId = Guid.NewGuid();
        var testEntity = new TestMongoEntity { ID = testEntityId, Value = Guid.NewGuid() };

        await testRepository.AddAsync(testEntity, default);

        var retrievedEntity = await testRepository.GetAsync(testEntityId, default);

        _ = retrievedEntity.Value.Should().Be(testEntity.Value);
    }
}
