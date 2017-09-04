using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;

namespace TIKSN.Data.Cache.Distributed
{
	public class DistributedCacheQueryRepository<TEntity, TIdentity> : DistributedCacheRepository<TEntity, TIdentity>, IQueryRepository<TEntity, TIdentity>
		where TEntity : IEntity<TIdentity>
		where TIdentity : IEquatable<TIdentity>
	{
		public DistributedCacheQueryRepository(
			IDistributedCache distributedCache,
			ISerializer<byte[]> serializer,
			IDeserializer<byte[]> deserializer,
			IOptions<DistributedCacheDecoratorOptions> genericOptions,
			IOptions<DistributedCacheDecoratorOptions<TEntity>> specificOptions) : base(distributedCache, serializer, deserializer, genericOptions, specificOptions)
		{
		}

		public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
		{
			return GetFromDistributedCacheAsync<TEntity>(CreateEntryCacheKey(id), cancellationToken);
		}
	}
}
