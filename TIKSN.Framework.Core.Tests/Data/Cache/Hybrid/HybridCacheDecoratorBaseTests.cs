using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LanguageExt;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TIKSN.Data;
using TIKSN.Data.Cache.Hybrid;
using TIKSN.DependencyInjection;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Data.Cache.Hybrid;

public class HybridCacheDecoratorBaseTests
{
    private readonly Dictionary<int, TestEntity> entityMap;
    private readonly ITestService testService;

    public HybridCacheDecoratorBaseTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = services.AddHybridCache();
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        _ = containerBuilder.RegisterModule<CoreModule>();
        this.entityMap = new[]
        {
                new TestEntity(1796652465, Guid.NewGuid(), "Item1"),
                new TestEntity(489680564, Guid.NewGuid(), "Item2"),
                new TestEntity(1242007805, Guid.NewGuid(), "Item3"),
                new TestEntity(307097393, Guid.NewGuid(), "Item4"),
                new TestEntity(1778174815, Guid.NewGuid(), "Item5"),
                new TestEntity(2118700136, Guid.NewGuid(), "Item6"),
                new TestEntity(2035652629, Guid.NewGuid(), "Item7"),
                new TestEntity(430380339, Guid.NewGuid(), "Item8"),
        }.ToDictionary(k => k.ID, v => v);
        _ = containerBuilder.RegisterInstance(this.entityMap).SingleInstance();
        _ = containerBuilder.RegisterType<RealTestService>().As<ITestService>();
        containerBuilder.RegisterDecorator<TestServiceHybridCacheDecorator, ITestService>();
        var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());
        this.testService = serviceProvider.GetService<ITestService>();
    }

    public interface ITestService
    {
        public Task<TestEntity> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken);

        public Task<TestEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByExternalIdAsync_ThenShouldCacheHit()
    {
        // Arrange

        var entity = this.entityMap[2118700136];
        var oldETag = entity.ETag;

        // Act

        var actual = await this.testService.GetByExternalIdAsync(entity.ExternalID, default);

        // Assert

        _ = actual.ShouldNotBeNull();
        actual.ID.ShouldBe(entity.ID);
        actual.ExternalID.ShouldBe(entity.ExternalID);
        actual.Name.ShouldBe(entity.Name);
        actual.ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByExternalIdTwiceAsync_ThenShouldCacheHitOnce()
    {
        // Arrange

        var entity = this.entityMap[430380339];
        var oldETag = entity.ETag;

        // Act

        var actual1 = await this.testService.GetByExternalIdAsync(entity.ExternalID, default);
        var actual2 = await this.testService.GetByExternalIdAsync(entity.ExternalID, default);

        // Assert

        _ = actual1.ShouldNotBeNull();
        actual1.ID.ShouldBe(entity.ID);
        actual1.ExternalID.ShouldBe(entity.ExternalID);
        actual1.Name.ShouldBe(entity.Name);
        actual1.ETag.ShouldBe(oldETag + 1);

        _ = actual2.ShouldNotBeNull();
        actual2.ID.ShouldBe(entity.ID);
        actual2.ExternalID.ShouldBe(entity.ExternalID);
        actual2.Name.ShouldBe(entity.Name);
        actual2.ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByIdAsync_ThenShouldCacheHit()
    {
        // Arrange

        var entity = this.entityMap[1778174815];
        var oldETag = entity.ETag;

        // Act

        var actual = await this.testService.GetByIdAsync(1778174815, default);

        // Assert

        _ = actual.ShouldNotBeNull();
        actual.ID.ShouldBe(entity.ID);
        actual.ExternalID.ShouldBe(entity.ExternalID);
        actual.Name.ShouldBe(entity.Name);
        actual.ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByIdTwiceAsync_ThenShouldCacheHitOnce()
    {
        // Arrange

        var entity = this.entityMap[2035652629];
        var oldETag = entity.ETag;

        // Act

        var actual1 = await this.testService.GetByIdAsync(2035652629, default);
        var actual2 = await this.testService.GetByIdAsync(2035652629, default);

        // Assert

        _ = actual1.ShouldNotBeNull();
        actual1.ID.ShouldBe(entity.ID);
        actual1.ExternalID.ShouldBe(entity.ExternalID);
        actual1.Name.ShouldBe(entity.Name);
        actual1.ETag.ShouldBe(oldETag + 1);

        _ = actual2.ShouldNotBeNull();
        actual2.ID.ShouldBe(entity.ID);
        actual2.ExternalID.ShouldBe(entity.ExternalID);
        actual2.Name.ShouldBe(entity.Name);
        actual2.ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public async Task GivenMissingEntity_WhenRequestedByExternalIdAsync_ThenShouldCacheMiss() =>
        // Arrange

        // Act & Assert

        _ = await Should.ThrowAsync<EntityNotFoundException>(async () => await this.testService.GetByExternalIdAsync(Guid.NewGuid(), default));

    [Fact]
    public async Task GivenMissingEntity_WhenRequestedByIdAsync_ThenShouldCacheMiss() =>
        // Arrange

        // Act & Assert

        _ = await Should.ThrowAsync<EntityNotFoundException>(async () => await this.testService.GetByIdAsync(1009129315, default));

    public record TestEntity(int ID, Guid ExternalID, string Name) : IEntity<int>
    {
        public int ETag { get; set; }

        public void Advance() => this.ETag++;
    }

    public class RealTestService : ITestService
    {
        private readonly Dictionary<int, TestEntity> entityMap;

        public RealTestService(Dictionary<int, TestEntity> entityMap)
            => this.entityMap = entityMap ?? throw new ArgumentNullException(nameof(entityMap));

        public Task<TestEntity> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken) => Task.FromResult(this.Get(x => x.Value.ExternalID == externalId));

        public Task<TestEntity> GetByIdAsync(int id, CancellationToken cancellationToken) => Task.FromResult(this.Get(x => x.Value.ID == id));

        private TestEntity Get(Func<KeyValuePair<int, TestEntity>, bool> predicate)
        {
            var match = this.entityMap.Where(predicate).ToArray();

            if (match.Length == 0)
            {
                throw new EntityNotFoundException();
            }

            var entity = match.Single().Value;

            entity.Advance();
            return entity;
        }
    }

    public class TestServiceHybridCacheDecorator : HybridCacheDecoratorBase<TestEntity>, ITestService
    {
        private readonly ITestService testService;

        public TestServiceHybridCacheDecorator(
            ITestService testService,
            HybridCache hybridCache,
            IOptions<HybridCacheDecoratorOptions> genericOptions,
            IOptions<HybridCacheDecoratorOptions<TestEntity>> specificOptions)
            : base(hybridCache, genericOptions, specificOptions)
            => this.testService = testService ?? throw new ArgumentNullException(nameof(testService));

        public async Task<TestEntity> GetByExternalIdAsync(Guid externalId, CancellationToken cancellationToken)
        {
            var cacheKey = ("TestServiceDecorator", "ByExternalID", externalId).ToString();

            return await this.GetFromHybridCacheAsync(cacheKey, async ct => await this.testService.GetByExternalIdAsync(externalId, ct).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        public async Task<TestEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cacheKey = ("TestServiceDecorator", "ByID", id).ToString();

            return await this.GetFromHybridCacheAsync(cacheKey, async ct => await this.testService.GetByIdAsync(id, ct).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
    }
}
