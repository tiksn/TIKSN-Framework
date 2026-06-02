using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Shouldly;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class BankOfCanadaTests
{
    private readonly IBankOfCanada bank;
    private readonly TimeProvider timeProvider;

    public BankOfCanadaTests(FrameworkCoreServiceProviderFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        this.timeProvider = fixture.GetRequiredService<TimeProvider>();
        this.bank = fixture.GetRequiredService<IBankOfCanada>();
    }

    [Fact]
    public async Task Calculate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(),
                     cancellationToken: TestContext.Current.CancellationToken))
        {
            var before = new Money(pair.BaseCurrency, amount: 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency,
                this.timeProvider.GetUtcNow(), cancellationToken: TestContext.Current.CancellationToken);

            (after.Amount == rate * before.Amount).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var canadianDollar = new CurrencyInfo(new RegionInfo("CA"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, amount: 100m);

        var afterInDollar = await this.bank.ConvertCurrencyAsync(
            beforeInPound,
            canadianDollar,
            this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        (beforeInPound.Amount < afterInDollar.Amount).ShouldBeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, decimal.One);

            var after = await this.bank.ConvertCurrencyAsync(
                before,
                pair.CounterCurrency,
                this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

            (after.Amount > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, decimal.One);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency,
                this.timeProvider.GetUtcNow(), cancellationToken: TestContext.Current.CancellationToken);

            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, amount: 10m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency,
                this.timeProvider.GetUtcNow(), cancellationToken: TestContext.Current.CancellationToken);

            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
            (after.Amount == rate * before.Amount).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency004()
    {
        var us = new RegionInfo("US");
        var ca = new RegionInfo("CA");

        var usd = new CurrencyInfo(us);
        var cad = new CurrencyInfo(ca);

        var before = new Money(usd, amount: 100m);

        _ = await new Func<Task>(async () =>
                await this.bank.ConvertCurrencyAsync(before, cad, this.timeProvider.GetUtcNow().AddMinutes(1d),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertCurrency006()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var before = new Money(aoa, amount: 100m);

        _ = await new Func<Task>(async () =>
                await this.bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        var expectedForeignCurrencies = new[]
        {
            "AUD",
            "BRL",
            "CNY",
            "EUR",
            "HKD",
            "INR",
            "IDR",
            "JPY",
            "MYR",
            "MXN",
            "NZD",
            "NOK",
            "PEN",
            "PLN",
            "SGD",
            "ZAR",
            "KRW",
            "SEK",
            "CHF",
            "TWD",
            "THB",
            "TRY",
            "GBP",
            "USD",
        };

        foreach (var currencyCode in expectedForeignCurrencies)
        {
            currencyPairs.ShouldContain(c => c.ToString() == $"CAD/{currencyCode}");
            currencyPairs.ShouldContain(c => c.ToString() == $"{currencyCode}/CAD");
        }
    }

    [Fact]
    public async Task CurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            currencyPairs.ShouldContain(c => c == reversePair);
        }
    }

    [Fact]
    public async Task CurrencyPairs003()
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
    public async Task CurrencyPairs005()
        => await new Func<Task>(async () =>
                await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddDays(10),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task Fetch001()
        => await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

    [Fact]
    public async Task GetExchangeRate001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            cancellationToken: TestContext.Current.CancellationToken);

        foreach (var pair in currencyPairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(),
                cancellationToken: TestContext.Current.CancellationToken);

            (rate > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var us = new RegionInfo("US");
        var ca = new RegionInfo("CA");

        var usd = new CurrencyInfo(us);
        var cad = new CurrencyInfo(ca);

        var pair = new CurrencyPair(cad, usd);

        _ = await new Func<Task>(async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var pair = new CurrencyPair(bwp, aoa);

        _ = await new Func<Task>(async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(),
                    cancellationToken: TestContext.Current.CancellationToken))
            .ShouldThrowAsync<ArgumentException>();
    }
}
