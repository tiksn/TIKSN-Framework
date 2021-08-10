using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;

namespace TIKSN.Data.Cache.Distributed
{
    public class RepositoryDistributedCacheDecorator<TEntity, TIdentity>
        : DistributedCacheDecoratorBase<TEntity>, IRepository<TEntity>
        where TEntity : IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        protected readonly HashSet<string> _cacheKeychain;
        protected readonly IRepository<TEntity> _repository;

        public RepositoryDistributedCacheDecorator(IRepository<TEntity> repository,
            IDistributedCache distributedCache,
            ISerializer<byte[]> serializer,
            IDeserializer<byte[]> deserializer,
            IOptions<DistributedCacheDecoratorOptions> genericOptions,
            IOptions<DistributedCacheDecoratorOptions<TEntity>> specificOptions)
            : base(distributedCache, serializer, deserializer, genericOptions, specificOptions)
        {
            this._repository = repository;
            this._cacheKeychain = new HashSet<string>();
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await this._repository.AddAsync(entity, cancellationToken);

            await this.InvalidateCacheItemAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await this._repository.AddRangeAsync(entities, cancellationToken);

            await this.InvalidateCacheItemsAsync(entities, cancellationToken);
        }

        public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await this._repository.RemoveAsync(entity, cancellationToken);

            await this.InvalidateCacheItemAsync(entity, cancellationToken);
        }

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await this._repository.RemoveRangeAsync(entities, cancellationToken);

            await this.InvalidateCacheItemsAsync(entities, cancellationToken);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await this._repository.RemoveAsync(entity, cancellationToken);

            await this.InvalidateCacheItemAsync(entity, cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await this._repository.UpdateRangeAsync(entities, cancellationToken);

            await this.InvalidateCacheItemsAsync(entities, cancellationToken);
        }

        protected Task<TEntity> GetQueryFromDistributedCacheAsync(string cacheKey, Func<Task<TEntity>> getFromSource,
            CancellationToken cancellationToken)
        {
            this._cacheKeychain.Add(cacheKey);

            return this.GetFromDistributedCacheAsync(cacheKey, cancellationToken, getFromSource);
        }

        protected virtual async Task InvalidateCacheEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, entity.ID).ToString();

            await this._distributedCache.RemoveAsync(cacheKey, cancellationToken);
        }

        protected virtual async Task InvalidateCacheItemAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await this.InvalidateCacheEntityAsync(entity, cancellationToken);

            await this.InvalidateCacheQueryItemsAsync(cancellationToken);
        }

        protected virtual async Task InvalidateCacheItemsAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken)
        {
            foreach (var entity in entities)
            {
                await this.InvalidateCacheEntityAsync(entity, cancellationToken);
            }

            await this.InvalidateCacheQueryItemsAsync(cancellationToken);
        }

        protected virtual async Task InvalidateCacheQueryItemsAsync(CancellationToken cancellationToken)
        {
            foreach (var cacheKey in this._cacheKeychain)
            {
                await this._distributedCache.RemoveAsync(cacheKey, cancellationToken);
            }

            this._cacheKeychain.Clear();
        }
    }
}
