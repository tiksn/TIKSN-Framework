using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
			_repository = repository;
			_cacheKeychain = new HashSet<object>();
		}

		public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await _repository.AddAsync(entity, cancellationToken);

			InvalidateCacheItem(entity);
		}

		public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			await _repository.AddRangeAsync(entities, cancellationToken);

			InvalidateCacheItems(entities);
		}

		public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await _repository.RemoveAsync(entity, cancellationToken);

			InvalidateCacheItem(entity);

		}

		public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			await _repository.RemoveRangeAsync(entities, cancellationToken);

			InvalidateCacheItems(entities);
		}

		public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await _repository.RemoveAsync(entity, cancellationToken);

			InvalidateCacheItem(entity);
		}

		public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			await _repository.UpdateRangeAsync(entities, cancellationToken);

			InvalidateCacheItems(entities);
		}

		protected Task<TEntity> GetQueryFromMemoryCacheAsync(object cacheKey, Func<Task<TEntity>> getFromSource)
		{
			_cacheKeychain.Add(cacheKey);

			return GetFromMemoryCacheAsync(cacheKey, getFromSource);
		}

		protected virtual void InvalidateCacheEntity(TEntity entity)
		{
			var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, entity.ID);

			_memoryCache.Remove(cacheKey);
		}

		protected virtual void InvalidateCacheItem(TEntity entity)
		{
			InvalidateCacheEntity(entity);

			InvalidateCacheQueryItems();
		}

		protected virtual void InvalidateCacheItems(IEnumerable<TEntity> entities)
		{
			foreach (var entity in entities)
				InvalidateCacheEntity(entity);

			InvalidateCacheQueryItems();
		}

		protected virtual void InvalidateCacheQueryItems()
		{
			foreach (var cacheKey in _cacheKeychain)
				_memoryCache.Remove(cacheKey);

			_cacheKeychain.Clear();
		}
	}
}
