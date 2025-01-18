using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class BankOfCanadaTests
{
    private readonly IBankOfCanada bank;
    private readonly TimeProvider timeProvider;

    public BankOfCanadaTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();

        var serviceProvider = services.BuildServiceProvider();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        this.bank = serviceProvider.GetRequiredService<IBankOfCanada>();
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
    public async Task ConversionDirection001()
    {
        var canadianDollar = new CurrencyInfo(new RegionInfo("CA"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, 100m);

        var afterInDollar = await this.bank.ConvertCurrencyAsync(
            beforeInPound,
            canadianDollar,
            this.timeProvider.GetUtcNow(),
            default);

        (beforeInPound.Amount < afterInDollar.Amount).ShouldBeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, decimal.One);

            var after = await this.bank.ConvertCurrencyAsync(
                before,
                pair.CounterCurrency,
                this.timeProvider.GetUtcNow(),
                default);

            (after.Amount > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, decimal.One);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

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

        var before = new Money(usd, 100m);

        _ = await new Func<Task>(async () => await this.bank.ConvertCurrencyAsync(before, cad, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertCurrency006()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var before = new Money(aoa, 100m);

        _ = await new Func<Task>(async () => await this.bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default);

        currencyPairs.ShouldContain(c => c.ToString() == "CAD/USD");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/AUD");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/BRL");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/CNY");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/EUR");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/HKD");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/INR");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/IDR");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/JPY");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/MXN");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/NZD");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/NOK");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/PEN");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/RUB");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/SGD");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/ZAR");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/KRW");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/SEK");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/CHF");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/TWD");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/TRY");
        currencyPairs.ShouldContain(c => c.ToString() == "CAD/GBP");

        currencyPairs.ShouldContain(c => c.ToString() == "USD/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "AUD/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "BRL/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "CNY/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "EUR/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "HKD/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "INR/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "IDR/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "JPY/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "MXN/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "NZD/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "NOK/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "PEN/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "RUB/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "SGD/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "ZAR/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "KRW/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "SEK/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "CHF/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "TWD/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "TRY/CAD");
        currencyPairs.ShouldContain(c => c.ToString() == "GBP/CAD");
    }

    [Fact]
    public async Task CurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default);

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

        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            _ = pairSet.Add(pair);
        }

        (pairSet.Count == currencyPairs.Count).ShouldBeTrue();
    }

    [Fact]
    public async Task CurrencyPairs005()
        => await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddDays(10), default)).ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task Fetch001()
        => await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), default);

    [Fact]
    public async Task GetExchangeRate001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default);

        foreach (var pair in currencyPairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

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

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var pair = new CurrencyPair(bwp, aoa);

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }
}
