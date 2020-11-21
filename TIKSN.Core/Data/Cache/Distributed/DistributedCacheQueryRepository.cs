using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cachedBytes = await _distributedCache.GetAsync(CreateEntryCacheKey(id), cancellationToken);

            return cachedBytes != null;
        }

        public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var result = await GetFromDistributedCacheAsync<TEntity>(CreateEntryCacheKey(id), cancellationToken);

            if (result == null)
            {
                throw new NullReferenceException("Result retrieved from cache or from original source is null.");
            }

            return result;
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken)
        {
            return GetFromDistributedCacheAsync<TEntity>(CreateEntryCacheKey(id), cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken)
        {
            return await BatchOperationHelper.BatchOperationAsync(ids, cancellationToken, (id, ct) => GetAsync(id, ct));
        }
    }
}