using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;

namespace TIKSN.Data.Cache.Distributed
{
	public class DistributedCacheRepository<TEntity, TIdentity> : DistributedCacheDecoratorBase<TEntity>, IRepository<TEntity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>
	{
		protected HashSet<TIdentity> _cachedIdentities;

		public DistributedCacheRepository(
			IDistributedCache distributedCache,
			ISerializer<byte[]> serializer,
			IDeserializer<byte[]> deserializer,
			IOptions<DistributedCacheDecoratorOptions> genericOptions,
			IOptions<DistributedCacheDecoratorOptions<TEntity>> specificOptions) : base(distributedCache, serializer, deserializer, genericOptions, specificOptions)
		{
			_cachedIdentities = new HashSet<TIdentity>();
		}

		public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await SetToDistributedCacheAsync(CreateEntryCacheKey(entity.ID), entity, cancellationToken);

			_cachedIdentities.Add(entity.ID);
		}

		public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, AddAsync);
		}

		public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
		{
			await _distributedCache.RemoveAsync(CreateEntryCacheKey(entity.ID), cancellationToken);

			_cachedIdentities.Remove(entity.ID);
		}

		public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, RemoveAsync);
		}

		public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
		{
			return SetToDistributedCacheAsync(CreateEntryCacheKey(entity.ID), entity, cancellationToken);
		}

		public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
		{
			return BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, UpdateAsync);
		}

		protected string CreateEntryCacheKey(TIdentity identity)
		{
			return Tuple.Create(entityType, CacheKeyKind.Entity, identity).ToString();
		}
	}
}
