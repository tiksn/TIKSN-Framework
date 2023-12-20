using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(false))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

            _ = (after.Amount == rate * before.Amount).Should().BeTrue();
            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Calculate002()
    {
        var tenYearsAgo = this.timeProvider.GetUtcNow().AddYears(-10);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(tenYearsAgo, default).ConfigureAwait(false))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, tenYearsAgo, default).ConfigureAwait(false);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, tenYearsAgo, default).ConfigureAwait(false);

            _ = (after.Amount == rate * before.Amount).Should().BeTrue();
            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
        }
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var usDollar = new CurrencyInfo(new RegionInfo("US"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, 100m);

        var afterInDollar = await this.bank.ConvertCurrencyAsync(beforeInPound, usDollar, this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

        _ = (beforeInPound.Amount < afterInDollar.Amount).Should().BeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

            _ = (after.Amount > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

        var pair = currencyPairs.First();

        var before = new Money(pair.BaseCurrency, 10m);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(1d), default).ConfigureAwait(false)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false);
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
                    await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(false)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false);
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

        _ = currencyPairs.Should().Contain(c => c.ToString() == "AUD/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "AUD/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "CNY/GBP");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CNY/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "CZK/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CZK/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "DKK/GBP");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "DKK/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "EUR/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "HKD/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "HKD/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "HUF/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "HUF/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "INR/GBP");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "INR/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "ILS/GBP");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "ILS/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "JPY/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "JPY/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "MYR/GBP");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "MYR/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "NZD/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "NZD/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "NOK/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "NOK/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "PLN/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "PLN/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "SAR/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "SAR/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "SGD/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "SGD/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "ZAR/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "ZAR/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "KRW/GBP");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "KRW/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "GBP/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "SEK/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "SEK/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "CHF/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CHF/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "TWD/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "TWD/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "THB/GBP");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "THB/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "TRY/USD");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "USD/GBP");
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

        var uniquePairs = new HashSet<CurrencyPair>();

        foreach (var pair in currencyPairs)
        {
            _ = uniquePairs.Add(pair);
        }

        _ = (uniquePairs.Count == currencyPairs.Count()).Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddYears(-10), default).ConfigureAwait(false);

        var uniquePairs = new HashSet<CurrencyPair>();

        foreach (var pair in currencyPairs)
        {
            _ = uniquePairs.Add(pair);
        }

        _ = (uniquePairs.Count == currencyPairs.Count()).Should().BeTrue();
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(false))
        {
            _ = (await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(false) > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(false);

        _ = await
                new Func<Task>(async () =>
                        await this.bank.GetExchangeRateAsync(currencyPairs.First(), this.timeProvider.GetUtcNow().AddMinutes(1d), default).ConfigureAwait(false)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false);
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var pair = new CurrencyPair(
            new CurrencyInfo(new RegionInfo("AM")),
            new CurrencyInfo(new RegionInfo("BY")));

        _ = await
                new Func<Task>(async () =>
                        await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(false)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false);
    }

    [Fact]
    public void KeepCurrenciesPairsUpdated()
    {
        // In case or failure, check currency pair information from BOE website and set deadline
        // up to 3 month.

        var deadline = new DateTimeOffset(2024, 02, 01, 0, 0, 0, TimeSpan.Zero);

        _ = deadline.Should().BeAfter(this.timeProvider.GetUtcNow());
    }
}
