using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.Cache.Memory;

public class RepositoryMemoryCacheDecorator<TEntity, TIdentity>
    : MemoryCacheDecoratorBase<TEntity>, IRepository<TEntity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    protected readonly IRepository<TEntity> repository;

    public RepositoryMemoryCacheDecorator(
        IRepository<TEntity> repository,
        IMemoryCache memoryCache,
        IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<TEntity>> specificOptions)
        : base(memoryCache, genericOptions, specificOptions)
    {
        this.repository = repository;
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        this.InvalidateCacheItem(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.repository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        this.InvalidateCacheItems(entities);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.repository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

        this.InvalidateCacheItem(entity);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.repository.RemoveRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        this.InvalidateCacheItems(entities);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.repository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

        this.InvalidateCacheItem(entity);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.repository.UpdateRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        this.InvalidateCacheItems(entities);
    }

    protected virtual void InvalidateCacheEntity(TEntity entity)
    {
        var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, entity.ID);

        this.memoryCache.Remove(cacheKey);
    }

    protected virtual void InvalidateCacheItem(TEntity entity)
        => this.InvalidateCacheEntity(entity);

    protected virtual void InvalidateCacheItems(IEnumerable<TEntity> entities)
    {
        if (entities is null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        foreach (var entity in entities)
        {
            this.InvalidateCacheEntity(entity);
        }
    }
}
