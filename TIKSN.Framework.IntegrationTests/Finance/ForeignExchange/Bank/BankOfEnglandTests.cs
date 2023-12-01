using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Bank.IntegrationTests;

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
        foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            Assert.True(after.Amount == rate * before.Amount);
            Assert.True(after.Currency == pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task Calculate002()
    {
        var tenYearsAgo = this.timeProvider.GetUtcNow().AddYears(-10);

        foreach (var pair in await bank.GetCurrencyPairsAsync(tenYearsAgo, default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await bank.GetExchangeRateAsync(pair, tenYearsAgo, default);

            var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, tenYearsAgo, default);

            Assert.True(after.Amount == rate * before.Amount);
            Assert.True(after.Currency == pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var usDollar = new CurrencyInfo(new RegionInfo("US"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, 100m);

        var afterInDollar = await bank.ConvertCurrencyAsync(beforeInPound, usDollar, this.timeProvider.GetUtcNow(), default);

        Assert.True(beforeInPound.Amount < afterInDollar.Amount);
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            Assert.True(after.Amount > decimal.Zero);
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        var pair = currencyPairs.First();

        var before = new Money(pair.BaseCurrency, 10m);

        _ = await
            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(1d), default));
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var pair = new CurrencyPair(
            new CurrencyInfo(new RegionInfo("AM")),
            new CurrencyInfo(new RegionInfo("BY")));

        var before = new Money(pair.BaseCurrency, 10m);

        _ = await
            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default));
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        Assert.Contains(currencyPairs, c => c.ToString() == "AUD/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "AUD/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "CAD/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "CNY/GBP");
        Assert.Contains(currencyPairs, c => c.ToString() == "CNY/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "CZK/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "CZK/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "DKK/GBP");
        Assert.Contains(currencyPairs, c => c.ToString() == "DKK/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "EUR/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "HKD/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "HKD/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "HUF/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "HUF/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "INR/GBP");
        Assert.Contains(currencyPairs, c => c.ToString() == "INR/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "ILS/GBP");
        Assert.Contains(currencyPairs, c => c.ToString() == "ILS/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "JPY/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "JPY/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "MYR/GBP");
        Assert.Contains(currencyPairs, c => c.ToString() == "MYR/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "NZD/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "NZD/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "NOK/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "NOK/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "PLN/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "PLN/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "SAR/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "SAR/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "SGD/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "SGD/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "ZAR/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "ZAR/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "KRW/GBP");
        Assert.Contains(currencyPairs, c => c.ToString() == "KRW/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "GBP/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "SEK/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "SEK/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "CHF/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "CHF/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "TWD/USD");
        Assert.Contains(currencyPairs, c => c.ToString() == "TWD/GBP");

        Assert.Contains(currencyPairs, c => c.ToString() == "THB/GBP");
        Assert.Contains(currencyPairs, c => c.ToString() == "THB/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "TRY/USD");

        Assert.Contains(currencyPairs, c => c.ToString() == "USD/GBP");
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        var uniquePairs = new HashSet<CurrencyPair>();

        foreach (var pair in currencyPairs)
        {
            _ = uniquePairs.Add(pair);
        }

        Assert.True(uniquePairs.Count == currencyPairs.Count());
    }

    [Fact]
    public async Task GetCurrencyPairs003()
    {
        var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddYears(-10), default);

        var uniquePairs = new HashSet<CurrencyPair>();

        foreach (var pair in currencyPairs)
        {
            _ = uniquePairs.Add(pair);
        }

        Assert.True(uniquePairs.Count == currencyPairs.Count());
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            Assert.True(await bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default) > decimal.Zero);
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(currencyPairs.First(), this.timeProvider.GetUtcNow().AddMinutes(1d), default));
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var pair = new CurrencyPair(
            new CurrencyInfo(new RegionInfo("AM")),
            new CurrencyInfo(new RegionInfo("BY")));

        _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default));
    }

    [Fact]
    public async Task KeepCurrenciesPairsUpdated()
    {
        // In case or failure, check currency pair information from BOE website and set deadline
        // up to 3 month.

        DateTimeOffset deadline = new DateTime(2024, 02, 01);

        if (this.timeProvider.GetUtcNow() > deadline)
            throw new Exception("Source is out of date. Please update.");
    }
}
