using System;
using System.Threading.Tasks;
using FluentAssertions;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange;
using TIKSN.Globalization;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

[Collection("ServiceProviderCollection")]
public class ExchangeRateServiceBaseTests
{
    private readonly ServiceProviderFixture serviceProviderFixture;

    public ExchangeRateServiceBaseTests(
        ServiceProviderFixture serviceProviderFixture)
        => this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));

    [Theory]
    [InlineData("LiteDB")]
    [InlineData("SQLite")]
    [InlineData("MongoDB")]
    public async Task Given_10USD_When_ExchangedForEuro_Then_ResultShouldBeEuro(string database)
    {
        // Arrange
        var exchangeRateService = this.serviceProviderFixture.GetServiceProvider(database).GetRequiredService<IExchangeRateService>();
        var currencyFactory = this.serviceProviderFixture.GetServiceProvider(database).GetRequiredService<ICurrencyFactory>();
        var usd = currencyFactory.Create("USD");
        var eur = currencyFactory.Create("EUR");
        var usd10 = new Money(usd, 10m);

        await exchangeRateService.InitializeAsync(default);

        // Act
        var result = await exchangeRateService.ConvertCurrencyAsync(
            usd10,
            eur,
            DateTimeOffset.Now,
            default);

        // Assert
        _ = result.IsSome.Should().BeTrue();
        _ = result.Map(s => s.Currency).Should<Option<CurrencyInfo>>().Be(Some(eur));
    }

    [Theory]
    [InlineData("LiteDB")]
    [InlineData("SQLite")]
    [InlineData("MongoDB")]
    public async Task Given_1000Dram_When_ExchangedForDanishKrone_Then_ResultShouldBeDoubleConverted(string database)
    {
        // Arrange
        var exchangeRateService = this.serviceProviderFixture.GetServiceProvider(database).GetRequiredService<IExchangeRateService>();
        var currencyFactory = this.serviceProviderFixture.GetServiceProvider(database).GetRequiredService<ICurrencyFactory>();
        var usd = currencyFactory.Create("USD");
        var amd = currencyFactory.Create("AMD");
        var dkk = currencyFactory.Create("DKK");
        var amd1000 = new Money(amd, 1000m);

        await exchangeRateService.InitializeAsync(default);

        // Act
        var resultWithoutDoubleConversion = await exchangeRateService.ConvertCurrencyAsync(
            amd1000,
            dkk,
            DateTimeOffset.Now,
            default);

        var resultWithDoubleConversion = await exchangeRateService.ConvertCurrencyAsync(
            amd1000,
            dkk,
            DateTimeOffset.Now,
            usd,
            default);

        // Assert
        _ = resultWithoutDoubleConversion.IsNone.Should().BeTrue();
        _ = resultWithoutDoubleConversion.Map(s => s.Currency).Should<Option<CurrencyInfo>>().Be(None);
        _ = resultWithDoubleConversion.IsSome.Should().BeTrue();
        _ = resultWithDoubleConversion.Map(s => s.Currency).Should<Option<CurrencyInfo>>().Be(Some(dkk));
    }
}
