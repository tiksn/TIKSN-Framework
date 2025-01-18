using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class BankOfEnglandTests
{
    private readonly IBankOfEngland bank;
    private readonly TimeProvider timeProvider;

    public BankOfEnglandTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        this.bank = serviceProvider.GetRequiredService<IBankOfEngland>();
    }

    [Fact]
    public async Task Calculate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            (after.Amount == rate * before.Amount).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task Calculate002()
    {
        var tenYearsAgo = this.timeProvider.GetUtcNow().AddYears(-10);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(tenYearsAgo, default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, tenYearsAgo, default);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, tenYearsAgo, default);

            (after.Amount == rate * before.Amount).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var usDollar = new CurrencyInfo(new RegionInfo("US"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, 100m);

        var afterInDollar = await this.bank.ConvertCurrencyAsync(beforeInPound, usDollar, this.timeProvider.GetUtcNow(), default);

        (beforeInPound.Amount < afterInDollar.Amount).ShouldBeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            (after.Amount > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        var pair = currencyPairs.First();

        var before = new Money(pair.BaseCurrency, 10m);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var pair = new CurrencyPair(
            new CurrencyInfo(new RegionInfo("AM")),
            new CurrencyInfo(new RegionInfo("BY")));

        var before = new Money(pair.BaseCurrency, 10m);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        currencyPairs.ShouldContain(c => c.ToString() == "AUD/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "AUD/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "CAD/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "CNY/GBP");
        currencyPairs.ShouldContain(c => c.ToString() == "CNY/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "CZK/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "CZK/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "DKK/GBP");
        currencyPairs.ShouldContain(c => c.ToString() == "DKK/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "EUR/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "HKD/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "HKD/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "HUF/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "HUF/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "INR/GBP");
        currencyPairs.ShouldContain(c => c.ToString() == "INR/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "ILS/GBP");
        currencyPairs.ShouldContain(c => c.ToString() == "ILS/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "JPY/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "JPY/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "MYR/GBP");
        currencyPairs.ShouldContain(c => c.ToString() == "MYR/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "NZD/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "NZD/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "NOK/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "NOK/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "PLN/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "PLN/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "SAR/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "SAR/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "SGD/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "SGD/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "ZAR/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "ZAR/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "KRW/GBP");
        currencyPairs.ShouldContain(c => c.ToString() == "KRW/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "GBP/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "SEK/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "SEK/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "CHF/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "CHF/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "TWD/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "TWD/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "THB/GBP");
        currencyPairs.ShouldContain(c => c.ToString() == "THB/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "TRY/USD");

        currencyPairs.ShouldContain(c => c.ToString() == "USD/GBP");
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        var uniquePairs = new HashSet<CurrencyPair>();

        foreach (var pair in currencyPairs)
        {
            _ = uniquePairs.Add(pair);
        }

        (uniquePairs.Count == currencyPairs.Count).ShouldBeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddYears(-10), default);

        var uniquePairs = new HashSet<CurrencyPair>();

        foreach (var pair in currencyPairs)
        {
            _ = uniquePairs.Add(pair);
        }

        (uniquePairs.Count == currencyPairs.Count).ShouldBeTrue();
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            (await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default) > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        _ = await
                new Func<Task>(async () =>
                        await this.bank.GetExchangeRateAsync(currencyPairs.First(), this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var pair = new CurrencyPair(
            new CurrencyInfo(new RegionInfo("AM")),
            new CurrencyInfo(new RegionInfo("BY")));

        _ = await
                new Func<Task>(async () =>
                        await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public void KeepCurrenciesPairsUpdated()
    {
        // In case or failure, check currency pair information from BOE website and set deadline
        // up to 3 month.

        var deadline = new DateTimeOffset(2024, 02, 01, 0, 0, 0, TimeSpan.Zero);

        deadline.ShouldBeGreaterThan(this.timeProvider.GetUtcNow());
    }
}
