using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;

namespace TIKSN.Data.Cache.Distributed
{
    public abstract class DistributedCacheDecoratorBase<T> : CacheDecoratorBase<T>
    {
        protected readonly IDeserializer<byte[]> _deserializer;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IOptions<DistributedCacheDecoratorOptions> _genericOptions;
        protected readonly ISerializer<byte[]> _serializer;
        protected readonly IOptions<DistributedCacheDecoratorOptions<T>> _specificOptions;

        protected DistributedCacheDecoratorBase(
            IDistributedCache distributedCache,
            ISerializer<byte[]> serializer,
            IDeserializer<byte[]> deserializer,
            IOptions<DistributedCacheDecoratorOptions> genericOptions,
            IOptions<DistributedCacheDecoratorOptions<T>> specificOptions)
        {
            this._distributedCache = distributedCache;
            this._genericOptions = genericOptions;
            this._specificOptions = specificOptions;
            this._serializer = serializer;
            this._deserializer = deserializer;
        }

        protected Task SetToDistributedCacheAsync<TValue>(string cacheKey, TValue value,
            CancellationToken cancellationToken)
        {
            var bytes = this._serializer.Serialize(value);

            return this._distributedCache.SetAsync(cacheKey, bytes, this.CreateEntryOptions(), cancellationToken);
        }

        protected async Task<TResult> GetFromDistributedCacheAsync<TResult>(string cacheKey,
            CancellationToken cancellationToken, Func<Task<TResult>> getFromSource = null)
        {
            var cachedBytes = await this._distributedCache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);

            TResult result;

            if (cachedBytes == null)
            {
                if (getFromSource == null)
                {
                    return default;
                }

                result = await getFromSource().ConfigureAwait(false);

                await this.SetToDistributedCacheAsync(cacheKey, result, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                result = this._deserializer.Deserialize<TResult>(cachedBytes);
            }

            return result;
        }

        protected DistributedCacheEntryOptions CreateEntryOptions() =>
            new()
            {
                AbsoluteExpiration =
                    this._specificOptions.Value.AbsoluteExpiration ?? this._genericOptions.Value.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow =
                    this._specificOptions.Value.AbsoluteExpirationRelativeToNow ??
                    this._genericOptions.Value.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = this._specificOptions.Value.SlidingExpiration ??
                                    this._genericOptions.Value.SlidingExpiration
            };
    }
}
