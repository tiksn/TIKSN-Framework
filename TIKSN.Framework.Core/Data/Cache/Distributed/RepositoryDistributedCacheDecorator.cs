using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;

namespace TIKSN.Data.Cache.Distributed;

public class RepositoryDistributedCacheDecorator<TEntity, TIdentity>
    : DistributedCacheDecoratorBase<TEntity>, IRepository<TEntity>
    where TEntity : IEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    protected readonly IRepository<TEntity> repository;

    public RepositoryDistributedCacheDecorator(IRepository<TEntity> repository,
        IDistributedCache distributedCache,
        ISerializer<byte[]> serializer,
        IDeserializer<byte[]> deserializer,
        IOptions<DistributedCacheDecoratorOptions> genericOptions,
        IOptions<DistributedCacheDecoratorOptions<TEntity>> specificOptions)
        : base(distributedCache, serializer, deserializer, genericOptions, specificOptions)
    {
        this.repository = repository;
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.repository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemsAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.repository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.repository.RemoveRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemsAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await this.repository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.repository.UpdateRangeAsync(entities, cancellationToken).ConfigureAwait(false);

        await this.InvalidateCacheItemsAsync(entities, cancellationToken).ConfigureAwait(false);
    }

    protected virtual async Task InvalidateCacheEntityAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, entity.ID).ToString();

        await this.distributedCache.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
    }

    protected virtual async Task InvalidateCacheItemAsync(TEntity entity, CancellationToken cancellationToken)
        => await this.InvalidateCacheEntityAsync(entity, cancellationToken).ConfigureAwait(false);

    protected virtual async Task InvalidateCacheItemsAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken)
    {
        if (entities is null)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        foreach (var entity in entities)
        {
            await this.InvalidateCacheEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
