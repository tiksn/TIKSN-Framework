using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Memory;

namespace TIKSN.Globalization;

public class CultureFactory : MemoryCacheDecoratorBase<CultureInfo>, ICultureFactory
{
    public CultureFactory(IMemoryCache memoryCache, IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<CultureInfo>> specificOptions) : base(memoryCache, genericOptions,
        specificOptions)
    {
    }

    public CultureInfo Create(string name)
    {
        var cacheKey = Tuple.Create(entityType, name);

        return this.GetFromMemoryCache(cacheKey, () => new CultureInfo(name));
    }
}
