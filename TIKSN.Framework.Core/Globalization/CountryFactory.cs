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

        var countryName = this.ResolveCountryName(region);
        var cacheKey = Tuple.Create(EntityType, countryName);

        return this.GetFromMemoryCache(cacheKey, () => this.CreateCountry(countryName))
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

    private static string NormalizeCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        return code.Trim().ToUpperInvariant();
    }

    private static string[] ParseRegionNames(IEnumerable<string> regionNames)
    {
        if (regionNames is null)
        {
            throw new ArgumentNullException(nameof(regionNames), "Country regions cannot be null.");
        }

        return
        [
            .. regionNames
                .Select(NormalizeCode)
                .Distinct(StringComparer.Ordinal)
        ];
    }

    private static void ValidateConfiguredConcreteRegionCode(string code)
    {
        if (code.Length != 2)
        {
            throw new ArgumentException($"Region '{code}' is not a concrete two-letter region.", nameof(code));
        }

        RegionInfo region;
        try
        {
            region = new RegionInfo(code);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Region '{code}' is not a concrete two-letter region.", nameof(code), ex);
        }

        if (!string.Equals(region.TwoLetterISORegionName, code, StringComparison.Ordinal))
        {
            throw new ArgumentException($"Region '{code}' is not a concrete two-letter region.", nameof(code));
        }
    }

    private static void ValidateCountryRegionMapping(string countryName, string[] regionNames)
    {
        ValidateConfiguredConcreteRegionCode(countryName);

        if (regionNames.Length == 0)
        {
            throw new ArgumentException($"Country '{countryName}' must contain at least one region.",
                nameof(regionNames));
        }

        foreach (var regionName in regionNames)
        {
            ValidateConfiguredConcreteRegionCode(regionName);
        }

        if (!regionNames.Contains(countryName, StringComparer.Ordinal))
        {
            throw new ArgumentException(
                $"Country '{countryName}' must include its principal region.",
                nameof(regionNames));
        }
    }

    private CountryInfo CreateCountry(string countryName)
    {
        var countryRegions = this.GetCountryRegions();
        if (!countryRegions.TryGetValue(countryName, out var regionNames))
        {
            regionNames = [countryName];
        }

        ValidateCountryRegionMapping(countryName, regionNames);

        var principalRegion = this.regionFactory.Create(countryName);
        var regions = regionNames.Select(this.regionFactory.Create).ToArray();

        return new CountryInfo(countryName, principalRegion, regions);
    }

    private Dictionary<string, string[]> GetCountryRegions()
    {
        var countryRegions = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var configuredCountryRegions in this.countryInfoOptions.Value.CountryRegions)
        {
            countryRegions[NormalizeCode(configuredCountryRegions.Key)] =
                ParseRegionNames(configuredCountryRegions.Value);
        }

        return countryRegions;
    }

    private string ResolveCountryName(RegionInfo region)
    {
        var regionName = region.TwoLetterISORegionName;
        if (regionName.Length != 2)
        {
            throw new CountryNotFoundException($"Region '{region.Name}' is not a concrete two-letter region.");
        }

        var countryRegions = this.GetCountryRegions();
        if (countryRegions.ContainsKey(regionName))
        {
            return regionName;
        }

        var containingCountries = countryRegions
            .Where(x => x.Value.Contains(regionName, StringComparer.Ordinal))
            .Select(x => x.Key)
            .ToArray();

        if (containingCountries.Length > 1)
        {
            throw new CountryNotFoundException($"Region '{region.Name}' is configured for multiple countries.");
        }

        return containingCountries.Length == 1 ? containingCountries[0] : regionName;
    }
}
