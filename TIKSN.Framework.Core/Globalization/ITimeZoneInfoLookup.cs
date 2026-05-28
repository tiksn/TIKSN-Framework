using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LanguageExt;

namespace TIKSN.Globalization;

public interface ITimeZoneInfoLookup
{
    public Seq<TimeZoneInfo> ListRegionalTimeZones(RegionInfo region);

    public Seq<TimeZoneInfo> ListCountryTimeZones(CountryInfo country);

    public RegionInfo ResolveTimeZoneRegion(TimeZoneInfo timeZone);

    public CountryInfo ResolveTimeZoneCountry(TimeZoneInfo timeZone);

    public bool TryResolveTimeZoneRegion(TimeZoneInfo timeZone, [NotNullWhen(true)] out RegionInfo? region);

    public bool TryResolveTimeZoneCountry(TimeZoneInfo timeZone, [NotNullWhen(true)] out CountryInfo? country);
}
