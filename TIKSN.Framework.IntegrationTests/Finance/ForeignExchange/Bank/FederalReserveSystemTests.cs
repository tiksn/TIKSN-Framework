using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Bank.IntegrationTests;

public class FederalReserveSystemTests
{
    private readonly IFederalReserveSystem bank;
    private readonly TimeProvider timeProvider;

    public FederalReserveSystemTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        this.bank = serviceProvider.GetRequiredService<IFederalReserveSystem>();
    }

    [Fact]
    public async Task Calculation001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            Assert.True(after.Amount == rate * before.Amount);
        }
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var usDollar = new CurrencyInfo(new RegionInfo("US"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, 100m);

        var afterInDollar = await this.bank.ConvertCurrencyAsync(beforeInPound, usDollar, this.timeProvider.GetUtcNow(), default);

        Assert.True(beforeInPound.Amount < afterInDollar.Amount);
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            Assert.True(after.Amount > decimal.Zero);
            Assert.True(after.Currency == pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var lastYear = this.timeProvider.GetUtcNow().AddYears(-1);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(lastYear, default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, lastYear, default);

            Assert.True(after.Amount > decimal.Zero);
            Assert.True(after.Currency == pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 10m);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(1d), default));
        }
    }

    [Fact]
    public async Task ConvertCurrency004()
    {
        var before = new Money(new CurrencyInfo(new RegionInfo("AL")), 10m);

        _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await this.bank.ConvertCurrencyAsync(before, new CurrencyInfo(new RegionInfo("AM")), this.timeProvider.GetUtcNow().AddMinutes(1d), default));
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            Assert.Contains(pairs, c => c == reversed);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

        foreach (var pair in pairs)
        {
            Assert.True(uniquePairs.Add(pair));
        }

        Assert.True(uniquePairs.Count == pairs.Count());
    }

    [Fact]
    public async Task GetCurrencyPairs003()
    {
        _ = await
            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(1d), default));
    }

    [Fact]
    public async Task GetCurrencyPairs004()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        Assert.Contains(pairs, c => c.ToString() == "AUD/USD");
        Assert.Contains(pairs, c => c.ToString() == "BRL/USD");
        Assert.Contains(pairs, c => c.ToString() == "CAD/USD");
        Assert.Contains(pairs, c => c.ToString() == "CNY/USD");
        Assert.Contains(pairs, c => c.ToString() == "DKK/USD");
        Assert.Contains(pairs, c => c.ToString() == "EUR/USD");
        Assert.Contains(pairs, c => c.ToString() == "HKD/USD");
        Assert.Contains(pairs, c => c.ToString() == "INR/USD");
        Assert.Contains(pairs, c => c.ToString() == "JPY/USD");
        Assert.Contains(pairs, c => c.ToString() == "MYR/USD");
        Assert.Contains(pairs, c => c.ToString() == "MXN/USD");
        Assert.Contains(pairs, c => c.ToString() == "NZD/USD");
        Assert.Contains(pairs, c => c.ToString() == "NOK/USD");
        Assert.Contains(pairs, c => c.ToString() == "SGD/USD");
        Assert.Contains(pairs, c => c.ToString() == "ZAR/USD");
        Assert.Contains(pairs, c => c.ToString() == "KRW/USD");
        Assert.Contains(pairs, c => c.ToString() == "LKR/USD");
        Assert.Contains(pairs, c => c.ToString() == "SEK/USD");
        Assert.Contains(pairs, c => c.ToString() == "CHF/USD");
        Assert.Contains(pairs, c => c.ToString() == "TWD/USD");
        Assert.Contains(pairs, c => c.ToString() == "THB/USD");
        Assert.Contains(pairs, c => c.ToString() == "GBP/USD");

        Assert.Contains(pairs, c => c.ToString() == "USD/AUD");
        Assert.Contains(pairs, c => c.ToString() == "USD/BRL");
        Assert.Contains(pairs, c => c.ToString() == "USD/CAD");
        Assert.Contains(pairs, c => c.ToString() == "USD/CNY");
        Assert.Contains(pairs, c => c.ToString() == "USD/DKK");
        Assert.Contains(pairs, c => c.ToString() == "USD/EUR");
        Assert.Contains(pairs, c => c.ToString() == "USD/HKD");
        Assert.Contains(pairs, c => c.ToString() == "USD/INR");
        Assert.Contains(pairs, c => c.ToString() == "USD/JPY");
        Assert.Contains(pairs, c => c.ToString() == "USD/MYR");
        Assert.Contains(pairs, c => c.ToString() == "USD/MXN");
        Assert.Contains(pairs, c => c.ToString() == "USD/NZD");
        Assert.Contains(pairs, c => c.ToString() == "USD/NOK");
        Assert.Contains(pairs, c => c.ToString() == "USD/SGD");
        Assert.Contains(pairs, c => c.ToString() == "USD/ZAR");
        Assert.Contains(pairs, c => c.ToString() == "USD/KRW");
        Assert.Contains(pairs, c => c.ToString() == "USD/LKR");
        Assert.Contains(pairs, c => c.ToString() == "USD/SEK");
        Assert.Contains(pairs, c => c.ToString() == "USD/CHF");
        Assert.Contains(pairs, c => c.ToString() == "USD/TWD");
        Assert.Contains(pairs, c => c.ToString() == "USD/THB");
        Assert.Contains(pairs, c => c.ToString() == "USD/GBP");
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            Assert.True(rate > decimal.Zero);
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var lastYear = this.timeProvider.GetUtcNow().AddYears(-1);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(lastYear, default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, lastYear, default);

            Assert.True(rate > decimal.Zero);
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default));
        }
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("AL")), new CurrencyInfo(new RegionInfo("AM")));

        _ = await Assert.ThrowsAsync<ArgumentException>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default));
    }

    [Fact]
    public async Task GetExchangeRate005()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("CN")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        Assert.True(rate > decimal.One);
    }

    [Fact]
    public async Task GetExchangeRate006()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("SG")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        Assert.True(rate > decimal.One);
    }

    [Fact]
    public async Task GetExchangeRate007()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("DE")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        Assert.True(rate < decimal.One);
    }

    [Fact]
    public async Task GetExchangeRate008()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("GB")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        Assert.True(rate < decimal.One);
    }
}
