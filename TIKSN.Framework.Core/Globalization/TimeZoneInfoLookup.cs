using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LanguageExt;
using NodaTime;

namespace TIKSN.Globalization;

public class TimeZoneInfoLookup : ITimeZoneInfoLookup
{
    private readonly IDateTimeZoneLookup dateTimeZoneLookup;

    public TimeZoneInfoLookup(IDateTimeZoneLookup dateTimeZoneLookup)
        => this.dateTimeZoneLookup = dateTimeZoneLookup ?? throw new ArgumentNullException(nameof(dateTimeZoneLookup));

    public Seq<TimeZoneInfo> ListCountryTimeZones(CountryInfo country)
    {
        ArgumentNullException.ThrowIfNull(country);

        return this.dateTimeZoneLookup.ListCountryTimeZones(country)
            .Select(ResolveTimeZoneInfo)
            .DistinctBy(timeZone => timeZone.Id)
            .OrderBy(timeZone => timeZone.Id, StringComparer.Ordinal)
            .ToSeq();
    }

    public Seq<TimeZoneInfo> ListRegionalTimeZones(RegionInfo region)
    {
        ArgumentNullException.ThrowIfNull(region);

        return this.dateTimeZoneLookup.ListRegionalTimeZones(region)
            .Select(ResolveTimeZoneInfo)
            .DistinctBy(timeZone => timeZone.Id)
            .OrderBy(timeZone => timeZone.Id, StringComparer.Ordinal)
            .ToSeq();
    }

    public CountryInfo ResolveTimeZoneCountry(TimeZoneInfo timeZone)
    {
        var dateTimeZone = ResolveDateTimeZone(timeZone);

        return this.dateTimeZoneLookup.ResolveTimeZoneCountry(dateTimeZone);
    }

    public RegionInfo ResolveTimeZoneRegion(TimeZoneInfo timeZone)
    {
        var dateTimeZone = ResolveDateTimeZone(timeZone);

        return this.dateTimeZoneLookup.ResolveTimeZoneRegion(dateTimeZone);
    }

    public bool TryResolveTimeZoneCountry(TimeZoneInfo timeZone, [NotNullWhen(true)] out CountryInfo? country)
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

    public bool TryResolveTimeZoneRegion(TimeZoneInfo timeZone, [NotNullWhen(true)] out RegionInfo? region)
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

    private static DateTimeZone ResolveDateTimeZone(TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        var dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZone.Id);
        if (dateTimeZone is not null)
        {
            return dateTimeZone;
        }

        if (TimeZoneInfo.TryConvertWindowsIdToIanaId(timeZone.Id, out var ianaId))
        {
            dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(ianaId);
            if (dateTimeZone is not null)
            {
                return dateTimeZone;
            }
        }

        throw new DateTimeZoneLookupException($"Time zone '{timeZone.Id}' cannot be resolved to a TZDB time zone.");
    }

    private static TimeZoneInfo ResolveTimeZoneInfo(DateTimeZone dateTimeZone)
    {
        ArgumentNullException.ThrowIfNull(dateTimeZone);

        if (TimeZoneInfo.TryFindSystemTimeZoneById(dateTimeZone.Id, out var timeZoneInfo))
        {
            return timeZoneInfo;
        }

        if (TimeZoneInfo.TryConvertIanaIdToWindowsId(dateTimeZone.Id, out var windowsId) &&
            TimeZoneInfo.TryFindSystemTimeZoneById(windowsId, out timeZoneInfo))
        {
            return timeZoneInfo;
        }

        throw new DateTimeZoneLookupException(
            $"TZDB time zone '{dateTimeZone.Id}' cannot be resolved to a system TimeZoneInfo.");
    }
}
