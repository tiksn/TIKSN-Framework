using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TIKSN.Data;
using TIKSN.Data.Cache.Memory;
using TIKSN.DependencyInjection;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Data.Cache.Memory;

public class MemoryCacheDecoratorBaseTests
{
    private readonly Dictionary<int, TestEntity> entityMap;
    private readonly ITestService testService;

    public MemoryCacheDecoratorBaseTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
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
        containerBuilder.RegisterDecorator<TestServiceMemoryCacheDecorator, ITestService>();
        var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());
        this.testService = serviceProvider.GetService<ITestService>();
    }

    public interface ITestService
    {
        Option<TestEntity> FindByExternalId(Guid externalId);

        Task<Option<TestEntity>> FindByExternalIdAsync(Guid externalId, CancellationToken cancellationToken);

        Option<TestEntity> FindById(int id);

        Task<Option<TestEntity>> FindByIdAsync(int id, CancellationToken cancellationToken);
    }

    [Fact]
    public void GivenExistingEntity_WhenRequestedByExternalId_ThenShouldCacheHit()
    {
        // Arrange

        var entity = this.entityMap[489680564];
        var oldETag = entity.ETag;

        // Act

        var actual = this.testService.FindByExternalId(entity.ExternalID);

        // Assert

        actual.IsSome.ShouldBeTrue();
        actual.Single().ID.ShouldBe(entity.ID);
        actual.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual.Single().Name.ShouldBe(entity.Name);
        actual.Single().ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByExternalIdAsync_ThenShouldCacheHit()
    {
        // Arrange

        var entity = this.entityMap[2118700136];
        var oldETag = entity.ETag;

        // Act

        var actual = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);

        // Assert

        actual.IsSome.ShouldBeTrue();
        actual.Single().ID.ShouldBe(entity.ID);
        actual.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual.Single().Name.ShouldBe(entity.Name);
        actual.Single().ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public void GivenExistingEntity_WhenRequestedByExternalIdTwice_ThenShouldCacheHitOnce()
    {
        // Arrange

        var entity = this.entityMap[307097393];
        var oldETag = entity.ETag;

        // Act

        var actual1 = this.testService.FindByExternalId(entity.ExternalID);
        var actual2 = this.testService.FindByExternalId(entity.ExternalID);

        // Assert

        actual1.IsSome.ShouldBeTrue();
        actual1.Single().ID.ShouldBe(entity.ID);
        actual1.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual1.Single().Name.ShouldBe(entity.Name);
        actual1.Single().ETag.ShouldBe(oldETag + 1);

        actual2.IsSome.ShouldBeTrue();
        actual2.Single().ID.ShouldBe(entity.ID);
        actual2.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual2.Single().Name.ShouldBe(entity.Name);
        actual2.Single().ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByExternalIdTwiceAsync_ThenShouldCacheHitOnce()
    {
        // Arrange

        var entity = this.entityMap[430380339];
        var oldETag = entity.ETag;

        // Act

        var actual1 = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);
        var actual2 = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);

        // Assert

        actual1.IsSome.ShouldBeTrue();
        actual1.Single().ID.ShouldBe(entity.ID);
        actual1.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual1.Single().Name.ShouldBe(entity.Name);
        actual1.Single().ETag.ShouldBe(oldETag + 1);

        actual2.IsSome.ShouldBeTrue();
        actual2.Single().ID.ShouldBe(entity.ID);
        actual2.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual2.Single().Name.ShouldBe(entity.Name);
        actual2.Single().ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public void GivenExistingEntity_WhenRequestedById_ThenShouldCacheHit()
    {
        // Arrange

        var entity = this.entityMap[1796652465];
        var oldETag = entity.ETag;

        // Act

        var actual = this.testService.FindById(1796652465);

        // Assert

        actual.IsSome.ShouldBeTrue();
        actual.Single().ID.ShouldBe(entity.ID);
        actual.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual.Single().Name.ShouldBe(entity.Name);
        actual.Single().ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByIdAsync_ThenShouldCacheHit()
    {
        // Arrange

        var entity = this.entityMap[1778174815];
        var oldETag = entity.ETag;

        // Act

        var actual = await this.testService.FindByIdAsync(1778174815, default);

        // Assert

        actual.IsSome.ShouldBeTrue();
        actual.Single().ID.ShouldBe(entity.ID);
        actual.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual.Single().Name.ShouldBe(entity.Name);
        actual.Single().ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public void GivenExistingEntity_WhenRequestedByIdTwice_ThenShouldCacheHitOnce()
    {
        // Arrange

        var entity = this.entityMap[1242007805];
        var oldETag = entity.ETag;

        // Act

        var actual1 = this.testService.FindById(1242007805);
        var actual2 = this.testService.FindById(1242007805);

        // Assert

        actual1.IsSome.ShouldBeTrue();
        actual1.Single().ID.ShouldBe(entity.ID);
        actual1.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual1.Single().Name.ShouldBe(entity.Name);
        actual1.Single().ETag.ShouldBe(oldETag + 1);

        actual2.IsSome.ShouldBeTrue();
        actual2.Single().ID.ShouldBe(entity.ID);
        actual2.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual2.Single().Name.ShouldBe(entity.Name);
        actual2.Single().ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByIdTwiceAsync_ThenShouldCacheHitOnce()
    {
        // Arrange

        var entity = this.entityMap[2035652629];
        var oldETag = entity.ETag;

        // Act

        var actual1 = await this.testService.FindByIdAsync(2035652629, default);
        var actual2 = await this.testService.FindByIdAsync(2035652629, default);

        // Assert

        actual1.IsSome.ShouldBeTrue();
        actual1.Single().ID.ShouldBe(entity.ID);
        actual1.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual1.Single().Name.ShouldBe(entity.Name);
        actual1.Single().ETag.ShouldBe(oldETag + 1);

        actual2.IsSome.ShouldBeTrue();
        actual2.Single().ID.ShouldBe(entity.ID);
        actual2.Single().ExternalID.ShouldBe(entity.ExternalID);
        actual2.Single().Name.ShouldBe(entity.Name);
        actual2.Single().ETag.ShouldBe(oldETag + 1);
    }

    [Fact]
    public void GivenMissingEntity_WhenRequestedByExternalId_ThenShouldCacheMiss()
    {
        // Arrange

        // Act

        var actual = this.testService.FindByExternalId(Guid.NewGuid());

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenMissingEntity_WhenRequestedByExternalIdAsync_ThenShouldCacheMiss()
    {
        // Arrange

        // Act

        var actual = await this.testService.FindByExternalIdAsync(Guid.NewGuid(), default);

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenMissingEntity_WhenRequestedById_ThenShouldCacheMiss()
    {
        // Arrange

        // Act

        var actual = this.testService.FindById(1009129315);

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task GivenMissingEntity_WhenRequestedByIdAsync_ThenShouldCacheMiss()
    {
        // Arrange

        // Act

        var actual = await this.testService.FindByIdAsync(1009129315, default);

        // Assert

        actual.IsNone.ShouldBeTrue();
    }

    public record TestEntity(int ID, Guid ExternalID, string Name) : IEntity<int>
    {
        public int ETag { get; private set; }

        public void Advance() => this.ETag++;
    }

    public class RealTestService : ITestService
    {
        private readonly Dictionary<int, TestEntity> entityMap;

        public RealTestService(Dictionary<int, TestEntity> entityMap)
            => this.entityMap = entityMap ?? throw new ArgumentNullException(nameof(entityMap));

        public Option<TestEntity> FindByExternalId(Guid externalId) => this.Find(x => x.Value.ExternalID == externalId);

        public Task<Option<TestEntity>> FindByExternalIdAsync(Guid externalId, CancellationToken cancellationToken) => Task.FromResult(this.Find(x => x.Value.ExternalID == externalId));

        public Option<TestEntity> FindById(int id) => this.Find(x => x.Value.ID == id);

        public Task<Option<TestEntity>> FindByIdAsync(int id, CancellationToken cancellationToken) => Task.FromResult(this.Find(x => x.Value.ID == id));

        private Option<TestEntity> Find(Func<KeyValuePair<int, TestEntity>, bool> predicate)
        {
            var match = this.entityMap.Where(predicate).ToArray();

            if (match.Length == 0)
            {
                return None;
            }

            var entity = match.Single().Value;

            entity.Advance();
            return entity;
        }
    }

    public class TestServiceMemoryCacheDecorator : MemoryCacheDecoratorBase<TestEntity>, ITestService
    {
        private readonly ITestService testService;

        public TestServiceMemoryCacheDecorator(
            ITestService testService,
            IMemoryCache memoryCache,
            IOptions<MemoryCacheDecoratorOptions> genericOptions,
            IOptions<MemoryCacheDecoratorOptions<TestEntity>> specificOptions)
            : base(memoryCache, genericOptions, specificOptions)
            => this.testService = testService ?? throw new ArgumentNullException(nameof(testService));

        public Option<TestEntity> FindByExternalId(Guid externalId)
        {
            var cacheKey = ("TestServiceDecorator", "ByExternalID", externalId);

            return this.FindFromMemoryCache(cacheKey, () => this.testService.FindByExternalId(externalId));
        }

        public Task<Option<TestEntity>> FindByExternalIdAsync(Guid externalId, CancellationToken cancellationToken)
        {
            var cacheKey = ("TestServiceDecorator", "ByExternalID", externalId);

            return this.FindFromMemoryCacheAsync(cacheKey, () => this.testService.FindByExternalIdAsync(externalId, cancellationToken));
        }

        public Option<TestEntity> FindById(int id)
        {
            var cacheKey = ("TestServiceDecorator", "ByID", id);

            return this.FindFromMemoryCache(cacheKey, () => this.testService.FindById(id));
        }

        public Task<Option<TestEntity>> FindByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cacheKey = ("TestServiceDecorator", "ByID", id);

            return this.FindFromMemoryCacheAsync(cacheKey, () => this.testService.FindByIdAsync(id, cancellationToken));
        }
    }
}
