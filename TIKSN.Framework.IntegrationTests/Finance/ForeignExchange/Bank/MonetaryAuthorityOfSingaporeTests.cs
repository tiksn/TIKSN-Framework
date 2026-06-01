using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class MonetaryAuthorityOfSingaporeTests
{
    private readonly IMonetaryAuthorityOfSingapore bank;
    private readonly ICurrencyFactory currencyFactory;
    private readonly TimeProvider timeProvider;

    public MonetaryAuthorityOfSingaporeTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.bank = serviceProvider.GetRequiredService<IMonetaryAuthorityOfSingapore>();
        this.currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
    }

    [Fact]
    public async Task Given10USD_WhenUSDRateRequested_ThenResultShouldBeGreaterThan10SGD()
    {
        var theSGD = this.currencyFactory.Create("SGD");
        var theUSD = this.currencyFactory.Create("USD");
        var the10USD = new Money(theUSD, amount: 10m);

        var result = await this.bank.ConvertCurrencyAsync(the10USD, theSGD, this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        _ = result.ShouldNotBeNull();
        result.Currency.ISOCurrencySymbol.ShouldBe("SGD");
        result.Amount.ShouldBeGreaterThan(10m);
    }

    [Fact]
    public async Task Given100SGD_WhenUSDRateRequested_ThenResultShouldBeLessThan100USD()
    {
        var theSGD = this.currencyFactory.Create("SGD");
        var theUSD = this.currencyFactory.Create("USD");
        var the100SGD = new Money(theSGD, amount: 100m);

        var result = await this.bank.ConvertCurrencyAsync(the100SGD, theUSD, this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        _ = result.ShouldNotBeNull();
        result.Currency.ISOCurrencySymbol.ShouldBe("USD");
        result.Amount.ShouldBeLessThan(100m);
    }

    [Fact]
    public async Task Given_WhenPairsRequested_ThenResultShouldContainExpectedCurrencies()
    {
        var result = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        _ = result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldBeUnique();
        result.ShouldContain(x => x.ToString() == "USD/SGD");
        result.ShouldContain(x => x.ToString() == "SGD/USD");
        result.ShouldContain(x => x.ToString() == "JPY/SGD");
        result.ShouldContain(x => x.ToString() == "SGD/JPY");
    }

    [Fact]
    public async Task GivenHistoricalDate_WhenRateRequested_ThenResultShouldBePositive()
    {
        var theSGD = this.currencyFactory.Create("SGD");
        var theUSD = this.currencyFactory.Create("USD");
        var pair = new CurrencyPair(theUSD, theSGD);
        var asOn = new DateTimeOffset(year: 2024, month: 12, day: 31, hour: 0, minute: 0, second: 0,
            offset: TimeSpan.FromHours(8));

        var result = await this.bank.GetExchangeRateAsync(pair, asOn,
            cancellationToken: TestContext.Current.CancellationToken);

        result.ShouldBeGreaterThan(decimal.Zero);
    }

    [Fact]
    public async Task GivenFutureDate_WhenRateRequested_ThenItShouldThrowArgumentException()
        => await new Func<Task>(async () =>
                await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(1d),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();
}
