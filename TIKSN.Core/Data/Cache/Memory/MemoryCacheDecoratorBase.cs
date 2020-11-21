using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace TIKSN.Data.Cache.Memory
{
    public abstract class MemoryCacheDecoratorBase<T> : CacheDecoratorBase<T>
    {
        protected readonly IMemoryCache _memoryCache;
        protected readonly IOptions<MemoryCacheDecoratorOptions> _genericOptions;
        protected readonly IOptions<MemoryCacheDecoratorOptions<T>> _specificOptions;

        protected MemoryCacheDecoratorBase(IMemoryCache memoryCache, IOptions<MemoryCacheDecoratorOptions> genericOptions, IOptions<MemoryCacheDecoratorOptions<T>> specificOptions)
        {
            _memoryCache = memoryCache;
            _genericOptions = genericOptions;
            _specificOptions = specificOptions;
        }

        protected TResult CreateMemoryCacheItem<TResult>(ICacheEntry entry, Func<TResult> getFromSource)
        {
            SpecifyOptions(entry);

            return getFromSource();
        }

        protected Task<TResult> CreateMemoryCacheItemAsync<TResult>(ICacheEntry cacheEntry, Func<Task<TResult>> getFromSource)
        {
            SpecifyOptions(cacheEntry);

            return getFromSource();
        }

        protected TResult GetFromMemoryCache<TResult>(object cacheKey, Func<TResult> getFromSource)
        {
            return _memoryCache.GetOrCreate(cacheKey, x => CreateMemoryCacheItem(x, getFromSource));
        }

        protected Task<TResult> GetFromMemoryCacheAsync<TResult>(object cacheKey, Func<Task<TResult>> getFromSource)
        {
            return _memoryCache.GetOrCreateAsync(cacheKey, x => CreateMemoryCacheItemAsync(x, getFromSource));
        }

        protected void SpecifyOptions(ICacheEntry cacheEntry)
        {
            cacheEntry.AbsoluteExpiration = _specificOptions.Value.AbsoluteExpiration ?? _genericOptions.Value.AbsoluteExpiration;
            cacheEntry.AbsoluteExpirationRelativeToNow = _specificOptions.Value.AbsoluteExpirationRelativeToNow ?? _genericOptions.Value.AbsoluteExpirationRelativeToNow;
            cacheEntry.SlidingExpiration = _specificOptions.Value.SlidingExpiration ?? _genericOptions.Value.SlidingExpiration;
        }
    }
}