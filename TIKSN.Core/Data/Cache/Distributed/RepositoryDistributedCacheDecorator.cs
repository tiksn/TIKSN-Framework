using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
			_repository = repository;
			_cacheKeychain = new HashSet<string>();
		}

		public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await _repository.AddAsync(entity, cancellationToken);

			await InvalidateCacheItemAsync(entity, cancellationToken);
		}

		public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			await _repository.AddRangeAsync(entities, cancellationToken);

			await InvalidateCacheItemsAsync(entities, cancellationToken);
		}

		public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await _repository.RemoveAsync(entity, cancellationToken);

			await InvalidateCacheItemAsync(entity, cancellationToken);

		}

		public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			await _repository.RemoveRangeAsync(entities, cancellationToken);

			await InvalidateCacheItemsAsync(entities, cancellationToken);
		}

		public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await _repository.RemoveAsync(entity, cancellationToken);

			await InvalidateCacheItemAsync(entity, cancellationToken);
		}

		public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			await _repository.UpdateRangeAsync(entities, cancellationToken);

			await InvalidateCacheItemsAsync(entities, cancellationToken);
		}

		protected Task<TEntity> GetQueryFromDistributedCacheAsync(string cacheKey, Func<Task<TEntity>> getFromSource, CancellationToken cancellationToken)
		{
			_cacheKeychain.Add(cacheKey);

			return GetFromDistributedCacheAsync(cacheKey, cancellationToken, getFromSource);
		}

		protected virtual async Task InvalidateCacheEntityAsync(TEntity entity, CancellationToken cancellationToken)
		{
			var cacheKey = Tuple.Create(entityType, CacheKeyKind.Entity, entity.ID).ToString();

			await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
		}

		protected virtual async Task InvalidateCacheItemAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await InvalidateCacheEntityAsync(entity, cancellationToken);

			await InvalidateCacheQueryItemsAsync(cancellationToken);
		}

		protected virtual async Task InvalidateCacheItemsAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			foreach (var entity in entities)
				await InvalidateCacheEntityAsync(entity, cancellationToken);

			await InvalidateCacheQueryItemsAsync(cancellationToken);
		}

		protected virtual async Task InvalidateCacheQueryItemsAsync(CancellationToken cancellationToken)
		{
			foreach (var cacheKey in _cacheKeychain)
				await _distributedCache.RemoveAsync(cacheKey, cancellationToken);

			_cacheKeychain.Clear();
		}
	}
}
