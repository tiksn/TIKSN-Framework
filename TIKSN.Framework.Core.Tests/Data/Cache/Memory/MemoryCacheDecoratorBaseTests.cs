using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TIKSN.DependencyInjection;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Data.Cache.Memory.Tests;

public class MemoryCacheDecoratorBaseTests
{
    private readonly Dictionary<int, TestEntity> entityMap;
    private readonly ITestService testService;

    public MemoryCacheDecoratorBaseTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkPlatform();
        var containerBuilder = new ContainerBuilder();
        containerBuilder.Populate(services);
        _ = containerBuilder.RegisterModule<CoreModule>();
        _ = containerBuilder.RegisterModule<PlatformModule>();
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

        _ = actual.IsSome.Should().BeTrue();
        _ = actual.Single().ID.Should().Be(entity.ID);
        _ = actual.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual.Single().Name.Should().Be(entity.Name);
        _ = actual.Single().ETag.Should().Be(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByExternalId_ThenShouldCacheHitAsync()
    {
        // Arrange

        var entity = this.entityMap[2118700136];
        var oldETag = entity.ETag;

        // Act

        var actual = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);

        // Assert

        _ = actual.IsSome.Should().BeTrue();
        _ = actual.Single().ID.Should().Be(entity.ID);
        _ = actual.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual.Single().Name.Should().Be(entity.Name);
        _ = actual.Single().ETag.Should().Be(oldETag + 1);
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

        _ = actual1.IsSome.Should().BeTrue();
        _ = actual1.Single().ID.Should().Be(entity.ID);
        _ = actual1.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual1.Single().Name.Should().Be(entity.Name);
        _ = actual1.Single().ETag.Should().Be(oldETag + 1);

        _ = actual2.IsSome.Should().BeTrue();
        _ = actual2.Single().ID.Should().Be(entity.ID);
        _ = actual2.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual2.Single().Name.Should().Be(entity.Name);
        _ = actual2.Single().ETag.Should().Be(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByExternalIdTwice_ThenShouldCacheHitOnceAsync()
    {
        // Arrange

        var entity = this.entityMap[430380339];
        var oldETag = entity.ETag;

        // Act

        var actual1 = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);
        var actual2 = await this.testService.FindByExternalIdAsync(entity.ExternalID, default);

        // Assert

        _ = actual1.IsSome.Should().BeTrue();
        _ = actual1.Single().ID.Should().Be(entity.ID);
        _ = actual1.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual1.Single().Name.Should().Be(entity.Name);
        _ = actual1.Single().ETag.Should().Be(oldETag + 1);

        _ = actual2.IsSome.Should().BeTrue();
        _ = actual2.Single().ID.Should().Be(entity.ID);
        _ = actual2.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual2.Single().Name.Should().Be(entity.Name);
        _ = actual2.Single().ETag.Should().Be(oldETag + 1);
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

        _ = actual.IsSome.Should().BeTrue();
        _ = actual.Single().ID.Should().Be(entity.ID);
        _ = actual.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual.Single().Name.Should().Be(entity.Name);
        _ = actual.Single().ETag.Should().Be(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedById_ThenShouldCacheHitAsync()
    {
        // Arrange

        var entity = this.entityMap[1778174815];
        var oldETag = entity.ETag;

        // Act

        var actual = await this.testService.FindByIdAsync(1778174815, default);

        // Assert

        _ = actual.IsSome.Should().BeTrue();
        _ = actual.Single().ID.Should().Be(entity.ID);
        _ = actual.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual.Single().Name.Should().Be(entity.Name);
        _ = actual.Single().ETag.Should().Be(oldETag + 1);
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

        _ = actual1.IsSome.Should().BeTrue();
        _ = actual1.Single().ID.Should().Be(entity.ID);
        _ = actual1.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual1.Single().Name.Should().Be(entity.Name);
        _ = actual1.Single().ETag.Should().Be(oldETag + 1);

        _ = actual2.IsSome.Should().BeTrue();
        _ = actual2.Single().ID.Should().Be(entity.ID);
        _ = actual2.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual2.Single().Name.Should().Be(entity.Name);
        _ = actual2.Single().ETag.Should().Be(oldETag + 1);
    }

    [Fact]
    public async Task GivenExistingEntity_WhenRequestedByIdTwice_ThenShouldCacheHitOnceAsync()
    {
        // Arrange

        var entity = this.entityMap[2035652629];
        var oldETag = entity.ETag;

        // Act

        var actual1 = await this.testService.FindByIdAsync(2035652629, default);
        var actual2 = await this.testService.FindByIdAsync(2035652629, default);

        // Assert

        _ = actual1.IsSome.Should().BeTrue();
        _ = actual1.Single().ID.Should().Be(entity.ID);
        _ = actual1.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual1.Single().Name.Should().Be(entity.Name);
        _ = actual1.Single().ETag.Should().Be(oldETag + 1);

        _ = actual2.IsSome.Should().BeTrue();
        _ = actual2.Single().ID.Should().Be(entity.ID);
        _ = actual2.Single().ExternalID.Should().Be(entity.ExternalID);
        _ = actual2.Single().Name.Should().Be(entity.Name);
        _ = actual2.Single().ETag.Should().Be(oldETag + 1);
    }

    [Fact]
    public void GivenMissingEntity_WhenRequestedByExternalId_ThenShouldCacheMiss()
    {
        // Arrange

        // Act

        var actual = this.testService.FindByExternalId(Guid.NewGuid());

        // Assert

        _ = actual.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task GivenMissingEntity_WhenRequestedByExternalId_ThenShouldCacheMissAsync()
    {
        // Arrange

        // Act

        var actual = await this.testService.FindByExternalIdAsync(Guid.NewGuid(), default);

        // Assert

        _ = actual.IsNone.Should().BeTrue();
    }

    [Fact]
    public void GivenMissingEntity_WhenRequestedById_ThenShouldCacheMiss()
    {
        // Arrange

        // Act

        var actual = this.testService.FindById(1009129315);

        // Assert

        _ = actual.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task GivenMissingEntity_WhenRequestedById_ThenShouldCacheMissAsync()
    {
        // Arrange

        // Act

        var actual = await this.testService.FindByIdAsync(1009129315, default);

        // Assert

        _ = actual.IsNone.Should().BeTrue();
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
