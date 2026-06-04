using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TIKSN.Data.Cache.Memory;

namespace TIKSN.Globalization;

public class CountryFactory : MemoryCacheDecoratorBase<CountryInfo>, ICountryFactory
{
    private readonly IOptions<CountryInfoOptions> countryInfoOptions;
    private readonly IRegionFactory regionFactory;

    public CountryFactory(
        IMemoryCache memoryCache,
        IRegionFactory regionFactory,
        IOptions<CountryInfoOptions> countryInfoOptions,
        IOptions<MemoryCacheDecoratorOptions> genericOptions,
        IOptions<MemoryCacheDecoratorOptions<CountryInfo>> specificOptions) : base(memoryCache, genericOptions,
        specificOptions)
    {
        this.regionFactory = regionFactory ?? throw new ArgumentNullException(nameof(regionFactory));
        this.countryInfoOptions = countryInfoOptions ?? throw new ArgumentNullException(nameof(countryInfoOptions));
    }

    private static EqualityComparer<RegionInfo> RegionInfoEqualityComparer => EqualityComparer<RegionInfo>.Create(
        (x, y) => string.Equals(x?.TwoLetterISORegionName, y?.TwoLetterISORegionName, StringComparison.Ordinal),
        x => x.TwoLetterISORegionName.GetHashCode(StringComparison.Ordinal));

    public CountryInfo Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new CountryNotFoundException("Country name cannot be null or whitespace.");
        }

        RegionInfo region;
        try
        {
            region = this.regionFactory.Create(name);
        }
        catch (ArgumentException ex)
        {
            throw new CountryNotFoundException($"Country '{name}' was not found.", ex);
        }

        return this.Create(region);
    }

    public CountryInfo Create(RegionInfo region)
    {
        if (region is null)
        {
            throw new CountryNotFoundException("Country region cannot be null.");
        }

        var countryName = region.TwoLetterISORegionName;
        var cacheKey = Tuple.Create(EntityType, countryName);

        return this.GetFromMemoryCache(cacheKey, () => this.GetOrCreateCountry(region))
            ?? throw new InvalidOperationException("Failed to create CountryInfo.");
    }

    public bool TryCreate(string name, [NotNullWhen(true)] out CountryInfo? country)
    {
        try
        {
            country = this.Create(name);
            return true;
        }
        catch (CountryNotFoundException)
        {
            country = null;
            return false;
        }
    }

    public bool TryCreate(RegionInfo region, [NotNullWhen(true)] out CountryInfo? country)
    {
        try
        {
            country = this.Create(region);
            return true;
        }
        catch (CountryNotFoundException)
        {
            country = null;
            return false;
        }
    }

    private static void ValidateConcreteRegion(RegionInfo region)
    {
        if (region.TwoLetterISORegionName.Length != 2)
        {
            throw new CountryNotFoundException($"Region '{region.Name}' is not a concrete two-letter region.");
        }
    }

    private static void ValidateCountryRegionMapping(RegionInfo principalRegion, HashSet<RegionInfo> regions)
    {
        ValidateConcreteRegion(principalRegion);

        if (regions.Count == 0)
        {
            throw new ArgumentException(
                $"Country '{principalRegion.TwoLetterISORegionName}' must contain at least one region.",
                nameof(regions));
        }

        foreach (var region in regions)
        {
            ValidateConcreteRegion(region);
        }

        if (!regions.Any(r => r.Equals(principalRegion)))
        {
            throw new ArgumentException(
                $"Country '{principalRegion.TwoLetterISORegionName}' must include its principal region.",
                nameof(regions));
        }
    }

    private CountryInfo CreateCountryInfo(RegionInfo region)
    {
        var principalRegion = region;

        var regionsByPrincipalRegion = this.GetRegionsByPrincipalRegion();
        if (!regionsByPrincipalRegion.TryGetValue(principalRegion, out var regions))
        {
            regions = new HashSet<RegionInfo>([region]);
        }

        ValidateCountryRegionMapping(principalRegion, regions);

        return new CountryInfo(principalRegion.TwoLetterISORegionName, principalRegion, regions);
    }

    private CountryInfo GetOrCreateCountry(RegionInfo region)
    {
        var principalRegionByRegion = this.GetPrincipalRegionByRegion();
        if (principalRegionByRegion.TryGetValue(region, out var principalRegion))
        {
            var cacheKey = Tuple.Create(EntityType, principalRegion.TwoLetterISORegionName);

            return this.GetFromMemoryCache(cacheKey, () => this.CreateCountryInfo(principalRegion))
                ?? throw new InvalidOperationException("Failed to create CountryInfo.");
        }

        return this.CreateCountryInfo(region);
    }

    private Dictionary<RegionInfo, RegionInfo> GetPrincipalRegionByRegion()
    {
        var principalRegionByRegion = new Dictionary<RegionInfo, RegionInfo>(RegionInfoEqualityComparer);

        foreach (var configuredCountry in this.countryInfoOptions.Value.CountryRegions)
        {
            var principalRegion = this.regionFactory.Create(configuredCountry.Key);
            foreach (var regionCode in configuredCountry.Value)
            {
                var region = this.regionFactory.Create(regionCode);
                principalRegionByRegion.Add(region, principalRegion);
            }
        }

        return principalRegionByRegion;
    }

    private Dictionary<RegionInfo, HashSet<RegionInfo>> GetRegionsByPrincipalRegion()
    {
        var regionsByPrincipalRegion = new Dictionary<RegionInfo, HashSet<RegionInfo>>(RegionInfoEqualityComparer);

        foreach (var configuredCountry in this.countryInfoOptions.Value.CountryRegions)
        {
            var principalRegion = this.regionFactory.Create(configuredCountry.Key);
            regionsByPrincipalRegion.Add(principalRegion,
                configuredCountry.Value.Select(this.regionFactory.Create).ToHashSet(RegionInfoEqualityComparer));
        }

        return regionsByPrincipalRegion;
    }
}
