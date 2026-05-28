using System.Diagnostics.CodeAnalysis;
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
        var cacheKey = Tuple.Create(EntityType, name);

        return this.GetFromMemoryCache(cacheKey, () => new CultureInfo(name))
            ?? throw new InvalidOperationException("Failed to create CultureInfo.");
    }

    public bool TryCreate(string name, [NotNullWhen(true)] out CultureInfo? culture)
    {
        try
        {
            culture = this.Create(name);
            return true;
        }
        catch (ArgumentException)
        {
            culture = null;
            return false;
        }
    }
}
