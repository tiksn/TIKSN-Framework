using System;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Globalization;

public class CountryFactoryTests
{
    [Fact]
    public void GivenAggregateRegion_WhenCountryCreated_ThenItShouldThrow()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act & Assert

        _ = Should.Throw<CountryNotFoundException>(() => countryFactory.Create("001"));
    }

    [Fact]
    public void GivenAggregateRegion_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var created = countryFactory.TryCreate("001", out var country);

        // Assert

        created.ShouldBeFalse();
        country.ShouldBeNull();
    }

    [Fact]
    public void GivenConfiguredOptionsWithoutPrincipalRegion_WhenCountryCreated_ThenItShouldThrow()
    {
        // Arrange

        var countryFactory = CreateCountryFactory(options => options.CountryRegions["DE"] = ["GU"]);

        // Act & Assert

        _ = Should.Throw<ArgumentException>(() => countryFactory.Create("DE"));
    }

    [Fact]
    public void GivenConfiguredOptions_WhenCountryCreated_ThenConfiguredMappingsShouldOverrideDefaults()
    {
        // Arrange

        var countryFactory = CreateCountryFactory(options =>
        {
            options.CountryRegions["US"] = ["US"];
            options.CountryRegions["DE"] = ["DE", "GU"];
        });

        // Act

        var unitedStates = countryFactory.Create("US");
        var germany = countryFactory.Create("GU");

        // Assert

        unitedStates.Regions.Select(x => x.TwoLetterISORegionName).ShouldBe(["US"]);
        germany.Name.ShouldBe("DE");
        germany.Regions.Select(x => x.TwoLetterISORegionName).ShouldBe(["DE", "GU"]);
    }

    [Theory]
    [InlineData("PL", "PL", 1)]
    [InlineData("US", "US", 7)]
    [InlineData("FR", "FR", 12)]
    [InlineData("GB", "GB", 15)]
    [InlineData("NL", "NL", 5)]
    [InlineData("CN", "CN", 3)]
    [InlineData("FI", "FI", 2)]
    [InlineData("NO", "NO", 2)]
    public void GivenCountryCode_WhenCountryCreated_ThenItShouldMatchDefaultMap(
        string input,
        string expectedCountryName,
        int expectedRegionCount)
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var country = countryFactory.Create(input);

        // Assert

        country.Name.ShouldBe(expectedCountryName);
        country.TwoLetterISORegionName.ShouldBe(expectedCountryName);
        country.PrincipalRegion.TwoLetterISORegionName.ShouldBe(expectedCountryName);
        country.Regions.Count.ShouldBe(expectedRegionCount);
        country.ContainsRegion(country.PrincipalRegion).ShouldBeTrue();
    }

    [Fact]
    public void GivenCultureName_WhenCountryCreated_ThenCountryShouldBeReturned()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var country = countryFactory.Create("en-US");

        // Assert

        country.Name.ShouldBe("US");
    }

    [Theory]
    [InlineData("GB", 15, "GB")]
    [InlineData("FR", 12, "FR")]
    [InlineData("US", 7, "US")]
    [InlineData("NL", 5, "NL")]
    [InlineData("AU", 4, "AU")]
    [InlineData("NZ", 4, "NZ")]
    [InlineData("CN", 3, "CN")]
    [InlineData("DK", 3, "DK")]
    [InlineData("FI", 2, "FI")]
    [InlineData("NO", 2, "NO")]
    [InlineData("AD", 1, "AD")]
    [InlineData("AE", 1, "AE")]
    [InlineData("AF", 1, "AF")]
    [InlineData("AG", 1, "AG")]
    [InlineData("AL", 1, "AL")]
    [InlineData("AM", 1, "AM")]
    [InlineData("AO", 1, "AO")]
    [InlineData("AR", 1, "AR")]
    [InlineData("AT", 1, "AT")]
    [InlineData("AZ", 1, "AZ")]
    [InlineData("BA", 1, "BA")]
    [InlineData("BB", 1, "BB")]
    [InlineData("BD", 1, "BD")]
    [InlineData("BE", 1, "BE")]
    [InlineData("BF", 1, "BF")]
    [InlineData("BG", 1, "BG")]
    [InlineData("BH", 1, "BH")]
    [InlineData("BI", 1, "BI")]
    [InlineData("BJ", 1, "BJ")]
    [InlineData("BN", 1, "BN")]
    [InlineData("BO", 1, "BO")]
    [InlineData("BR", 1, "BR")]
    [InlineData("BS", 1, "BS")]
    [InlineData("BT", 1, "BT")]
    [InlineData("BW", 1, "BW")]
    [InlineData("BY", 1, "BY")]
    [InlineData("BZ", 1, "BZ")]
    [InlineData("CA", 1, "CA")]
    [InlineData("CD", 1, "CD")]
    [InlineData("CF", 1, "CF")]
    [InlineData("CG", 1, "CG")]
    [InlineData("CH", 1, "CH")]
    [InlineData("CI", 1, "CI")]
    [InlineData("CL", 1, "CL")]
    [InlineData("CM", 1, "CM")]
    [InlineData("CO", 1, "CO")]
    [InlineData("CR", 1, "CR")]
    [InlineData("CU", 1, "CU")]
    [InlineData("CV", 1, "CV")]
    [InlineData("CY", 1, "CY")]
    [InlineData("CZ", 1, "CZ")]
    [InlineData("DE", 1, "DE")]
    [InlineData("DJ", 1, "DJ")]
    [InlineData("DM", 1, "DM")]
    [InlineData("DO", 1, "DO")]
    [InlineData("DZ", 1, "DZ")]
    [InlineData("EC", 1, "EC")]
    [InlineData("EE", 1, "EE")]
    [InlineData("EG", 1, "EG")]
    [InlineData("ER", 1, "ER")]
    [InlineData("ES", 1, "ES")]
    [InlineData("ET", 1, "ET")]
    [InlineData("FJ", 1, "FJ")]
    [InlineData("FM", 1, "FM")]
    [InlineData("GA", 1, "GA")]
    [InlineData("GD", 1, "GD")]
    [InlineData("GE", 1, "GE")]
    [InlineData("GH", 1, "GH")]
    [InlineData("GM", 1, "GM")]
    [InlineData("GN", 1, "GN")]
    [InlineData("GQ", 1, "GQ")]
    [InlineData("GR", 1, "GR")]
    [InlineData("GT", 1, "GT")]
    [InlineData("GW", 1, "GW")]
    [InlineData("GY", 1, "GY")]
    [InlineData("HN", 1, "HN")]
    [InlineData("HR", 1, "HR")]
    [InlineData("HT", 1, "HT")]
    [InlineData("HU", 1, "HU")]
    [InlineData("ID", 1, "ID")]
    [InlineData("IE", 1, "IE")]
    [InlineData("IL", 1, "IL")]
    [InlineData("IN", 1, "IN")]
    [InlineData("IQ", 1, "IQ")]
    [InlineData("IR", 1, "IR")]
    [InlineData("IS", 1, "IS")]
    [InlineData("IT", 1, "IT")]
    [InlineData("JM", 1, "JM")]
    [InlineData("JO", 1, "JO")]
    [InlineData("JP", 1, "JP")]
    [InlineData("KE", 1, "KE")]
    [InlineData("KG", 1, "KG")]
    [InlineData("KH", 1, "KH")]
    [InlineData("KI", 1, "KI")]
    [InlineData("KM", 1, "KM")]
    [InlineData("KN", 1, "KN")]
    [InlineData("KP", 1, "KP")]
    [InlineData("KR", 1, "KR")]
    [InlineData("KW", 1, "KW")]
    [InlineData("KZ", 1, "KZ")]
    [InlineData("LA", 1, "LA")]
    [InlineData("LB", 1, "LB")]
    [InlineData("LC", 1, "LC")]
    [InlineData("LI", 1, "LI")]
    [InlineData("LK", 1, "LK")]
    [InlineData("LR", 1, "LR")]
    [InlineData("LS", 1, "LS")]
    [InlineData("LT", 1, "LT")]
    [InlineData("LU", 1, "LU")]
    [InlineData("LV", 1, "LV")]
    [InlineData("LY", 1, "LY")]
    [InlineData("MA", 1, "MA")]
    [InlineData("MC", 1, "MC")]
    [InlineData("MD", 1, "MD")]
    [InlineData("ME", 1, "ME")]
    [InlineData("MG", 1, "MG")]
    [InlineData("MH", 1, "MH")]
    [InlineData("MK", 1, "MK")]
    [InlineData("ML", 1, "ML")]
    [InlineData("MM", 1, "MM")]
    [InlineData("MN", 1, "MN")]
    [InlineData("MR", 1, "MR")]
    [InlineData("MT", 1, "MT")]
    [InlineData("MU", 1, "MU")]
    [InlineData("MV", 1, "MV")]
    [InlineData("MW", 1, "MW")]
    [InlineData("MX", 1, "MX")]
    [InlineData("MY", 1, "MY")]
    [InlineData("MZ", 1, "MZ")]
    [InlineData("NA", 1, "NA")]
    [InlineData("NE", 1, "NE")]
    [InlineData("NG", 1, "NG")]
    [InlineData("NI", 1, "NI")]
    [InlineData("NP", 1, "NP")]
    [InlineData("NR", 1, "NR")]
    [InlineData("OM", 1, "OM")]
    [InlineData("PA", 1, "PA")]
    [InlineData("PE", 1, "PE")]
    [InlineData("PG", 1, "PG")]
    [InlineData("PH", 1, "PH")]
    [InlineData("PK", 1, "PK")]
    [InlineData("PL", 1, "PL")]
    [InlineData("PS", 1, "PS")]
    [InlineData("PT", 1, "PT")]
    [InlineData("PW", 1, "PW")]
    [InlineData("PY", 1, "PY")]
    [InlineData("QA", 1, "QA")]
    [InlineData("RO", 1, "RO")]
    [InlineData("RS", 1, "RS")]
    [InlineData("RU", 1, "RU")]
    [InlineData("RW", 1, "RW")]
    [InlineData("SA", 1, "SA")]
    [InlineData("SB", 1, "SB")]
    [InlineData("SC", 1, "SC")]
    [InlineData("SD", 1, "SD")]
    [InlineData("SE", 1, "SE")]
    [InlineData("SG", 1, "SG")]
    [InlineData("SI", 1, "SI")]
    [InlineData("SK", 1, "SK")]
    [InlineData("SL", 1, "SL")]
    [InlineData("SM", 1, "SM")]
    [InlineData("SN", 1, "SN")]
    [InlineData("SO", 1, "SO")]
    [InlineData("SR", 1, "SR")]
    [InlineData("SS", 1, "SS")]
    [InlineData("ST", 1, "ST")]
    [InlineData("SV", 1, "SV")]
    [InlineData("SY", 1, "SY")]
    [InlineData("SZ", 1, "SZ")]
    [InlineData("TD", 1, "TD")]
    [InlineData("TG", 1, "TG")]
    [InlineData("TH", 1, "TH")]
    [InlineData("TJ", 1, "TJ")]
    [InlineData("TL", 1, "TL")]
    [InlineData("TM", 1, "TM")]
    [InlineData("TN", 1, "TN")]
    [InlineData("TO", 1, "TO")]
    [InlineData("TR", 1, "TR")]
    [InlineData("TT", 1, "TT")]
    [InlineData("TV", 1, "TV")]
    [InlineData("TW", 1, "TW")]
    [InlineData("TZ", 1, "TZ")]
    [InlineData("UA", 1, "UA")]
    [InlineData("UG", 1, "UG")]
    [InlineData("UY", 1, "UY")]
    [InlineData("UZ", 1, "UZ")]
    [InlineData("VA", 1, "VA")]
    [InlineData("VC", 1, "VC")]
    [InlineData("VE", 1, "VE")]
    [InlineData("VN", 1, "VN")]
    [InlineData("VU", 1, "VU")]
    [InlineData("WS", 1, "WS")]
    [InlineData("XK", 1, "XK")]
    [InlineData("YE", 1, "YE")]
    [InlineData("ZA", 1, "ZA")]
    [InlineData("ZM", 1, "ZM")]
    [InlineData("ZW", 1, "ZW")]
    public void GivenDefaultMapCountry_WhenCountryCreated_ThenPrincipalRegionAndRegionCountShouldMatch(
        string input,
        int expectedRegionCount,
        string expectedPrincipalRegion)
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var country = countryFactory.Create(input);

        // Assert

        country.PrincipalRegion.TwoLetterISORegionName.ShouldBe(expectedPrincipalRegion);
        country.Regions.Count.ShouldBe(expectedRegionCount);
        country.ContainsRegion(expectedPrincipalRegion).ShouldBeTrue();
    }

    [Fact]
    public void GivenInvalidRegionName_WhenCountryCreated_ThenItShouldThrowCountryNotFoundException()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act & Assert

        _ = Should.Throw<CountryNotFoundException>(() => countryFactory.Create("this is not a region"));
    }

    [Fact]
    public void GivenInvalidRegionName_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var created = countryFactory.TryCreate("this is not a region", out var country);

        // Assert

        created.ShouldBeFalse();
        country.ShouldBeNull();
    }

    [Fact]
    public void GivenMissingConcreteRegion_WhenCountryCreated_ThenItShouldFallbackToSingleRegionCountry()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var country = countryFactory.Create("AQ");

        // Assert

        country.Name.ShouldBe("AQ");
        country.PrincipalRegion.TwoLetterISORegionName.ShouldBe("AQ");
        country.Regions.Single().TwoLetterISORegionName.ShouldBe("AQ");
    }

    [Theory]
    [InlineData("GU", "US")]
    [InlineData("AS", "US")]
    [InlineData("GF", "FR")]
    [InlineData("SX", "NL")]
    [InlineData("HK", "CN")]
    [InlineData("AX", "FI")]
    [InlineData("SJ", "NO")]
    public void GivenRegionCode_WhenCountryCreated_ThenParentCountryShouldBeReturned(
        string input,
        string expectedCountryName)
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var country = countryFactory.Create(input);

        // Assert

        country.Name.ShouldBe(expectedCountryName);
        country.ContainsRegion(input).ShouldBeTrue();
    }

    [Fact]
    public void GivenRegionInfoForTerritory_WhenCountryCreated_ThenParentCountryShouldBeReturned()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var country = countryFactory.Create(new RegionInfo("GU"));

        // Assert

        country.Name.ShouldBe("US");
        country.ContainsRegion("GU").ShouldBeTrue();
    }

    [Fact]
    public void GivenSameCountryAndRegion_WhenCountryCreated_ThenCachedInstanceShouldBeReturned()
    {
        // Arrange

        var countryFactory = CreateCountryFactory();

        // Act

        var fromCountry = countryFactory.Create("US");
        var fromRegion = countryFactory.Create("GU");
        var fromCulture = countryFactory.Create(new RegionInfo("en-US"));

        // Assert

        fromRegion.ShouldBeSameAs(fromCountry);
        fromCulture.ShouldBeSameAs(fromCountry);
    }

    private static ICountryFactory CreateCountryFactory(Action<CountryInfoOptions> configure = null)
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        if (configure != null)
        {
            _ = services.Configure(configure);
        }

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<ICountryFactory>();
    }
}
