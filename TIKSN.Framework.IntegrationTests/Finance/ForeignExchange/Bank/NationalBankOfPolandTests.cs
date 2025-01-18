using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
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

    public NationalBankOfPolandTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.bank = serviceProvider.GetRequiredService<INationalBankOfPoland>();
        this.currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
    }

    [Fact]
    public async Task Given_WhenPairsRequested_ThenResultShouldBeUnique()
    {
        // Arrange

        // Act
        var result = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetLocalNow(), default);

        // Assert
        _ = result.Should().NotBeNull();
        _ = result.Should().NotBeEmpty();
        _ = result.Should().OnlyHaveUniqueItems();
        _ = result.Should().OnlyContain(x =>
            x.BaseCurrency.ISOCurrencySymbol == "PLN" ||
            x.CounterCurrency.ISOCurrencySymbol == "PLN");
    }

    [Fact]
    public async Task Given10USD_WhenUSDRateRequestedHistorical_ThenResultShouldBeGreaterThan10()
    {
        // Arrange
        var thePLN = this.currencyFactory.Create("PLN");
        var theUSD = this.currencyFactory.Create("USD");
        var the10USD = new Money(theUSD, 10m);
        var asOn = this.timeProvider.GetLocalNow().AddMonths(-1);

        // Act
        var result = await this.bank.ConvertCurrencyAsync(the10USD, thePLN, asOn, default);

        // Assert
        _ = result.Should().NotBeNull();
        _ = result.Currency.ISOCurrencySymbol.Should().Be("PLN");
        _ = result.Amount.Should().BeGreaterThan(10m);
    }

    [Fact]
    public async Task Given10USD_WhenUSDRateRequestedLocalTime_ThenResultShouldBeGreaterThan10()
    {
        // Arrange
        var thePLN = this.currencyFactory.Create("PLN");
        var theUSD = this.currencyFactory.Create("USD");
        var the10USD = new Money(theUSD, 10m);

        // Act
        var result = await this.bank.ConvertCurrencyAsync(the10USD, thePLN, this.timeProvider.GetLocalNow(), default);

        // Assert
        _ = result.Should().NotBeNull();
        _ = result.Currency.ISOCurrencySymbol.Should().Be("PLN");
        _ = result.Amount.Should().BeGreaterThan(10m);
    }

    [Fact]
    public async Task Given10USD_WhenUSDRateRequestedUtcTime_ThenResultShouldBeGreaterThan10()
    {
        // Arrange
        var thePLN = this.currencyFactory.Create("PLN");
        var theUSD = this.currencyFactory.Create("USD");
        var the10USD = new Money(theUSD, 10m);

        // Act
        var result = await this.bank.ConvertCurrencyAsync(the10USD, thePLN, this.timeProvider.GetUtcNow(), default);

        // Assert
        _ = result.Should().NotBeNull();
        _ = result.Currency.ISOCurrencySymbol.Should().Be("PLN");
        _ = result.Amount.Should().BeGreaterThan(10m);
    }
}
