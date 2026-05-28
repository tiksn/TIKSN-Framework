using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LanguageExt;
using NodaTime;

namespace TIKSN.Globalization;

public interface IDateTimeZoneLookup
{
    public Seq<DateTimeZone> ListRegionalTimeZones(RegionInfo region);

    public Seq<DateTimeZone> ListCountryTimeZones(CountryInfo country);

    public RegionInfo ResolveTimeZoneRegion(DateTimeZone timeZone);

    public CountryInfo ResolveTimeZoneCountry(DateTimeZone timeZone);

    public bool TryResolveTimeZoneRegion(DateTimeZone timeZone, [NotNullWhen(true)] out RegionInfo? region);

    public bool TryResolveTimeZoneCountry(DateTimeZone timeZone, [NotNullWhen(true)] out CountryInfo? country);
}
