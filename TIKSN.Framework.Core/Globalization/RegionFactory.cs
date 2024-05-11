using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Memory;

namespace TIKSN.Globalization;

public class RegionFactory : MemoryCacheDecoratorBase<RegionInfo>, IRegionFactory
{
    public RegionFactory(IMemoryCache memoryCache, IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<RegionInfo>> specificOptions) : base(memoryCache, genericOptions,
        specificOptions)
    {
    }

    public RegionInfo Create(string name)
    {
        var cacheKey = Tuple.Create(EntityType, name);

        return this.GetFromMemoryCache(cacheKey, () => new RegionInfo(name))
            ?? throw new InvalidOperationException("Failed to create RegionInfo.");
    }
}
