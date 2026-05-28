using System;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Globalization;

public class DateTimeZoneLookupTests
{
    [Fact]
    public void GivenAliasTimeZone_WhenResolved_ThenCanonicalRegionShouldBeReturned()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<IDateTimeZoneLookup>();
        var timeZone = DateTimeZoneProviders.Tzdb["US/Eastern"];

        // Act

        var region = lookup.ResolveTimeZoneRegion(timeZone);
        var country = lookup.ResolveTimeZoneCountry(timeZone);

        // Assert

        region.TwoLetterISORegionName.ShouldBe("US");
        country.Name.ShouldBe("US");
    }

    [Fact]
    public void GivenCanonicalTimeZone_WhenResolved_ThenRegionAndCountryShouldBeReturned()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<IDateTimeZoneLookup>();
        var timeZone = DateTimeZoneProviders.Tzdb["Asia/Yerevan"];

        // Act

        var region = lookup.ResolveTimeZoneRegion(timeZone);
        var country = lookup.ResolveTimeZoneCountry(timeZone);

        // Assert

        region.TwoLetterISORegionName.ShouldBe("AM");
        country.Name.ShouldBe("AM");
    }

    [Fact]
    public void GivenCountry_WhenTimeZonesListed_ThenAllRegionalZonesShouldBeReturned()
    {
        // Arrange

        var serviceProvider = CreateServiceProvider();
        var countryFactory = serviceProvider.GetRequiredService<ICountryFactory>();
        var lookup = serviceProvider.GetRequiredService<IDateTimeZoneLookup>();
        var unitedStates = countryFactory.Create("US");

        // Act

        var timeZoneIds = lookup.ListCountryTimeZones(unitedStates).Select(x => x.Id).ToArray();

        // Assert

        timeZoneIds.ShouldContain("America/New_York");
        timeZoneIds.ShouldContain("America/Puerto_Rico");
        timeZoneIds.ShouldContain("Pacific/Guam");
    }

    [Fact]
    public void GivenCountry_WhenTimeZonesListed_ThenResultShouldBeDistinctAndOrdered()
    {
        // Arrange

        var serviceProvider = CreateServiceProvider();
        var countryFactory = serviceProvider.GetRequiredService<ICountryFactory>();
        var lookup = serviceProvider.GetRequiredService<IDateTimeZoneLookup>();
        var unitedStates = countryFactory.Create("US");

        // Act

        var timeZoneIds = lookup.ListCountryTimeZones(unitedStates).Select(x => x.Id).ToArray();

        // Assert

        timeZoneIds.Length.ShouldBe(timeZoneIds.Distinct(StringComparer.Ordinal).Count());
        timeZoneIds.ShouldBe([.. timeZoneIds.OrderBy(x => x, StringComparer.Ordinal)]);
    }

    [Fact]
    public void GivenKnownRegions_WhenTimeZonesListed_ThenRegionalZonesShouldBeReturned()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<IDateTimeZoneLookup>();

        // Act

        var unitedStatesZones = lookup.ListRegionalTimeZones(new RegionInfo("US")).Select(x => x.Id).ToArray();
        var armeniaZones = lookup.ListRegionalTimeZones(new RegionInfo("AM")).Select(x => x.Id).ToArray();

        // Assert

        unitedStatesZones.ShouldContain("America/New_York");
        armeniaZones.ShouldContain("Asia/Yerevan");
    }

    [Fact]
    public void GivenNullInputs_WhenLookupCalled_ThenItShouldThrow()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<IDateTimeZoneLookup>();
        RegionInfo nullRegion = null;
        CountryInfo nullCountry = null;
        DateTimeZone nullTimeZone = null;

        // Act & Assert

        _ = Should.Throw<ArgumentNullException>(() => lookup.ListRegionalTimeZones(nullRegion));
        _ = Should.Throw<ArgumentNullException>(() => lookup.ListCountryTimeZones(nullCountry));
        _ = Should.Throw<ArgumentNullException>(() => lookup.ResolveTimeZoneRegion(nullTimeZone));
        _ = Should.Throw<ArgumentNullException>(() => lookup.ResolveTimeZoneCountry(nullTimeZone));
        _ = Should.Throw<ArgumentNullException>(() => lookup.TryResolveTimeZoneRegion(nullTimeZone, out _));
        _ = Should.Throw<ArgumentNullException>(() => lookup.TryResolveTimeZoneCountry(nullTimeZone, out _));
    }

    [Fact]
    public void GivenTerritoryTimeZone_WhenResolved_ThenTerritoryRegionAndParentCountryShouldBeReturned()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<IDateTimeZoneLookup>();
        var timeZone = DateTimeZoneProviders.Tzdb["Pacific/Guam"];

        // Act

        var region = lookup.ResolveTimeZoneRegion(timeZone);
        var country = lookup.ResolveTimeZoneCountry(timeZone);

        // Assert

        region.TwoLetterISORegionName.ShouldBe("GU");
        country.Name.ShouldBe("US");
    }

    [Fact]
    public void GivenUtcTimeZone_WhenResolved_ThenRequiredLookupShouldThrow()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<IDateTimeZoneLookup>();

        // Act & Assert

        _ = Should.Throw<DateTimeZoneLookupException>(() => lookup.ResolveTimeZoneRegion(DateTimeZone.Utc));
        _ = Should.Throw<DateTimeZoneLookupException>(() => lookup.ResolveTimeZoneCountry(DateTimeZone.Utc));
    }

    [Fact]
    public void GivenUtcTimeZone_WhenTryResolved_ThenItShouldReturnFalse()
    {
        // Arrange

        var lookup = CreateServiceProvider().GetRequiredService<IDateTimeZoneLookup>();

        // Act

        var regionResolved = lookup.TryResolveTimeZoneRegion(DateTimeZone.Utc, out var region);
        var countryResolved = lookup.TryResolveTimeZoneCountry(DateTimeZone.Utc, out var country);

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
}
