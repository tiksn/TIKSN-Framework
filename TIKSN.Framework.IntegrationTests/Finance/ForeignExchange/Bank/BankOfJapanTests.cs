using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class BankOfJapanTests
{
    private readonly IBankOfJapan bank;
    private readonly ICurrencyFactory currencyFactory;
    private readonly TimeProvider timeProvider;

    public BankOfJapanTests(FrameworkCoreServiceProviderFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        this.bank = fixture.GetRequiredService<IBankOfJapan>();
        this.currencyFactory = fixture.GetRequiredService<ICurrencyFactory>();
        this.timeProvider = fixture.GetRequiredService<TimeProvider>();
    }

    [Fact]
    public async Task Given100USD_WhenConvertedToJPY_ThenResultShouldBeGreaterThan100JPY()
    {
        var usDollar = this.currencyFactory.Create("USD");
        var japaneseYen = this.currencyFactory.Create("JPY");
        var before = new Money(usDollar, amount: 100m);

        var result = await this.bank.ConvertCurrencyAsync(before, japaneseYen, this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        _ = result.ShouldNotBeNull();
        result.Currency.ISOCurrencySymbol.ShouldBe("JPY");
        result.Amount.ShouldBeGreaterThan(100m);
    }

    [Fact]
    public async Task GivenExchangeRates_WhenRequested_ThenDerivedEuroYenShouldEqualCrossRate()
    {
        var euro = this.currencyFactory.Create("EUR");
        var japaneseYen = this.currencyFactory.Create("JPY");
        var usDollar = this.currencyFactory.Create("USD");
        var asOn = this.timeProvider.GetUtcNow();

        var euroUsDollar = CurrencyPairTestHelper.CurrencyPairFactory.Create(euro, usDollar);
        var usDollarYen = CurrencyPairTestHelper.CurrencyPairFactory.Create(usDollar, japaneseYen);
        var euroYen = CurrencyPairTestHelper.CurrencyPairFactory.Create(euro, japaneseYen);

        var euroUsDollarRate = await this.bank.GetExchangeRateAsync(euroUsDollar, asOn,
            cancellationToken: TestContext.Current.CancellationToken);
        var usDollarYenRate = await this.bank.GetExchangeRateAsync(usDollarYen, asOn,
            cancellationToken: TestContext.Current.CancellationToken);
        var euroYenRate = await this.bank.GetExchangeRateAsync(euroYen, asOn,
            cancellationToken: TestContext.Current.CancellationToken);

        euroYenRate.ShouldBe(euroUsDollarRate * usDollarYenRate);
    }

    [Fact]
    public async Task GivenFutureDate_WhenRateRequested_ThenItShouldThrowArgumentException()
        => await new Func<Task>(async () =>
                await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(1d),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task GivenHistoricalWeekendDate_WhenRateRequested_ThenResultShouldBePositive()
    {
        var japaneseYen = this.currencyFactory.Create("JPY");
        var usDollar = this.currencyFactory.Create("USD");
        var pair = CurrencyPairTestHelper.CurrencyPairFactory.Create(usDollar, japaneseYen);
        var asOn = new DateTimeOffset(year: 2026, month: 5, day: 31, hour: 0, minute: 0, second: 0,
            offset: TimeSpan.FromHours(9));

        var result = await this.bank.GetExchangeRateAsync(pair, asOn,
            cancellationToken: TestContext.Current.CancellationToken);

        result.ShouldBeGreaterThan(decimal.Zero);
    }

    [Fact]
    public async Task GivenPairsRequested_ThenResultShouldContainExpectedCurrencies()
    {
        var result = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        _ = result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldBeUnique();
        result.Select(x => x.ToString()).ShouldBe([
            "USD/JPY",
            "JPY/USD",
            "EUR/USD",
            "USD/EUR",
            "EUR/JPY",
            "JPY/EUR",
        ], ignoreOrder: true);
    }
}
