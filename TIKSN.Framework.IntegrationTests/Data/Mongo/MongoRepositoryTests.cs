using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.IntegrationTests;
using Xunit;

namespace TIKSN.Data.Mongo.IntegrationTests;

[Collection("ServiceProviderCollection")]
public class MongoRepositoryTests
{
    private readonly ServiceProviderFixture _serviceProviderFixture;

    public MongoRepositoryTests(ServiceProviderFixture serviceProviderFixture) =>
        this._serviceProviderFixture = serviceProviderFixture;

    [Fact]
    public async Task TestCreationAndRetrievalAsync()
    {
        var testRepository = this._serviceProviderFixture.GetServiceProvider().GetRequiredService<ITestMongoRepository>();

        var testEntityId = Guid.NewGuid();
        var testEntity = new TestMongoEntity { ID = testEntityId, Value = Guid.NewGuid() };

        await testRepository.AddAsync(testEntity, default).ConfigureAwait(true);

        var retrievedEntity = await testRepository.GetAsync(testEntityId, default).ConfigureAwait(true);

        _ = retrievedEntity.Value.Should().Be(testEntity.Value);
    }
}
