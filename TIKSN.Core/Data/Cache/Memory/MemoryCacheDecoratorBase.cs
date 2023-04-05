using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.Cache.Memory
{
    public abstract class MemoryCacheDecoratorBase<T> : CacheDecoratorBase<T>
    {
        protected readonly IOptions<MemoryCacheDecoratorOptions> genericOptions;
        protected readonly IMemoryCache memoryCache;
        protected readonly IOptions<MemoryCacheDecoratorOptions<T>> specificOptions;

        protected MemoryCacheDecoratorBase(IMemoryCache memoryCache,
            IOptions<MemoryCacheDecoratorOptions> genericOptions,
            IOptions<MemoryCacheDecoratorOptions<T>> specificOptions)
        {
            this.memoryCache = memoryCache;
            this.genericOptions = genericOptions;
            this.specificOptions = specificOptions;
        }

        protected TResult CreateMemoryCacheItem<TResult>(ICacheEntry entry, Func<TResult> getFromSource)
        {
            this.SpecifyOptions(entry);

            return getFromSource();
        }

        protected Task<TResult> CreateMemoryCacheItemAsync<TResult>(ICacheEntry cacheEntry,
            Func<Task<TResult>> getFromSource)
        {
            this.SpecifyOptions(cacheEntry);

            return getFromSource();
        }

        protected TResult GetFromMemoryCache<TResult>(object cacheKey, Func<TResult> getFromSource) =>
            this.memoryCache.GetOrCreate(cacheKey, x => this.CreateMemoryCacheItem(x, getFromSource));

        protected Task<TResult> GetFromMemoryCacheAsync<TResult>(object cacheKey, Func<Task<TResult>> getFromSource) =>
            this.memoryCache.GetOrCreateAsync(cacheKey, x => this.CreateMemoryCacheItemAsync(x, getFromSource));

        protected void SpecifyOptions(ICacheEntry cacheEntry)
        {
            cacheEntry.AbsoluteExpiration = this.specificOptions.Value.AbsoluteExpiration ??
                                            this.genericOptions.Value.AbsoluteExpiration;
            cacheEntry.AbsoluteExpirationRelativeToNow = this.specificOptions.Value.AbsoluteExpirationRelativeToNow ??
                                                         this.genericOptions.Value.AbsoluteExpirationRelativeToNow;
            cacheEntry.SlidingExpiration = this.specificOptions.Value.SlidingExpiration ??
                                           this.genericOptions.Value.SlidingExpiration;
        }
    }
}
