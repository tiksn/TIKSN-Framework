using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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
        => this.serviceProviderFixture =
            serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));

    [Theory]
    [InlineData("LiteDB")]
    [InlineData("SQLite")]
    [InlineData("MongoDB")]
    [InlineData("RavenDB")]
    public async Task Given_1000Dram_When_ExchangedForDanishKrone_Then_ResultShouldBeDoubleConverted(string database)
    {
        // Arrange
        var exchangeRateService = this.serviceProviderFixture.GetServiceProvider(database)
            .GetRequiredService<IExchangeRateService>();
        var currencyFactory = this.serviceProviderFixture.GetServiceProvider(database)
            .GetRequiredService<ICurrencyFactory>();
        var usd = currencyFactory.Create("USD");
        var amd = currencyFactory.Create("AMD");
        var dkk = currencyFactory.Create("DKK");
        var amd1000 = new Money(amd, amount: 1000m);

        await exchangeRateService.InitializeAsync(TestContext.Current.CancellationToken);

        // Act
        var resultWithoutDoubleConversion = await exchangeRateService.ConvertCurrencyAsync(
            amd1000,
            dkk,
            DateTimeOffset.Now,
            cancellationToken: TestContext.Current.CancellationToken);

        var resultWithDoubleConversion = await exchangeRateService.ConvertCurrencyAsync(
            amd1000,
            dkk,
            DateTimeOffset.Now,
            usd,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resultWithoutDoubleConversion.IsNone.ShouldBeTrue();
        resultWithoutDoubleConversion.Map(s => s.Currency).ShouldBe(None);
        resultWithDoubleConversion.IsSome.ShouldBeTrue();
        resultWithDoubleConversion.Map(s => s.Currency).ShouldBe(Some(dkk));
    }

    [Theory]
    [InlineData("LiteDB")]
    [InlineData("SQLite")]
    [InlineData("MongoDB")]
    [InlineData("RavenDB")]
    public async Task Given_10USD_When_ExchangedForEuro_Then_ResultShouldBeEuro(string database)
    {
        // Arrange
        var exchangeRateService = this.serviceProviderFixture.GetServiceProvider(database)
            .GetRequiredService<IExchangeRateService>();
        var currencyFactory = this.serviceProviderFixture.GetServiceProvider(database)
            .GetRequiredService<ICurrencyFactory>();
        var usd = currencyFactory.Create("USD");
        var eur = currencyFactory.Create("EUR");
        var usd10 = new Money(usd, amount: 10m);

        await exchangeRateService.InitializeAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await exchangeRateService.ConvertCurrencyAsync(
            usd10,
            eur,
            DateTimeOffset.Now,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.IsSome.ShouldBeTrue();
        result.Map(s => s.Currency).ShouldBe(Some(eur));
    }
}
