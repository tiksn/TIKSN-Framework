using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.Cache.Hybrid;

public abstract class HybridCacheDecoratorBase<T> : CacheDecoratorBase<T>
{
    protected HybridCacheDecoratorBase(
        HybridCache hybridCache,
        IOptions<HybridCacheDecoratorOptions> genericOptions,
        IOptions<HybridCacheDecoratorOptions<T>> specificOptions)
    {
        this.HybridCache = hybridCache ?? throw new ArgumentNullException(nameof(hybridCache));
        this.GenericOptions = genericOptions ?? throw new ArgumentNullException(nameof(genericOptions));
        this.SpecificOptions = specificOptions ?? throw new ArgumentNullException(nameof(specificOptions));
    }

    protected IOptions<HybridCacheDecoratorOptions> GenericOptions { get; }
    protected HybridCache HybridCache { get; }
    protected IOptions<HybridCacheDecoratorOptions<T>> SpecificOptions { get; }

    protected HybridCacheEntryOptions? CreateEntryOptions() =>
        this.SpecificOptions.Value.EntryOptions ?? this.GenericOptions.Value.EntryOptions;

    protected ValueTask<TResult> GetFromHybridCacheAsync<TResult>(
        string cacheKey,
        Func<CancellationToken, ValueTask<TResult>> getFromSource,
        CancellationToken cancellationToken)
        => this.HybridCache.GetOrCreateAsync(
            cacheKey,
            getFromSource,
            this.CreateEntryOptions(),
            cancellationToken: cancellationToken);
}
