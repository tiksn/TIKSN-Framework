using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LanguageExt;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TIKSN.Data;
using TIKSN.Data.Cache.Distributed;
using TIKSN.DependencyInjection;
using TIKSN.Serialization;
using TIKSN.Serialization.MessagePack;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Data.Cache.Distributed;

public class DistributedCacheDecoratorBaseTests
{
    private readonly Dictionary<int, int> counterMap;
    private readonly Dictionary<int, TestEntity> entityMap;
    private readonly ITestService testService;

    public DistributedCacheDecoratorBaseTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = services.AddDistributedMemoryCache();
        _ = services.AddSingleton<ISerializer<byte[]>, MessagePackSerializer>();
        _ = services.AddSingleton<IDeserializer<byte[]>, MessagePackDeserializer>();
        _ = services.AddSingleton(ContractlessStandardResolver.Options);
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        _ = containerBuilder.RegisterModule<CoreModule>();
        this.entityMap = new[]
        {
                new TestEntity(1778174815, Guid.NewGuid(), "Item5"),
                new TestEntity(2118700136, Guid.NewGuid(), "Item6"),
                new TestEntity(2035652629, Guid.NewGuid(), "Item7"),
                new TestEntity(430380339, Guid.NewGuid(), "Item8"),
        }.ToDictionary(k => k.ID, v => v);
        this.counterMap = this.entityMap.ToDictionary(k => k.Key, _ => 0);
        _ = containerBuilder.RegisterInstance(this.entityMap).SingleInstance();
        _ = containerBuilder.RegisterInstance(this.counterMap).SingleInstance();
        _ = containerBuilder.RegisterType<RealTestService>().As<ITestService>();
        containerBuilder.RegisterDecorator<TestServiceMemoryCacheDecorator, ITestService>();
        var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());
        this.testService = serviceProvider.GetService<ITestService>();
    }

    public interface ITestService
    {
        Task<Option<TestEntity>> FindByExternalIdAsync(Guid externalId, CancellationToken cancellationToken);

        Task<Option<TestEntity>> FindByIdAsync(int id, CancellationToken cancellationToken);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByExternalId_ThenShouldCacheHit()
    {
        // Arrange

        var entity = this.entityMap[2118700136];
        var oldCount = this.counterMap[entity.ID];

        // Act

        var actual = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);

        var newCount = this.counterMap[entity.ID];

        // Assert

        actual.IsSome.ShouldBeTrue();
        actual.Single().ID.ShouldBe(entity.ID);
        actual.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual.Single().Name.ShouldBe(entity.Name);
        newCount.ShouldBe(oldCount + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByExternalIdTwice_ThenShouldCacheHitOnce()
    {
        // Arrange

        var entity = this.entityMap[430380339];
        var oldCount = this.counterMap[entity.ID];

        // Act

        var actual1 = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);
        var newCount1 = this.counterMap[entity.ID];
        var actual2 = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);
        var newCount2 = this.counterMap[entity.ID];

        // Assert

        actual1.IsSome.ShouldBeTrue();
        actual1.Single().ID.ShouldBe(entity.ID);
        actual1.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual1.Single().Name.ShouldBe(entity.Name);
        newCount1.ShouldBe(oldCount + 1);

        actual2.IsSome.ShouldBeTrue();
        actual2.Single().ID.ShouldBe(entity.ID);
        actual2.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual2.Single().Name.ShouldBe(entity.Name);
        newCount2.ShouldBe(oldCount + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedById_ThenShouldCacheHit()
    {
        // Arrange

        var entity = this.entityMap[1778174815];
        var oldCount = this.counterMap[entity.ID];

        // Act

        var actual = await this.testService.FindByIdAsync(1778174815, default);

        var newCount = this.counterMap[entity.ID];

        // Assert

        actual.IsSome.ShouldBeTrue();
        actual.Single().ID.ShouldBe(entity.ID);
        actual.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual.Single().Name.ShouldBe(entity.Name);
        newCount.ShouldBe(oldCount + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByIdTwice_ThenShouldCacheHitOnce()
    {
        // Arrange

        var entity = this.entityMap[2035652629];
        var oldCount = this.counterMap[entity.ID];

        // Act

        var actual1 = await this.testService.FindByIdAsync(2035652629, default);
        var newCount1 = this.counterMap[entity.ID];
        var actual2 = await this.testService.FindByIdAsync(2035652629, default);
        var newCount2 = this.counterMap[entity.ID];

        // Assert

        actual1.IsSome.ShouldBeTrue();
        actual1.Single().ID.ShouldBe(entity.ID);
        actual1.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual1.Single().Name.ShouldBe(entity.Name);
        newCount1.ShouldBe(oldCount + 1);

        actual2.IsSome.ShouldBeTrue();
        actual2.Single().ID.ShouldBe(entity.ID);
        actual2.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual2.Single().Name.ShouldBe(entity.Name);
        newCount2.ShouldBe(oldCount + 1);
    }

    [Fact]
    public async Task GivenMissingEntity_WhenRequestedByExternalId_ThenShouldCacheMiss()
    {
        // Arrange

        // Act

        var actual = await this.testService.FindByExternalIdAsync(Guid.NewGuid(), default);

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenMissingEntity_WhenRequestedById_ThenShouldCacheMiss()
    {
        // Arrange

        // Act

        var actual = await this.testService.FindByIdAsync(1009129315, default);

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    public record TestEntity(int ID, Guid ExternalID, string Name) : IEntity<int>;

    public class RealTestService : ITestService
    {
        private readonly Dictionary<int, int> counterMap;
        private readonly Dictionary<int, TestEntity> entityMap;

        public RealTestService(
            Dictionary<int, TestEntity> entityMap,
            Dictionary<int, int> counterMap)
        {
            this.entityMap = entityMap ?? throw new ArgumentNullException(nameof(entityMap));
            this.counterMap = counterMap ?? throw new ArgumentNullException(nameof(counterMap));
        }

        public Task<Option<TestEntity>> FindByExternalIdAsync(Guid externalId, CancellationToken cancellationToken) => Task.FromResult(this.Find(x => x.Value.ExternalID == externalId));

        public Task<Option<TestEntity>> FindByIdAsync(int id, CancellationToken cancellationToken) => Task.FromResult(this.Find(x => x.Value.ID == id));

        private Option<TestEntity> Find(Func<KeyValuePair<int, TestEntity>, bool> predicate)
        {
            var match = this.entityMap.Where(predicate).ToArray();

            if (match.Length == 0)
            {
                return None;
            }

            var entity = match.Single().Value;

            this.counterMap[entity.ID]++;

            return entity;
        }
    }

    public class TestServiceMemoryCacheDecorator : DistributedCacheDecoratorBase<TestEntity>, ITestService
    {
        private readonly ITestService testService;

        public TestServiceMemoryCacheDecorator(
            ITestService testService,
            IDistributedCache distributedCache,
            ISerializer<byte[]> serializer,
            IDeserializer<byte[]> deserializer,
            IOptions<DistributedCacheDecoratorOptions> genericOptions,
            IOptions<DistributedCacheDecoratorOptions<TestEntity>> specificOptions)
            : base(distributedCache, serializer, deserializer, genericOptions, specificOptions)
            => this.testService = testService ?? throw new ArgumentNullException(nameof(testService));

        public Task<Option<TestEntity>> FindByExternalIdAsync(Guid externalId, CancellationToken cancellationToken)
        {
            var cacheKey = ("TestServiceDecorator", "ByExternalID", externalId).ToString();

            return this.FindFromDistributedCacheAsync(cacheKey, () => this.testService.FindByExternalIdAsync(externalId, cancellationToken), cancellationToken);
        }

        public Task<Option<TestEntity>> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cacheKey = ("TestServiceDecorator", "ByID", id).ToString();

            return this.FindFromDistributedCacheAsync(cacheKey, () => this.testService.FindByIdAsync(id, cancellationToken), cancellationToken);
        }
    }
}
