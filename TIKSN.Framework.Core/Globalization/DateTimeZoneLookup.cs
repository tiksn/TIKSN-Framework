using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LanguageExt;
using NodaTime;
using NodaTime.TimeZones;

namespace TIKSN.Globalization;

public class DateTimeZoneLookup : IDateTimeZoneLookup
{
    private static readonly TzdbDateTimeZoneSource Source = TzdbDateTimeZoneSource.Default;

    private readonly ICountryFactory countryFactory;
    private readonly FrozenDictionary<string, Seq<DateTimeZone>> regionCodeToZones;
    private readonly IRegionFactory regionFactory;
    private readonly FrozenDictionary<string, string[]> zoneIdToRegionCodes;

    public DateTimeZoneLookup(IRegionFactory regionFactory, ICountryFactory countryFactory)
    {
        this.regionFactory = regionFactory ?? throw new ArgumentNullException(nameof(regionFactory));
        this.countryFactory = countryFactory ?? throw new ArgumentNullException(nameof(countryFactory));
        this.regionCodeToZones = BuildRegionCodeToZones();
        this.zoneIdToRegionCodes = BuildZoneIdToRegionCodes();
    }

    public Seq<DateTimeZone> ListCountryTimeZones(CountryInfo country)
    {
        ArgumentNullException.ThrowIfNull(country);

        return country.Regions
            .SelectMany(region => this.ListRegionalTimeZones(region))
            .DistinctBy(timeZone => timeZone.Id)
            .OrderBy(timeZone => timeZone.Id, StringComparer.Ordinal)
            .ToSeq();
    }

    public Seq<DateTimeZone> ListRegionalTimeZones(RegionInfo region)
    {
        ArgumentNullException.ThrowIfNull(region);

        return this.regionCodeToZones.TryGetValue(region.TwoLetterISORegionName, out var zones)
            ? zones
            : Enumerable.Empty<DateTimeZone>().ToSeq();
    }

    public CountryInfo ResolveTimeZoneCountry(DateTimeZone timeZone)
    {
        var region = this.ResolveTimeZoneRegion(timeZone);

        return this.countryFactory.Create(region);
    }

    public RegionInfo ResolveTimeZoneRegion(DateTimeZone timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        var canonicalZoneId = NormalizeZoneId(timeZone.Id);

        if (!this.zoneIdToRegionCodes.TryGetValue(canonicalZoneId, out var regionCodes))
        {
            throw new DateTimeZoneLookupException($"Time zone '{timeZone.Id}' does not have a regional TZDB location.");
        }

        if (regionCodes.Length != 1)
        {
            throw new DateTimeZoneLookupException(
                $"Time zone '{timeZone.Id}' resolves to multiple regions: {string.Join(", ", regionCodes)}.");
        }

        return this.regionFactory.Create(regionCodes[0]);
    }

    public bool TryResolveTimeZoneCountry(DateTimeZone timeZone, [NotNullWhen(true)] out CountryInfo? country)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        try
        {
            country = this.ResolveTimeZoneCountry(timeZone);
            return true;
        }
        catch (ArgumentException)
        {
            country = null;
            return false;
        }
        catch (CountryNotFoundException)
        {
            country = null;
            return false;
        }
        catch (DateTimeZoneLookupException)
        {
            country = null;
            return false;
        }
    }

    public bool TryResolveTimeZoneRegion(DateTimeZone timeZone, [NotNullWhen(true)] out RegionInfo? region)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        try
        {
            region = this.ResolveTimeZoneRegion(timeZone);
            return true;
        }
        catch (ArgumentException)
        {
            region = null;
            return false;
        }
        catch (DateTimeZoneLookupException)
        {
            region = null;
            return false;
        }
    }

    private static FrozenDictionary<string, Seq<DateTimeZone>> BuildRegionCodeToZones() => GetZoneLocations()
            .GroupBy(location => NormalizeRegionCode(location.CountryCode), StringComparer.Ordinal)
            .ToFrozenDictionary(
                group => group.Key,
                group => group
                    .Select(location => DateTimeZoneProviders.Tzdb[NormalizeZoneId(location.ZoneId)])
                    .DistinctBy(timeZone => timeZone.Id)
                    .OrderBy(timeZone => timeZone.Id, StringComparer.Ordinal)
                    .ToSeq(),
                StringComparer.Ordinal);

    private static FrozenDictionary<string, string[]> BuildZoneIdToRegionCodes() => GetZoneLocations()
            .Where(location =>
                string.Equals(location.ZoneId, NormalizeZoneId(location.ZoneId), StringComparison.Ordinal))
            .GroupBy(location => location.ZoneId, StringComparer.Ordinal)
            .ToFrozenDictionary(
                group => group.Key,
                group => group
                    .Select(location => NormalizeRegionCode(location.CountryCode))
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(regionCode => regionCode, StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.Ordinal);

    private static IEnumerable<TzdbZoneLocation> GetZoneLocations() =>
        Source.ZoneLocations ?? Enumerable.Empty<TzdbZoneLocation>();

    private static string NormalizeRegionCode(string regionCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(regionCode);

        return regionCode.Trim().ToUpperInvariant();
    }

    private static string NormalizeZoneId(string zoneId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(zoneId);

        return Source.CanonicalIdMap.TryGetValue(zoneId, out var canonicalZoneId)
            ? canonicalZoneId
            : zoneId;
    }
}
