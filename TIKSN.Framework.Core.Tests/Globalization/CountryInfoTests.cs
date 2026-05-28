using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Globalization;

public class CountryInfoTests
{
    [Fact]
    public void GivenCountry_WhenComparedToDifferentObject_ThenItShouldNotBeEqual()
    {
        // Arrange

        var country = CreateCountryFactory().Create("US");

        // Act & Assert

        country.Equals(new object()).ShouldBeFalse();
    }

    [Fact]
    public void GivenCountry_WhenComparedToNull_ThenItShouldNotBeEqual()
    {
        // Arrange

        var country = CreateCountryFactory().Create("US");
        CountryInfo nullCountry = null;

        // Act & Assert

        country.Equals(nullCountry).ShouldBeFalse();
        (country == nullCountry).ShouldBeFalse();
        (country != nullCountry).ShouldBeTrue();
    }

    [Fact]
    public void GivenCountry_WhenPropertiesRead_ThenItShouldExposeCountryInformation()
    {
        // Arrange

        var country = CreateCountryFactory().Create("US");

        // Act & Assert

        country.Name.ShouldBe("US");
        country.TwoLetterISORegionName.ShouldBe("US");
        country.PrincipalRegion.TwoLetterISORegionName.ShouldBe("US");
        country.Regions.Select(x => x.TwoLetterISORegionName).ShouldBe(["AS", "GU", "MP", "PR", "UM", "US", "VI"]);
        country.ToString().ShouldBe("US");
    }

    [Fact]
    public void GivenCountry_WhenRegionChecked_ThenMembershipShouldBeEvaluated()
    {
        // Arrange

        var country = CreateCountryFactory().Create("US");

        // Act & Assert

        country.ContainsRegion("US").ShouldBeTrue();
        country.ContainsRegion("en-US").ShouldBeTrue();
        country.ContainsRegion(new RegionInfo("GU")).ShouldBeTrue();
        country.ContainsRegion("PL").ShouldBeFalse();
    }

    [Fact]
    public void GivenCountry_WhenRegionsRead_ThenCollectionShouldBeImmutable()
    {
        // Arrange

        var country = CreateCountryFactory().Create("US");

        // Act & Assert

        _ = country.Regions.ShouldBeAssignableTo<System.Collections.ObjectModel.ReadOnlyCollection<RegionInfo>>();
    }

    [Fact]
    public void GivenDifferentCountryName_WhenCompared_ThenCountriesShouldNotBeEqual()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();
        var unitedStates = countryFactory.Create("US");
        var poland = countryFactory.Create("PL");

        // Act & Assert

        unitedStates.Equals(poland).ShouldBeFalse();
        (unitedStates != poland).ShouldBeTrue();
        (unitedStates == poland).ShouldBeFalse();
    }

    [Fact]
    public void GivenSameCountryName_WhenCompared_ThenCountriesShouldBeEqual()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();
        var unitedStates1 = countryFactory.Create("US");
        var unitedStates2 = countryFactory.Create(new RegionInfo("en-US"));

        // Act & Assert

        unitedStates1.Equals(unitedStates2).ShouldBeTrue();
        (unitedStates1 == unitedStates2).ShouldBeTrue();
        (unitedStates1 != unitedStates2).ShouldBeFalse();
        unitedStates1.GetHashCode().ShouldBe(unitedStates2.GetHashCode());
    }

    private static ICountryFactory CreateCountryFactory()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<ICountryFactory>();
    }
}
