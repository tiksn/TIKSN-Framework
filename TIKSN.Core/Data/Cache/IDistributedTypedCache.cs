using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace TIKSN.Data.Cache
{
	public interface IDistributedTypedCache
    {
		T Get<T>(string key);
		Task<T> GetAsync<T>(string key);
		void Set<T>(string key, T value, DistributedCacheEntryOptions options);
		Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options);
	}
}
