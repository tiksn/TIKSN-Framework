using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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

            (after.Amount == rate * before.Amount).ShouldBeTrue();
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
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            (after.Amount > decimal.Zero).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
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

            (after.Amount > decimal.Zero).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
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
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task ConvertCurrency004()
    {
        var before = new Money(new CurrencyInfo(new RegionInfo("AL")), 10m);

        _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, new CurrencyInfo(new RegionInfo("AM")), this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            pairs.ShouldContain(c => c == reversed);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

        foreach (var pair in pairs)
        {
            uniquePairs.Add(pair).ShouldBeTrue();
        }

        (uniquePairs.Count == pairs.Count).ShouldBeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003()
        => await
            new Func<Task>(async () =>
                    await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task GetCurrencyPairs004()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        pairs.ShouldContain(c => c.ToString() == "AUD/USD");
        pairs.ShouldContain(c => c.ToString() == "BRL/USD");
        pairs.ShouldContain(c => c.ToString() == "CAD/USD");
        pairs.ShouldContain(c => c.ToString() == "CNY/USD");
        pairs.ShouldContain(c => c.ToString() == "DKK/USD");
        pairs.ShouldContain(c => c.ToString() == "EUR/USD");
        pairs.ShouldContain(c => c.ToString() == "HKD/USD");
        pairs.ShouldContain(c => c.ToString() == "INR/USD");
        pairs.ShouldContain(c => c.ToString() == "JPY/USD");
        pairs.ShouldContain(c => c.ToString() == "MYR/USD");
        pairs.ShouldContain(c => c.ToString() == "MXN/USD");
        pairs.ShouldContain(c => c.ToString() == "NZD/USD");
        pairs.ShouldContain(c => c.ToString() == "NOK/USD");
        pairs.ShouldContain(c => c.ToString() == "SGD/USD");
        pairs.ShouldContain(c => c.ToString() == "ZAR/USD");
        pairs.ShouldContain(c => c.ToString() == "KRW/USD");
        pairs.ShouldContain(c => c.ToString() == "LKR/USD");
        pairs.ShouldContain(c => c.ToString() == "SEK/USD");
        pairs.ShouldContain(c => c.ToString() == "CHF/USD");
        pairs.ShouldContain(c => c.ToString() == "TWD/USD");
        pairs.ShouldContain(c => c.ToString() == "THB/USD");
        pairs.ShouldContain(c => c.ToString() == "GBP/USD");

        pairs.ShouldContain(c => c.ToString() == "USD/AUD");
        pairs.ShouldContain(c => c.ToString() == "USD/BRL");
        pairs.ShouldContain(c => c.ToString() == "USD/CAD");
        pairs.ShouldContain(c => c.ToString() == "USD/CNY");
        pairs.ShouldContain(c => c.ToString() == "USD/DKK");
        pairs.ShouldContain(c => c.ToString() == "USD/EUR");
        pairs.ShouldContain(c => c.ToString() == "USD/HKD");
        pairs.ShouldContain(c => c.ToString() == "USD/INR");
        pairs.ShouldContain(c => c.ToString() == "USD/JPY");
        pairs.ShouldContain(c => c.ToString() == "USD/MYR");
        pairs.ShouldContain(c => c.ToString() == "USD/MXN");
        pairs.ShouldContain(c => c.ToString() == "USD/NZD");
        pairs.ShouldContain(c => c.ToString() == "USD/NOK");
        pairs.ShouldContain(c => c.ToString() == "USD/SGD");
        pairs.ShouldContain(c => c.ToString() == "USD/ZAR");
        pairs.ShouldContain(c => c.ToString() == "USD/KRW");
        pairs.ShouldContain(c => c.ToString() == "USD/LKR");
        pairs.ShouldContain(c => c.ToString() == "USD/SEK");
        pairs.ShouldContain(c => c.ToString() == "USD/CHF");
        pairs.ShouldContain(c => c.ToString() == "USD/TWD");
        pairs.ShouldContain(c => c.ToString() == "USD/THB");
        pairs.ShouldContain(c => c.ToString() == "USD/GBP");
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            (rate > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var lastYear = this.timeProvider.GetUtcNow().AddYears(-1);

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(lastYear, default))
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, lastYear, default);

            (rate > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            _ = await new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("AL")), new CurrencyInfo(new RegionInfo("AM")));

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate005()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("CN")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        (rate > decimal.One).ShouldBeTrue();
    }

    [Fact]
    public async Task GetExchangeRate006()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("SG")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        (rate > decimal.One).ShouldBeTrue();
    }

    [Fact]
    public async Task GetExchangeRate007()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("DE")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        (rate < decimal.One).ShouldBeTrue();
    }

    [Fact]
    public async Task GetExchangeRate008()
    {
        var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("GB")));

        var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

        (rate < decimal.One).ShouldBeTrue();
    }
}
