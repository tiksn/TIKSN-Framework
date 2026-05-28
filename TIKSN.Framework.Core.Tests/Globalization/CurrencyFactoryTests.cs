using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Globalization;

public class CurrencyFactoryTests
{
    [Theory]
    [InlineData("GGP", "GBP")]
    [InlineData("JEP", "GBP")]
    [InlineData("IMP", "GBP")]
    [InlineData("FKP", "GBP")]
    [InlineData("GIP", "GBP")]
    [InlineData("SHP", "GBP")]
    [InlineData("XZZ", "UAH")]
    public void GivenConfiguredOptions_WhenCurrencyInfoCreated_ThenItShouldMatchToOutput(
        string inputIsoCurrencySymbol, string outputIsoCurrencySymbol)
    {
        // Arrange

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "CurrencyUnionRedirections:XZZ", "uk-UA" },
            })
            .Build();
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = services.Configure<CurrencyUnionRedirectionOptions>(configurationRoot);
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var currencyInfo = currencyFactory.Create(inputIsoCurrencySymbol);

        // Assert

        _ = currencyInfo.ShouldNotBeNull();
        currencyInfo.ISOCurrencySymbol.ShouldBe(outputIsoCurrencySymbol);
    }

    [Theory]
    [InlineData("001", "USD")]
    [InlineData("150", "EUR")]
    [InlineData("029", "XCD")]
    [InlineData("419", "USD")]
    [InlineData("XK", "UAH")]
    public void GivenConfiguredOptions_WhenRegionCurrencyInfoCreated_ThenItShouldMatchToOutput(
        string inputRegionName, string outputIsoCurrencySymbol)
    {
        // Arrange

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RegionalCurrencyRedirections:XK", "uk-UA" },
            })
            .Build();
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = services.Configure<RegionalCurrencyRedirectionOptions>(configurationRoot);
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var currencyInfo = currencyFactory.Create(new RegionInfo(inputRegionName));

        // Assert

        _ = currencyInfo.ShouldNotBeNull();
        currencyInfo.ISOCurrencySymbol.ShouldBe(outputIsoCurrencySymbol);
    }

    [Theory]
    [InlineData("GGP", "GBP")]
    [InlineData("JEP", "GBP")]
    [InlineData("IMP", "GBP")]
    [InlineData("FKP", "GBP")]
    [InlineData("GIP", "GBP")]
    [InlineData("SHP", "GBP")]
    public void GivenInputIsoCurrencySymbol_WhenCurrencyInfoCreated_ThenItShouldMatchToOutput(
        string inputIsoCurrencySymbol, string outputIsoCurrencySymbol)
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var currencyInfo = currencyFactory.Create(inputIsoCurrencySymbol);

        // Assert

        _ = currencyInfo.ShouldNotBeNull();
        currencyInfo.ISOCurrencySymbol.ShouldBe(outputIsoCurrencySymbol);
    }

    [Theory]
    [InlineData("001", "USD")]
    [InlineData("150", "EUR")]
    [InlineData("029", "XCD")]
    [InlineData("419", "USD")]
    public void GivenInputIsoCurrencySymbol_WhenRegionCurrencyInfoCreated_ThenItShouldMatchToOutput(
        string inputRegionName, string outputIsoCurrencySymbol)
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var currencyInfo = currencyFactory.Create(new RegionInfo(inputRegionName));

        // Assert

        _ = currencyInfo.ShouldNotBeNull();
        currencyInfo.ISOCurrencySymbol.ShouldBe(outputIsoCurrencySymbol);
    }

    [Fact]
    public void GivenInvalidIsoCurrencySymbol_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var created = currencyFactory.TryCreate("ZZZ", out var currency);

        // Assert

        created.ShouldBeFalse();
        currency.ShouldBeNull();
    }

    [Fact]
    public void GivenNullIsoCurrencySymbol_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        string isoCurrencySymbol = null;

        // Act

        var created = currencyFactory.TryCreate(isoCurrencySymbol, out var currency);

        // Assert

        created.ShouldBeFalse();
        currency.ShouldBeNull();
    }

    [Fact]
    public void GivenNullRegion_WhenTryCreateCalled_ThenItShouldReturnFalse()
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        RegionInfo region = null;

        // Act

        var created = currencyFactory.TryCreate(region, out var currency);

        // Assert

        created.ShouldBeFalse();
        currency.ShouldBeNull();
    }

    [Fact]
    public void GivenValidIsoCurrencySymbol_WhenTryCreateCalled_ThenItShouldReturnCurrency()
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var created = currencyFactory.TryCreate("USD", out var currency);

        // Assert

        created.ShouldBeTrue();
        _ = currency.ShouldNotBeNull();
        currency.ISOCurrencySymbol.ShouldBe("USD");
    }

    [Fact]
    public void GivenValidRegion_WhenTryCreateCalled_ThenItShouldReturnCurrency()
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var created = currencyFactory.TryCreate(new RegionInfo("US"), out var currency);

        // Assert

        created.ShouldBeTrue();
        _ = currency.ShouldNotBeNull();
        currency.ISOCurrencySymbol.ShouldBe("USD");
    }
}
