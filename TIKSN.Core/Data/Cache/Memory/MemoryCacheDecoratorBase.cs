using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

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

		protected T CreateMemoryCacheItem(ICacheEntry entry, Func<T> getFromSource)
		{
			SpecifyOptions(entry);

			return getFromSource();
		}

		protected Task<T> CreateMemoryCacheItemAsync(ICacheEntry cacheEntry, Func<Task<T>> getFromSource)
		{
			SpecifyOptions(cacheEntry);

			return getFromSource();
		}

		protected T GetFromMemoryCache(object cacheKey, Func<T> getFromSource)
		{
			return _memoryCache.GetOrCreate(cacheKey, x => CreateMemoryCacheItem(x, getFromSource));
		}

		protected Task<T> GetFromMemoryCacheAsync(object cacheKey, Func<Task<T>> getFromSource)
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
