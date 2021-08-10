using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.Cache.Memory
{
    public abstract class MemoryCacheDecoratorBase<T> : CacheDecoratorBase<T>
    {
        protected readonly IOptions<MemoryCacheDecoratorOptions> _genericOptions;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IOptions<MemoryCacheDecoratorOptions<T>> _specificOptions;

        protected MemoryCacheDecoratorBase(IMemoryCache memoryCache,
            IOptions<MemoryCacheDecoratorOptions> genericOptions,
            IOptions<MemoryCacheDecoratorOptions<T>> specificOptions)
        {
            this._memoryCache = memoryCache;
            this._genericOptions = genericOptions;
            this._specificOptions = specificOptions;
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
            this._memoryCache.GetOrCreate(cacheKey, x => this.CreateMemoryCacheItem(x, getFromSource));

        protected Task<TResult> GetFromMemoryCacheAsync<TResult>(object cacheKey, Func<Task<TResult>> getFromSource) =>
            this._memoryCache.GetOrCreateAsync(cacheKey, x => this.CreateMemoryCacheItemAsync(x, getFromSource));

        protected void SpecifyOptions(ICacheEntry cacheEntry)
        {
            cacheEntry.AbsoluteExpiration = this._specificOptions.Value.AbsoluteExpiration ??
                                            this._genericOptions.Value.AbsoluteExpiration;
            cacheEntry.AbsoluteExpirationRelativeToNow = this._specificOptions.Value.AbsoluteExpirationRelativeToNow ??
                                                         this._genericOptions.Value.AbsoluteExpirationRelativeToNow;
            cacheEntry.SlidingExpiration = this._specificOptions.Value.SlidingExpiration ??
                                           this._genericOptions.Value.SlidingExpiration;
        }
    }
}
