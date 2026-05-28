using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Globalization;

public class TimeZoneInfoLookupTests
{
    [Fact]
    public void GivenCountry_WhenTimeZonesListed_ThenAllRegionalZonesShouldBeReturned()
    {
        // Arrange

        var serviceProvider = CreateServiceProvider();
        var countryFactory = serviceProvider.GetRequiredService<ICountryFactory>();
        var lookup = serviceProvider.GetRequiredService<ITimeZoneInfoLookup>();
        var unitedStates = countryFactory.Create("US");

        // Act

        var timeZoneIds = lookup.ListCountryTimeZones(unitedStates).Select(ToIanaId).ToArray();

        // Assert

        timeZoneIds.ShouldContain("America/New_York");
        timeZoneIds.ShouldContain("America/Puerto_Rico");
        timeZoneIds.ShouldContain("Pacific/Guam");
    }

    [Fact]
    public void GivenKnownRegions_WhenTimeZonesListed_ThenRegionalZonesShouldBeReturned()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<ITimeZoneInfoLookup>();

        // Act

        var unitedStatesZones = lookup.ListRegionalTimeZones(new RegionInfo("US")).Select(ToIanaId).ToArray();
        var armeniaZones = lookup.ListRegionalTimeZones(new RegionInfo("AM")).Select(ToIanaId).ToArray();

        // Assert

        unitedStatesZones.ShouldContain("America/New_York");
        armeniaZones.ShouldContain("Asia/Yerevan");
    }

    [Fact]
    public void GivenNullInputs_WhenLookupCalled_ThenItShouldThrow()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<ITimeZoneInfoLookup>();
        RegionInfo nullRegion = null;
        CountryInfo nullCountry = null;
        TimeZoneInfo nullTimeZone = null;

        // Act & Assert

        _ = Should.Throw<ArgumentNullException>(() => lookup.ListRegionalTimeZones(nullRegion));
        _ = Should.Throw<ArgumentNullException>(() => lookup.ListCountryTimeZones(nullCountry));
        _ = Should.Throw<ArgumentNullException>(() => lookup.ResolveTimeZoneRegion(nullTimeZone));
        _ = Should.Throw<ArgumentNullException>(() => lookup.ResolveTimeZoneCountry(nullTimeZone));
        _ = Should.Throw<ArgumentNullException>(() => lookup.TryResolveTimeZoneRegion(nullTimeZone, out _));
        _ = Should.Throw<ArgumentNullException>(() => lookup.TryResolveTimeZoneCountry(nullTimeZone, out _));
    }

    [Fact]
    public void GivenTerritoryTimeZoneInfo_WhenResolved_ThenTerritoryRegionAndParentCountryShouldBeReturned()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<ITimeZoneInfoLookup>();
        var timeZone = ResolveTimeZoneInfo("Pacific/Guam");

        // Act

        var region = lookup.ResolveTimeZoneRegion(timeZone);
        var country = lookup.ResolveTimeZoneCountry(timeZone);

        // Assert

        region.TwoLetterISORegionName.ShouldBe("GU");
        country.Name.ShouldBe("US");
    }

    [Fact]
    public void GivenTimeZoneInfo_WhenResolved_ThenRegionAndCountryShouldBeReturned()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<ITimeZoneInfoLookup>();
        var timeZone = ResolveTimeZoneInfo("Asia/Yerevan");

        // Act

        var region = lookup.ResolveTimeZoneRegion(timeZone);
        var country = lookup.ResolveTimeZoneCountry(timeZone);

        // Assert

        region.TwoLetterISORegionName.ShouldBe("AM");
        country.Name.ShouldBe("AM");
    }

    [Fact]
    public void GivenUtcTimeZoneInfo_WhenResolved_ThenRequiredLookupShouldThrow()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<ITimeZoneInfoLookup>();

        // Act & Assert

        _ = Should.Throw<DateTimeZoneLookupException>(() => lookup.ResolveTimeZoneRegion(TimeZoneInfo.Utc));
        _ = Should.Throw<DateTimeZoneLookupException>(() => lookup.ResolveTimeZoneCountry(TimeZoneInfo.Utc));
    }

    [Fact]
    public void GivenUtcTimeZoneInfo_WhenTryResolved_ThenItShouldReturnFalse()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<ITimeZoneInfoLookup>();

        // Act

        var regionResolved = lookup.TryResolveTimeZoneRegion(TimeZoneInfo.Utc, out var region);
        var countryResolved = lookup.TryResolveTimeZoneCountry(TimeZoneInfo.Utc, out var country);

        // Assert

        regionResolved.ShouldBeFalse();
        region.ShouldBeNull();
        countryResolved.ShouldBeFalse();
        country.ShouldBeNull();
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        return services.BuildServiceProvider();
    }

    private static TimeZoneInfo ResolveTimeZoneInfo(string ianaId)
    {
        if (TimeZoneInfo.TryFindSystemTimeZoneById(ianaId, out var timeZoneInfo))
        {
            return timeZoneInfo;
        }

        if (TimeZoneInfo.TryConvertIanaIdToWindowsId(ianaId, out var windowsId) &&
            TimeZoneInfo.TryFindSystemTimeZoneById(windowsId, out timeZoneInfo))
        {
            return timeZoneInfo;
        }

        throw new InvalidOperationException($"TZDB time zone '{ianaId}' cannot be resolved to TimeZoneInfo.");
    }

    private static string ToIanaId(TimeZoneInfo timeZoneInfo)
    {
        if (DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneInfo.Id) is not null)
        {
            return timeZoneInfo.Id;
        }

        if (TimeZoneInfo.TryConvertWindowsIdToIanaId(timeZoneInfo.Id, out var ianaId))
        {
            return ianaId;
        }

        throw new InvalidOperationException($"TimeZoneInfo '{timeZoneInfo.Id}' cannot be resolved to an IANA ID.");
    }
}
