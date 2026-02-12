using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.Cache.Hybrid;

public class RepositoryHybridCacheDecorator<TEntity, TIdentity>
    : HybridCacheDecoratorBase<TEntity>, IRepository<TEntity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    public RepositoryHybridCacheDecorator(
        IRepository<TEntity> repository,
        HybridCache hybridCache,
        IOptions<HybridCacheDecoratorOptions> genericOptions,
        IOptions<HybridCacheDecoratorOptions<TEntity>> specificOptions)
        : base(hybridCache, genericOptions, specificOptions)
        => this.Repository = repository;

    protected IRepository<TEntity> Repository { get; }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.Repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.Repository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemsAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.Repository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.Repository.RemoveRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemsAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.Repository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.Repository.UpdateRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemsAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    protected virtual async Task InvalidateCacheEntityAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(EntityType, CacheKeyKind.Entity, entity.ID).ToString();

        await this.HybridCache.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
    }

    protected virtual async Task InvalidateCacheItemAsync(TEntity entity, CancellationToken cancellationToken)
        => await this.InvalidateCacheEntityAsync(entity, cancellationToken).ConfigureAwait(false);

    protected virtual async Task InvalidateCacheItemsAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            await this.InvalidateCacheEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
