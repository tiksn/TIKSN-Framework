using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using static LanguageExt.Prelude;

namespace TIKSN.Data.Cache.Memory;

public abstract class MemoryCacheDecoratorBase<T> : CacheDecoratorBase<T>
{
    protected MemoryCacheDecoratorBase(
        IMemoryCache memoryCache,
        IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<T>> specificOptions)
    {
        this.MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        this.GenericOptions = genericOptions ?? throw new ArgumentNullException(nameof(genericOptions));
        this.SpecificOptions = specificOptions ?? throw new ArgumentNullException(nameof(specificOptions));
    }

    protected IOptions<MemoryCacheDecoratorOptions> GenericOptions { get; }
    protected IMemoryCache MemoryCache { get; }
    protected IOptions<MemoryCacheDecoratorOptions<T>> SpecificOptions { get; }

    protected TResult CreateMemoryCacheItem<TResult>(
        ICacheEntry entry,
        Func<TResult> getFromSource)
    {
        ArgumentNullException.ThrowIfNull(getFromSource);

        this.SpecifyOptions(entry);

        return getFromSource();
    }

    protected Task<TResult> CreateMemoryCacheItemAsync<TResult>(
        ICacheEntry cacheEntry,
        Func<Task<TResult>> getFromSource)
    {
        ArgumentNullException.ThrowIfNull(getFromSource);

        this.SpecifyOptions(cacheEntry);

        return getFromSource();
    }

    protected Option<TResult> FindFromMemoryCache<TResult>(
        object cacheKey,
        Func<Option<TResult>> findFromSource)
    {
        ArgumentNullException.ThrowIfNull(findFromSource);

        if (!this.MemoryCache.TryGetValue(cacheKey, out TResult? result))
        {
            _ = findFromSource().IfSome(foundItem =>
            {
                using var entry = this.MemoryCache.CreateEntry(cacheKey);

                this.SpecifyOptions(entry);

                result = foundItem;
                entry.Value = result;
            });
        }

        return Optional(result);
    }

    protected async Task<Option<TResult>> FindFromMemoryCacheAsync<TResult>(
        object cacheKey,
        Func<Task<Option<TResult>>> findFromSourceAsync)
    {
        ArgumentNullException.ThrowIfNull(findFromSourceAsync);

        if (!this.MemoryCache.TryGetValue(cacheKey, out TResult? result))
        {
            var findings = await findFromSourceAsync().ConfigureAwait(false);
            _ = await findings.IfSomeAsync(foundItem =>
            {
                using var entry = this.MemoryCache.CreateEntry(cacheKey);

                this.SpecifyOptions(entry);

                result = foundItem;
                entry.Value = result;
            }).ConfigureAwait(false);
        }

        return Optional(result);
    }

    protected TResult GetFromMemoryCache<TResult>(
            object cacheKey,
        Func<TResult> getFromSource) =>
        this.MemoryCache.GetOrCreate(cacheKey, x => this.CreateMemoryCacheItem(x, getFromSource));

    protected Task<TResult> GetFromMemoryCacheAsync<TResult>(
        object cacheKey,
        Func<Task<TResult>> getFromSource) =>
        this.MemoryCache.GetOrCreateAsync(cacheKey, x => this.CreateMemoryCacheItemAsync(x, getFromSource));

    protected void SpecifyOptions(ICacheEntry cacheEntry)
    {
        ArgumentNullException.ThrowIfNull(cacheEntry);

        cacheEntry.AbsoluteExpiration = this.SpecificOptions.Value.AbsoluteExpiration ??
                                        this.GenericOptions.Value.AbsoluteExpiration;
        cacheEntry.AbsoluteExpirationRelativeToNow = this.SpecificOptions.Value.AbsoluteExpirationRelativeToNow ??
                                                     this.GenericOptions.Value.AbsoluteExpirationRelativeToNow;
        cacheEntry.SlidingExpiration = this.SpecificOptions.Value.SlidingExpiration ??
                                       this.GenericOptions.Value.SlidingExpiration;
    }
}
