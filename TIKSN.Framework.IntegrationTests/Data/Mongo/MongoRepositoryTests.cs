using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TIKSN.Framework.IntegrationTests.Data.Mongo
{
    [Collection("ServiceProviderCollection")]
    public class MongoRepositoryTests
    {
        private readonly ServiceProviderFixture _serviceProviderFixture;

        public MongoRepositoryTests(ServiceProviderFixture serviceProviderFixture) =>
            this._serviceProviderFixture = serviceProviderFixture;

        [Fact]
        public async Task TestCreationAndRetrieval()
        {
            var testRepository = this._serviceProviderFixture.Services.GetRequiredService<ITestMongoRepository>();

            var testEntityId = Guid.NewGuid();
            var testEntity = new TestMongoEntity {ID = testEntityId, Value = Guid.NewGuid()};

            await testRepository.AddAsync(testEntity, default);

            var retrievedEntity = await testRepository.GetAsync(testEntityId, default);

            retrievedEntity.Value.Should().Be(testEntity.Value);
        }
    }
}
