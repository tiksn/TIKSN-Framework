using Microsoft.Extensions.Caching.Hybrid;

namespace TIKSN.Data.Cache.Hybrid;

public class HybridCacheDecoratorOptions
{
    public HybridCacheEntryOptions? EntryOptions { get; set; }
}

#pragma warning disable S2094 // Classes should not be empty
#pragma warning disable S2326 // Unused type parameters should be removed
public class HybridCacheDecoratorOptions<T> : HybridCacheDecoratorOptions;
#pragma warning restore S2326 // Unused type parameters should be removed
#pragma warning restore S2094 // Classes should not be empty
