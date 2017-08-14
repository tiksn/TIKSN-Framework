using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace TIKSN.Data.Cache
{
	public class DistributedTypedCache : IDistributedTypedCache
	{
		public T Get<T>(string key)
		{
			throw new NotImplementedException();
		}

		public Task<T> GetAsync<T>(string key)
		{
			throw new NotImplementedException();
		}

		public void Set<T>(string key, T value, DistributedCacheEntryOptions options)
		{
			throw new NotImplementedException();
		}

		public Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
