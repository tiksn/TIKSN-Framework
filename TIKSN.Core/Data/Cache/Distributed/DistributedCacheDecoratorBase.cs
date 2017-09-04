using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using TIKSN.Serialization;
using System.Threading;

namespace TIKSN.Data.Cache.Distributed
{
	public abstract class DistributedCacheDecoratorBase<T> : CacheDecoratorBase<T>
	{
		protected readonly IDistributedCache _distributedCache;
		protected readonly IOptions<DistributedCacheDecoratorOptions> _genericOptions;
		protected readonly IOptions<DistributedCacheDecoratorOptions<T>> _specificOptions;
		protected readonly ISerializer<byte[]> _serializer;
		protected readonly IDeserializer<byte[]> _deserializer;

		protected DistributedCacheDecoratorBase(IDistributedCache distributedCache, ISerializer<byte[]> serializer, IDeserializer<byte[]> deserializer, IOptions<DistributedCacheDecoratorOptions> genericOptions, IOptions<DistributedCacheDecoratorOptions<T>> specificOptions)
		{
			_distributedCache = distributedCache;
			_genericOptions = genericOptions;
			_specificOptions = specificOptions;
			_serializer = serializer;
			_deserializer = deserializer;
		}

		protected Task SetToDistributedCacheAsync<TValue>(string cacheKey, TValue value, CancellationToken cancellationToken)
		{
			var bytes = _serializer.Serialize(value);

			return _distributedCache.SetAsync(cacheKey, bytes, CreateEntryOptions(), cancellationToken);
		}

		protected async Task<TResult> GetFromDistributedCacheAsync<TResult>(string cacheKey, CancellationToken cancellationToken, Func<Task<TResult>> getFromSource = null)
		{
			var cachedBytes = _distributedCache.Get(cacheKey);

			TResult result;

			if (cachedBytes == null)
			{
				if (getFromSource == null) return default;

				result = await getFromSource();

				await SetToDistributedCacheAsync(cacheKey, result, cancellationToken);
			}
			else
			{
				result = _deserializer.Deserialize<TResult>(cachedBytes);
			}

			return result;
		}

		protected DistributedCacheEntryOptions CreateEntryOptions()
		{
			return new DistributedCacheEntryOptions
			{
				AbsoluteExpiration = _specificOptions.Value.AbsoluteExpiration ?? _genericOptions.Value.AbsoluteExpiration,
				AbsoluteExpirationRelativeToNow = _specificOptions.Value.AbsoluteExpirationRelativeToNow ?? _genericOptions.Value.AbsoluteExpirationRelativeToNow,
				SlidingExpiration = _specificOptions.Value.SlidingExpiration ?? _genericOptions.Value.SlidingExpiration
			};
		}
	}
}
