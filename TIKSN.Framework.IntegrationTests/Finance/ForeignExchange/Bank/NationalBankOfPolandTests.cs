using System;
using System.Threading.Tasks;
using Shouldly;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class NationalBankOfPolandTests
{
    private readonly INationalBankOfPoland bank;
    private readonly ICurrencyFactory currencyFactory;
    private readonly TimeProvider timeProvider;

    public NationalBankOfPolandTests(FrameworkCoreServiceProviderFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        this.bank = fixture.GetRequiredService<INationalBankOfPoland>();
        this.currencyFactory = fixture.GetRequiredService<ICurrencyFactory>();
        this.timeProvider = fixture.GetRequiredService<TimeProvider>();
    }

    [Fact]
    public async Task Given10USD_WhenUSDRateRequestedHistorical_ThenResultShouldBeGreaterThan10()
    {
        // Arrange
        var thePLN = this.currencyFactory.Create("PLN");
        var theUSD = this.currencyFactory.Create("USD");
        var the10USD = new Money(theUSD, amount: 10m);
        var asOn = this.timeProvider.GetLocalNow().AddMonths(-1);

        // Act
        var result = await this.bank.ConvertCurrencyAsync(the10USD, thePLN, asOn,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        _ = result.ShouldNotBeNull();
        result.Currency.ISOCurrencySymbol.ShouldBe("PLN");
        result.Amount.ShouldBeGreaterThan(10m);
    }

    [Fact]
    public async Task Given10USD_WhenUSDRateRequestedLocalTime_ThenResultShouldBeGreaterThan10()
    {
        // Arrange
        var thePLN = this.currencyFactory.Create("PLN");
        var theUSD = this.currencyFactory.Create("USD");
        var the10USD = new Money(theUSD, amount: 10m);

        // Act
        var result = await this.bank.ConvertCurrencyAsync(the10USD, thePLN, this.timeProvider.GetLocalNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        _ = result.ShouldNotBeNull();
        result.Currency.ISOCurrencySymbol.ShouldBe("PLN");
        result.Amount.ShouldBeGreaterThan(10m);
    }

    [Fact]
    public async Task Given10USD_WhenUSDRateRequestedUtcTime_ThenResultShouldBeGreaterThan10()
    {
        // Arrange
        var thePLN = this.currencyFactory.Create("PLN");
        var theUSD = this.currencyFactory.Create("USD");
        var the10USD = new Money(theUSD, amount: 10m);

        // Act
        var result = await this.bank.ConvertCurrencyAsync(the10USD, thePLN, this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        _ = result.ShouldNotBeNull();
        result.Currency.ISOCurrencySymbol.ShouldBe("PLN");
        result.Amount.ShouldBeGreaterThan(10m);
    }

    [Fact]
    public async Task Given_WhenPairsRequested_ThenResultShouldBeUnique()
    {
        // Arrange

        // Act
        var result = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetLocalNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        _ = result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldBeUnique();
        result.ShouldAllBe(x =>
            x.BaseCurrency.ISOCurrencySymbol == "PLN" ||
            x.CounterCurrency.ISOCurrencySymbol == "PLN");
    }
}
