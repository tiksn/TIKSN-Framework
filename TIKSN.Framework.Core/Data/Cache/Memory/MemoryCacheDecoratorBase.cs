using LanguageExt;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using static LanguageExt.Prelude;

namespace TIKSN.Data.Cache.Memory;

public abstract class MemoryCacheDecoratorBase<T> : CacheDecoratorBase<T>
{
    protected readonly IOptions<MemoryCacheDecoratorOptions> genericOptions;
    protected readonly IMemoryCache memoryCache;
    protected readonly IOptions<MemoryCacheDecoratorOptions<T>> specificOptions;

    protected MemoryCacheDecoratorBase(
        IMemoryCache memoryCache,
        IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<T>> specificOptions)
    {
        this.memoryCache = memoryCache;
        this.genericOptions = genericOptions;
        this.specificOptions = specificOptions;
    }

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

        if (!this.memoryCache.TryGetValue(cacheKey, out TResult? result))
        {
            _ = findFromSource().IfSome(foundItem =>
            {
                using var entry = this.memoryCache.CreateEntry(cacheKey);

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

        if (!this.memoryCache.TryGetValue(cacheKey, out TResult? result))
        {
            var findings = await findFromSourceAsync().ConfigureAwait(false);
            _ = await findings.IfSomeAsync(foundItem =>
            {
                using var entry = this.memoryCache.CreateEntry(cacheKey);

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
        this.memoryCache.GetOrCreate(cacheKey, x => this.CreateMemoryCacheItem(x, getFromSource));

    protected Task<TResult> GetFromMemoryCacheAsync<TResult>(
        object cacheKey,
        Func<Task<TResult>> getFromSource) =>
        this.memoryCache.GetOrCreateAsync(cacheKey, x => this.CreateMemoryCacheItemAsync(x, getFromSource));

    protected void SpecifyOptions(ICacheEntry cacheEntry)
    {
        ArgumentNullException.ThrowIfNull(cacheEntry);

        cacheEntry.AbsoluteExpiration = this.specificOptions.Value.AbsoluteExpiration ??
                                        this.genericOptions.Value.AbsoluteExpiration;
        cacheEntry.AbsoluteExpirationRelativeToNow = this.specificOptions.Value.AbsoluteExpirationRelativeToNow ??
                                                     this.genericOptions.Value.AbsoluteExpirationRelativeToNow;
        cacheEntry.SlidingExpiration = this.specificOptions.Value.SlidingExpiration ??
                                       this.genericOptions.Value.SlidingExpiration;
    }
}
