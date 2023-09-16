using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;

namespace TIKSN.Data.Cache.Distributed;

public abstract class DistributedCacheDecoratorBase<T> : CacheDecoratorBase<T>
{
    protected readonly IDeserializer<byte[]> deserializer;
    protected readonly IDistributedCache distributedCache;
    protected readonly IOptions<DistributedCacheDecoratorOptions> genericOptions;
    protected readonly ISerializer<byte[]> serializer;
    protected readonly IOptions<DistributedCacheDecoratorOptions<T>> specificOptions;

    protected DistributedCacheDecoratorBase(
        IDistributedCache distributedCache,
        ISerializer<byte[]> serializer,
        IDeserializer<byte[]> deserializer,
        IOptions<DistributedCacheDecoratorOptions> genericOptions,
        IOptions<DistributedCacheDecoratorOptions<T>> specificOptions)
    {
        this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        this.genericOptions = genericOptions ?? throw new ArgumentNullException(nameof(genericOptions));
        this.specificOptions = specificOptions ?? throw new ArgumentNullException(nameof(specificOptions));
        this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
    }

    protected DistributedCacheEntryOptions CreateEntryOptions() =>
        new()
        {
            AbsoluteExpiration =
                this.specificOptions.Value.AbsoluteExpiration ?? this.genericOptions.Value.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow =
                this.specificOptions.Value.AbsoluteExpirationRelativeToNow ??
                this.genericOptions.Value.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = this.specificOptions.Value.SlidingExpiration ??
                                this.genericOptions.Value.SlidingExpiration,
        };

    protected async Task<TResult> GetFromDistributedCacheAsync<TResult>(
        string cacheKey,
        CancellationToken cancellationToken,
        Func<Task<TResult>> getFromSource = null)
    {
        var cachedBytes = await this.distributedCache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);

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
            result = this.deserializer.Deserialize<TResult>(cachedBytes);
        }

        return result;
    }

    protected Task SetToDistributedCacheAsync<TValue>(
        string cacheKey,
        TValue value,
        CancellationToken cancellationToken)
    {
        var bytes = this.serializer.Serialize(value);

        return this.distributedCache.SetAsync(cacheKey, bytes, this.CreateEntryOptions(), cancellationToken);
    }
}
