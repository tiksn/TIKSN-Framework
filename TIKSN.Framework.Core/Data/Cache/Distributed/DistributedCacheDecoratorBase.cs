using LanguageExt;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;
using static LanguageExt.Prelude;

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

    protected async Task<Option<TResult>> FindFromDistributedCacheAsync<TResult>(
        string cacheKey,
        Func<Task<Option<TResult>>> findFromSource,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);

        ArgumentNullException.ThrowIfNull(findFromSource);

        var cachedBytes = await this.distributedCache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);

        if (cachedBytes == null)
        {
            var findings = await findFromSource().ConfigureAwait(false);

            _ = await findings
                .IfSomeAsync(async foundItem => await this.SetToDistributedCacheAsync(cacheKey, foundItem, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

            return findings;
        }

        return Optional(this.deserializer.Deserialize<TResult>(cachedBytes));
    }

    protected async Task<TResult> GetFromDistributedCacheAsync<TResult>(
        string cacheKey,
        Func<Task<TResult>> getFromSource,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);

        ArgumentNullException.ThrowIfNull(getFromSource);

        var cachedBytes = await this.distributedCache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);

        if (cachedBytes == null)
        {
            var result = await getFromSource().ConfigureAwait(false);

            await this.SetToDistributedCacheAsync(cacheKey, result, cancellationToken).ConfigureAwait(false);

            return result;
        }

        return this.deserializer.Deserialize<TResult>(cachedBytes);
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
