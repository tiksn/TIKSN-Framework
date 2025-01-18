using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public void GivenConfiguredOptions_WhenCurrencyInfoCreated_ThenItShouldMatchToOutput(string inputIsoCurrencySymbol, string outputIsoCurrencySymbol)
    {
        // Arrange

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "CurrencyUnionRedirections:XZZ", "uk-UA"}
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

        _ = currencyInfo.Should().NotBeNull();
        _ = currencyInfo.ISOCurrencySymbol.Should().Be(outputIsoCurrencySymbol);
    }

    [Theory]
    [InlineData("001", "USD")]
    [InlineData("150", "EUR")]
    [InlineData("029", "XCD")]
    [InlineData("419", "USD")]
    [InlineData("XK", "UAH")]
    public void GivenConfiguredOptions_WhenRegionCurrencyInfoCreated_ThenItShouldMatchToOutput(string inputRegionName, string outputIsoCurrencySymbol)
    {
        // Arrange

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
            { "RegionalCurrencyRedirections:XK", "uk-UA"}
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

        _ = currencyInfo.Should().NotBeNull();
        _ = currencyInfo.ISOCurrencySymbol.Should().Be(outputIsoCurrencySymbol);
    }

    [Theory]
    [InlineData("GGP", "GBP")]
    [InlineData("JEP", "GBP")]
    [InlineData("IMP", "GBP")]
    [InlineData("FKP", "GBP")]
    [InlineData("GIP", "GBP")]
    [InlineData("SHP", "GBP")]
    public void GivenInputIsoCurrencySymbol_WhenCurrencyInfoCreated_ThenItShouldMatchToOutput(string inputIsoCurrencySymbol, string outputIsoCurrencySymbol)
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var currencyInfo = currencyFactory.Create(inputIsoCurrencySymbol);

        // Assert

        _ = currencyInfo.Should().NotBeNull();
        _ = currencyInfo.ISOCurrencySymbol.Should().Be(outputIsoCurrencySymbol);
    }

    [Theory]
    [InlineData("001", "USD")]
    [InlineData("150", "EUR")]
    [InlineData("029", "XCD")]
    [InlineData("419", "USD")]
    public void GivenInputIsoCurrencySymbol_WhenRegionCurrencyInfoCreated_ThenItShouldMatchToOutput(string inputRegionName, string outputIsoCurrencySymbol)
    {
        // Arrange

        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();

        // Act

        var currencyInfo = currencyFactory.Create(new RegionInfo(inputRegionName));

        // Assert

        _ = currencyInfo.Should().NotBeNull();
        _ = currencyInfo.ISOCurrencySymbol.Should().Be(outputIsoCurrencySymbol);
    }
}
