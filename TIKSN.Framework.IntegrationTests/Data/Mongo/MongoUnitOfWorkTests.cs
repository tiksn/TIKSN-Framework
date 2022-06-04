using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polly;
using TIKSN.IntegrationTests;
using Xunit;

namespace TIKSN.Data.Mongo.IntegrationTests
{
    [Collection("ServiceProviderCollection")]
    public class MongoUnitOfWorkTests
    {
        private readonly ServiceProviderFixture _serviceProviderFixture;

        public MongoUnitOfWorkTests(ServiceProviderFixture serviceProviderFixture) =>
            this._serviceProviderFixture = serviceProviderFixture;

        [Fact]
        public async Task TestCreationAndRetrievalAsync()
        {
            var testEntityId = Guid.NewGuid();
            var testEntity = new TestMongoEntity { ID = testEntityId, Value = Guid.NewGuid() };
            TestMongoEntity retrievedEntity = null;

            var mongoUnitOfWorkFactory =
                this._serviceProviderFixture.GetServiceProvider().GetRequiredService<IMongoUnitOfWorkFactory>();

            await using (var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default).ConfigureAwait(true))
            {
                var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

                await testRepository.AddAsync(testEntity, default).ConfigureAwait(true);

                await mongoUnitOfWork.CompleteAsync(default).ConfigureAwait(true);
            }

            await using (var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default).ConfigureAwait(true))
            {
                var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

                retrievedEntity = await testRepository.GetAsync(testEntityId, default).ConfigureAwait(true);

                await mongoUnitOfWork.CompleteAsync(default).ConfigureAwait(true);
            }

            _ = retrievedEntity.Value.Should().Be(testEntity.Value);
        }

        [Fact]
        public async Task TestConcurrentUpdatesAsync()
        {
            var testEntityId = Guid.NewGuid();
            var testEntity = new TestMongoEntity { ID = testEntityId, Value = Guid.NewGuid(), Version = 1 };
            TestMongoEntity retrievedEntity = null;

            var mongoUnitOfWorkFactory =
                this._serviceProviderFixture.GetServiceProvider().GetRequiredService<IMongoUnitOfWorkFactory>();

            await using (var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default).ConfigureAwait(true))
            {
                var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

                await testRepository.AddAsync(testEntity, default).ConfigureAwait(true);

                await mongoUnitOfWork.CompleteAsync(default).ConfigureAwait(true);
            }

            var tasks = Enumerable.Repeat(0, 3).Select(_ => UpdateEntityWithRetry(mongoUnitOfWorkFactory, testEntityId))
                .ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(true);

            await using (var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default).ConfigureAwait(true))
            {
                var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

                retrievedEntity = await testRepository.GetAsync(testEntityId, default).ConfigureAwait(true);

                await mongoUnitOfWork.CompleteAsync(default).ConfigureAwait(true);
            }

            _ = retrievedEntity.Version.Should().Be(4);

            static Task UpdateEntityWithRetry(IMongoUnitOfWorkFactory mongoUnitOfWorkFactory, Guid testEntityId) => Policy.Handle<MongoCommandException>()
                    .WaitAndRetryAsync(10, i => TimeSpan.FromMilliseconds(i * 10))
                    .ExecuteAsync(() => UpdateEntity(mongoUnitOfWorkFactory, testEntityId));

            static async Task UpdateEntity(IMongoUnitOfWorkFactory mongoUnitOfWorkFactory, Guid testEntityId)
            {
                await using var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default).ConfigureAwait(true);
                var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

                var entity = await testRepository.GetAsync(testEntityId, default).ConfigureAwait(true);

                entity.Version += 1;

                await testRepository.UpdateAsync(entity, default).ConfigureAwait(true);

                await mongoUnitOfWork.CompleteAsync(default).ConfigureAwait(true);
            }
        }
    }
}
