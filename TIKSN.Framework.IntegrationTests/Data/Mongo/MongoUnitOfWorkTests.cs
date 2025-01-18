using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polly;
using Shouldly;
using TIKSN.Data;
using Xunit;

namespace TIKSN.IntegrationTests.Data.Mongo;

[Collection("ServiceProviderCollection")]
public class MongoUnitOfWorkTests
{
    private readonly ServiceProviderFixture serviceProviderFixture;

    public MongoUnitOfWorkTests(ServiceProviderFixture serviceProviderFixture) =>
        this.serviceProviderFixture = serviceProviderFixture;

    [Fact]
    public async Task TestCreationAndRetrieval()
    {
        var testEntityId = Guid.NewGuid();
        var testEntity = new TestMongoEntity { ID = testEntityId, Value = Guid.NewGuid() };
        TestMongoEntity retrievedEntity = null;

        var mongoUnitOfWorkFactory =
            this.serviceProviderFixture.GetServiceProvider("MongoDB").GetRequiredService<IUnitOfWorkFactory>();

        await using (var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default))
        {
            var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

            await testRepository.AddAsync(testEntity, default);

            await mongoUnitOfWork.CompleteAsync(default);
        }

        await using (var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default))
        {
            var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

            retrievedEntity = await testRepository.GetAsync(testEntityId, default);

            await mongoUnitOfWork.CompleteAsync(default);
        }

        retrievedEntity.Value.ShouldBe(testEntity.Value);
    }

    [Fact]
    public async Task TestConcurrentUpdates()
    {
        var testEntityId = Guid.NewGuid();
        var testEntity = new TestMongoEntity { ID = testEntityId, Value = Guid.NewGuid(), Version = 1 };
        TestMongoEntity retrievedEntity = null;

        var mongoUnitOfWorkFactory =
            this.serviceProviderFixture.GetServiceProvider("MongoDB").GetRequiredService<IUnitOfWorkFactory>();

        await using (var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default))
        {
            var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

            await testRepository.AddAsync(testEntity, default);

            await mongoUnitOfWork.CompleteAsync(default);
        }

        var tasks = Enumerable.Repeat(0, 3).Select(_ => UpdateEntityWithRetry(mongoUnitOfWorkFactory, testEntityId))
            .ToArray();
        await Task.WhenAll(tasks);

        await using (var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default))
        {
            var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

            retrievedEntity = await testRepository.GetAsync(testEntityId, default);

            await mongoUnitOfWork.CompleteAsync(default);
        }

        retrievedEntity.Version.ShouldBe(4);

        static Task UpdateEntityWithRetry(IUnitOfWorkFactory mongoUnitOfWorkFactory, Guid testEntityId) => Policy.Handle<MongoCommandException>()
                .WaitAndRetryAsync(10, i => TimeSpan.FromMilliseconds(i * 10))
                .ExecuteAsync(() => UpdateEntity(mongoUnitOfWorkFactory, testEntityId));

        static async Task UpdateEntity(IUnitOfWorkFactory mongoUnitOfWorkFactory, Guid testEntityId)
        {
            await using var mongoUnitOfWork = await mongoUnitOfWorkFactory.CreateAsync(default);
            var testRepository = mongoUnitOfWork.Services.GetRequiredService<ITestMongoRepository>();

            var entity = await testRepository.GetAsync(testEntityId, default);

            entity.Version++;

            await testRepository.UpdateAsync(entity, default);

            await mongoUnitOfWork.CompleteAsync(default);
        }
    }
}
