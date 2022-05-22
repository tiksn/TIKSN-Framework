using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.Cache.Memory
{
    public class RepositoryMemoryCacheDecorator<TEntity, TIdentity>
        : MemoryCacheDecoratorBase<TEntity>, IRepository<TEntity>
        where TEntity : IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        protected readonly HashSet<object> _cacheKeychain;
        protected readonly IRepository<TEntity> _repository;

        public RepositoryMemoryCacheDecorator(IRepository<TEntity> repository,
            IMemoryCache memoryCache,
            IOptions<MemoryCacheDecoratorOptions> genericOptions,
            IOptions<MemoryCacheDecoratorOptions<TEntity>> specificOptions)
            : base(memoryCache, genericOptions, specificOptions)
        {
            this._repository = repository;
            this._cacheKeychain = new HashSet<object>();
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await this._repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

            this.InvalidateCacheItem(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await this._repository.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);

            this.InvalidateCacheItems(entities);
        }

        public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await this._repository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

            this.InvalidateCacheItem(entity);
        }

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await this._repository.RemoveRangeAsync(entities, cancellationToken).ConfigureAwait(false);

            this.InvalidateCacheItems(entities);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await this._repository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);

            this.InvalidateCacheItem(entity);
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await this._repository.UpdateRangeAsync(entities, cancellationToken).ConfigureAwait(false);

            this.InvalidateCacheItems(entities);
        }

        protected Task<TResult> GetQueryFromMemoryCacheAsync<TResult>(object cacheKey,
            Func<Task<TResult>> getFromSource)
        {
            _ = this._cacheKeychain.Add(cacheKey);

            return this.GetFromMemoryCacheAsync(cacheKey, getFromSource);
        }

        protected virtual void InvalidateCacheEntity(TEntity entity)
        {
            var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, entity.ID);

            this._memoryCache.Remove(cacheKey);
        }

        protected virtual void InvalidateCacheItem(TEntity entity)
        {
            this.InvalidateCacheEntity(entity);

            this.InvalidateCacheQueryItems();
        }

        protected virtual void InvalidateCacheItems(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.InvalidateCacheEntity(entity);
            }

            this.InvalidateCacheQueryItems();
        }

        protected virtual void InvalidateCacheQueryItems()
        {
            foreach (var cacheKey in this._cacheKeychain)
            {
                this._memoryCache.Remove(cacheKey);
            }

            this._cacheKeychain.Clear();
        }
    }
}
