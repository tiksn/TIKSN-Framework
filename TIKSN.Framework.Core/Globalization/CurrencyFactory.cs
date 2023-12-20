using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Memory;
using TIKSN.Finance;

namespace TIKSN.Globalization;

public class CurrencyFactory : MemoryCacheDecoratorBase<CurrencyInfo>, ICurrencyFactory
{
    private readonly IOptions<CurrencyUnionRedirectionOptions> currencyUnionRedirectionOptions;
    private readonly IOptions<RegionalCurrencyRedirectionOptions> regionalCurrencyRedirectionOptions;
    private readonly IRegionFactory regionFactory;

    public CurrencyFactory(
        IMemoryCache memoryCache,
        IRegionFactory regionFactory,
        IOptions<RegionalCurrencyRedirectionOptions> regionalCurrencyRedirectionOptions,
        IOptions<CurrencyUnionRedirectionOptions> currencyUnionRedirectionOptions,
        IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<CurrencyInfo>> specificOptions) : base(memoryCache, genericOptions,
        specificOptions)
    {
        this.regionFactory = regionFactory;
        this.regionalCurrencyRedirectionOptions = regionalCurrencyRedirectionOptions;
        this.currencyUnionRedirectionOptions = currencyUnionRedirectionOptions;
    }

    public CurrencyInfo Create(string isoCurrencySymbol)
    {
        if (string.IsNullOrWhiteSpace(isoCurrencySymbol))
        {
            throw new ArgumentException($"'{nameof(isoCurrencySymbol)}' cannot be null or whitespace.", nameof(isoCurrencySymbol));
        }

        if (this.currencyUnionRedirectionOptions.Value.CurrencyUnionRedirections.TryGetValue(isoCurrencySymbol,
            out var redirectedRegion))
        {
            return this.Create(this.regionFactory.Create(redirectedRegion));
        }

        var cacheKey = Tuple.Create(EntityType, isoCurrencySymbol.ToUpperInvariant());

        return this.GetFromMemoryCache(cacheKey, () => new CurrencyInfo(isoCurrencySymbol));
    }

    public CurrencyInfo Create(RegionInfo region)
    {
        ArgumentNullException.ThrowIfNull(region);

        if (this.regionalCurrencyRedirectionOptions.Value.RegionalCurrencyRedirections.TryGetValue(region.Name, out var regionByName))
        {
            region = this.regionFactory.Create(regionByName);
        }
        else if (this.regionalCurrencyRedirectionOptions.Value.RegionalCurrencyRedirections.TryGetValue(region.TwoLetterISORegionName, out var regionByISOName))
        {
            region = this.regionFactory.Create(regionByISOName);
        }

        var cacheKey = Tuple.Create(EntityType, region.ISOCurrencySymbol);

        return this.GetFromMemoryCache(cacheKey, () => new CurrencyInfo(region));
    }
}
