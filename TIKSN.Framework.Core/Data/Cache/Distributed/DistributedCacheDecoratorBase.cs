using LanguageExt;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TIKSN.Serialization;
using static LanguageExt.Prelude;

namespace TIKSN.Data.Cache.Distributed;

public abstract class DistributedCacheDecoratorBase<T> : CacheDecoratorBase<T>
{
    protected DistributedCacheDecoratorBase(
        IDistributedCache distributedCache,
        ISerializer<byte[]> serializer,
        IDeserializer<byte[]> deserializer,
        IOptions<DistributedCacheDecoratorOptions> genericOptions,
        IOptions<DistributedCacheDecoratorOptions<T>> specificOptions)
    {
        this.DistributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        this.GenericOptions = genericOptions ?? throw new ArgumentNullException(nameof(genericOptions));
        this.SpecificOptions = specificOptions ?? throw new ArgumentNullException(nameof(specificOptions));
        this.Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        this.Deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
    }

    protected IDeserializer<byte[]> Deserializer { get; }
    protected IDistributedCache DistributedCache { get; }
    protected IOptions<DistributedCacheDecoratorOptions> GenericOptions { get; }
    protected ISerializer<byte[]> Serializer { get; }
    protected IOptions<DistributedCacheDecoratorOptions<T>> SpecificOptions { get; }

    protected DistributedCacheEntryOptions CreateEntryOptions() =>
        new()
        {
            AbsoluteExpiration =
                this.SpecificOptions.Value.AbsoluteExpiration ?? this.GenericOptions.Value.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow =
                this.SpecificOptions.Value.AbsoluteExpirationRelativeToNow ??
                this.GenericOptions.Value.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = this.SpecificOptions.Value.SlidingExpiration ??
                                this.GenericOptions.Value.SlidingExpiration,
        };

    protected async Task<Option<TResult>> FindFromDistributedCacheAsync<TResult>(
        string cacheKey,
        Func<Task<Option<TResult>>> findFromSource,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);

        ArgumentNullException.ThrowIfNull(findFromSource);

        var cachedBytes = await this.DistributedCache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);

        if (cachedBytes == null)
        {
            var findings = await findFromSource().ConfigureAwait(false);

            _ = await findings
                .IfSomeAsync(async foundItem => await this.SetToDistributedCacheAsync(cacheKey, foundItem, cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

            return findings;
        }

        return Optional(this.Deserializer.Deserialize<TResult>(cachedBytes));
    }

    protected async Task<TResult?> GetFromDistributedCacheAsync<TResult>(
        string cacheKey,
        Func<Task<TResult?>> getFromSource,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);

        ArgumentNullException.ThrowIfNull(getFromSource);

        var cachedBytes = await this.DistributedCache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);

        if (cachedBytes == null)
        {
            var result = await getFromSource().ConfigureAwait(false);

            if (result is not null)
            {
                await this.SetToDistributedCacheAsync(cacheKey, result, cancellationToken).ConfigureAwait(false);

                return result;
            }
        }
        else
        {
            return this.Deserializer.Deserialize<TResult>(cachedBytes);
        }

        return default;
    }

    protected Task SetToDistributedCacheAsync<TValue>(
        string cacheKey,
        TValue value,
        CancellationToken cancellationToken)
    {
        var bytes = this.Serializer.Serialize(value);

        return this.DistributedCache.SetAsync(cacheKey, bytes, this.CreateEntryOptions(), cancellationToken);
    }
}
