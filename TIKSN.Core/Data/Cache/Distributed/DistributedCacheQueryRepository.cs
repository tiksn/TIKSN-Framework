using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;

namespace TIKSN.Data.Cache.Distributed
{
    public class DistributedCacheQueryRepository<TEntity, TIdentity> : DistributedCacheRepository<TEntity, TIdentity>,
        IQueryRepository<TEntity, TIdentity>
        where TEntity : IEntity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        public DistributedCacheQueryRepository(
            IDistributedCache distributedCache,
            ISerializer<byte[]> serializer,
            IDeserializer<byte[]> deserializer,
            IOptions<DistributedCacheDecoratorOptions> genericOptions,
            IOptions<DistributedCacheDecoratorOptions<TEntity>> specificOptions) : base(distributedCache, serializer,
            deserializer, genericOptions, specificOptions)
        {
        }

        public async Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var cachedBytes = await this._distributedCache.GetAsync(this.CreateEntryCacheKey(id), cancellationToken).ConfigureAwait(false);

            return cachedBytes != null;
        }

        public async Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            var result =
                await this.GetFromDistributedCacheAsync<TEntity>(this.CreateEntryCacheKey(id), cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                throw new NullReferenceException("Result retrieved from cache or from original source is null.");
            }

            return result;
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
            this.GetFromDistributedCacheAsync<TEntity>(this.CreateEntryCacheKey(id), cancellationToken);

        public async Task<IEnumerable<TEntity>>
            ListAsync(IEnumerable<TIdentity> ids, CancellationToken cancellationToken) =>
            await BatchOperationHelper.BatchOperationAsync(ids, cancellationToken, this.GetAsync).ConfigureAwait(false);
    }
}
