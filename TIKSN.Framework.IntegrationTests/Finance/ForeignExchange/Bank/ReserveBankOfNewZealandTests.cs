using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Shouldly;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class ReserveBankOfNewZealandTests
{
    private readonly IReserveBankOfNewZealand bank;
    private readonly TimeProvider timeProvider;

    public ReserveBankOfNewZealandTests(FrameworkCoreServiceProviderFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        this.bank = fixture.GetRequiredService<IReserveBankOfNewZealand>();
        this.timeProvider = fixture.GetRequiredService<TimeProvider>();
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var newZealandDollar = new CurrencyInfo(new RegionInfo("NZ"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));
        var beforeInPound = new Money(poundSterling, amount: 100m);

        var afterInDollar =
            await this.bank.ConvertCurrencyAsync(beforeInPound, newZealandDollar, this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        (beforeInPound.Amount < afterInDollar.Amount).ShouldBeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, amount: 10m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency,
                this.timeProvider.GetUtcNow(), cancellationToken: TestContext.Current.CancellationToken);

            (after.Amount > decimal.Zero).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, amount: 10m);

            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency,
                this.timeProvider.GetUtcNow(), cancellationToken: TestContext.Current.CancellationToken);

            (after.Amount == before.Amount * rate).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, amount: 10m);

            _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency,
                            this.timeProvider.GetUtcNow().AddMinutes(1d),
                            cancellationToken: TestContext.Current.CancellationToken))
                    .ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task ConvertCurrency004()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, amount: 10m);

            _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency,
                            new DateTimeOffset(2017, 12, 31, 12, 0, 0, TimeSpan.Zero),
                            cancellationToken: TestContext.Current.CancellationToken))
                    .ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task ConvertCurrency005()
    {
        var armenia = new RegionInfo("AM");
        var belarus = new RegionInfo("BY");

        var armenianDram = new CurrencyInfo(armenia);
        var belarusianRuble = new CurrencyInfo(belarus);

        var before = new Money(armenianDram, amount: 10m);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, belarusianRuble, this.timeProvider.GetUtcNow(),
                        cancellationToken: TestContext.Current.CancellationToken))
                .ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Fetch001()
    {
        var rates = await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        rates.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        var expectedForeignCurrencies = new[]
        {
            "USD",
            "GBP",
            "AUD",
            "JPY",
            "EUR",
            "CNY",
            "HKD",
            "IDR",
            "MYR",
            "SGD",
            "KRW",
            "THB",
            "TWD",
            "INR",
            "PHP",
            "VND",
            "CAD",
        };

        foreach (var currencyCode in expectedForeignCurrencies)
        {
            currencyPairs.ShouldContain(p => p.ToString() == $"NZD/{currencyCode}");
            currencyPairs.ShouldContain(p => p.ToString() == $"{currencyCode}/NZD");
        }
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var reversedPair = CurrencyPairTestHelper.CurrencyPairFactory.Reverse(pair);

            currencyPairs.ShouldContain(p => p == reversedPair);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs003()
    {
        var pairSet = new HashSet<CurrencyPair>();

        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            _ = pairSet.Add(pair);
        }

        (pairSet.Count == currencyPairs.Count).ShouldBeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs004()
        => await
            new Func<Task>(async () =>
                    await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(10),
                        cancellationToken: TestContext.Current.CancellationToken))
                .ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task GetCurrencyPairs005()
        => await new Func<Task>(async () =>
                await this.bank.GetCurrencyPairsAsync(new DateTimeOffset(2017, 12, 31, 12, 0, 0, TimeSpan.Zero),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task GetExchangeRate001()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            (await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(),
                        cancellationToken: TestContext.Current.CancellationToken) >
                    decimal.Zero)
                .ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            _ = await new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d),
                        cancellationToken: TestContext.Current.CancellationToken))
                .ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var currencyPairs =
            await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            _ = await new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(pair,
                        new DateTimeOffset(2017, 12, 31, 12, 0, 0, TimeSpan.Zero),
                        cancellationToken: TestContext.Current.CancellationToken))
                .ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var armenia = new RegionInfo("AM");
        var belarus = new RegionInfo("BY");

        var armenianDram = new CurrencyInfo(armenia);
        var belarusianRuble = new CurrencyInfo(belarus);

        var pair = CurrencyPairTestHelper.CurrencyPairFactory.Create(armenianDram, belarusianRuble);

        _ = await new Func<Task>(async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();
    }
}
