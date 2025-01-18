using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

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

            _ = (after.Amount == rate * before.Amount).Should().BeTrue();
        }
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var usDollar = new CurrencyInfo(new RegionInfo("US"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, 100m);

        var afterInDollar = await this.bank.ConvertCurrencyAsync(beforeInPound, usDollar, this.timeProvider.GetUtcNow(), default);

        _ = (beforeInPound.Amount < afterInDollar.Amount).Should().BeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            _ = (after.Amount > decimal.Zero).Should().BeTrue();
            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
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

            _ = (after.Amount > decimal.Zero).Should().BeTrue();
            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 10m);

            _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task ConvertCurrency004()
    {
        var before = new Money(new CurrencyInfo(new RegionInfo("AL")), 10m);

        _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, new CurrencyInfo(new RegionInfo("AM")), this.timeProvider.GetUtcNow().AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            _ = pairs.Should().Contain(c => c == reversed);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

        foreach (var pair in pairs)
        {
            _ = uniquePairs.Add(pair).Should().BeTrue();
        }

        _ = (uniquePairs.Count == pairs.Count).Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003()
        => _ = await
            new Func<Task>(async () =>
                    await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();

    [Fact]
    public async Task GetCurrencyPairs004()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        _ = pairs.Should().Contain(c => c.ToString() == "AUD/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "BRL/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "CAD/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "CNY/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "DKK/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "EUR/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "HKD/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "INR/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "JPY/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "MYR/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "MXN/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "NZD/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "NOK/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "SGD/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "ZAR/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "KRW/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "LKR/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "SEK/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "CHF/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "TWD/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "THB/USD");
        _ = pairs.Should().Contain(c => c.ToString() == "GBP/USD");

        _ = pairs.Should().Contain(c => c.ToString() == "USD/AUD");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/BRL");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/CAD");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/CNY");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/DKK");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/EUR");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/HKD");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/INR");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/JPY");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/MYR");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/MXN");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/NZD");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/NOK");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/SGD");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/ZAR");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/KRW");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/LKR");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/SEK");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/CHF");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/TWD");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/THB");
        _ = pairs.Should().Contain(c => c.ToString() == "USD/GBP");
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            _ = (rate > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var lastYear = this.timeProvider.GetUtcNow().AddYears(-1);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(lastYear, default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, lastYear, default);

            _ = (rate > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            _ = await new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("AL")), new CurrencyInfo(new RegionInfo("AM")));

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate005()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("CN")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        _ = (rate > decimal.One).Should().BeTrue();
    }

    [Fact]
    public async Task GetExchangeRate006()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("SG")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        _ = (rate > decimal.One).Should().BeTrue();
    }

    [Fact]
    public async Task GetExchangeRate007()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("DE")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        _ = (rate < decimal.One).Should().BeTrue();
    }

    [Fact]
    public async Task GetExchangeRate008()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("GB")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        _ = (rate < decimal.One).Should().BeTrue();
    }
}
