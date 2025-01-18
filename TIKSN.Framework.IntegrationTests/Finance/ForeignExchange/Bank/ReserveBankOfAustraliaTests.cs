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

public class ReserveBankOfAustraliaTests
{
    private readonly IReserveBankOfAustralia bank;
    private readonly TimeProvider timeProvider;

    public ReserveBankOfAustraliaTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.bank = serviceProvider.GetRequiredService<IReserveBankOfAustralia>();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var australianDollar = new CurrencyInfo(new RegionInfo("AU"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, 100m);

        var afterInDollar = await this.bank.ConvertCurrencyAsync(beforeInPound, australianDollar, this.timeProvider.GetUtcNow(), default);

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
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            (after.Amount == before.Amount * rate).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
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
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddDays(-20d), default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task ConvertCurrency005()
    {
        var armenia = new RegionInfo("AM");
        var belarus = new RegionInfo("BY");

        var armenianDram = new CurrencyInfo(armenia);
        var belarusianRuble = new CurrencyInfo(belarus);

        var before = new Money(armenianDram, 10m);

        _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, belarusianRuble, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Fetch001()
    {
        var rates = await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), default);

        rates.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        currencyPairs.ShouldContain(p => p.ToString() == "USD/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "CNY/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "JPY/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "EUR/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "KRW/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "GBP/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "SGD/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "INR/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "THB/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "NZD/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "TWD/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "MYR/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "IDR/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "VND/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "HKD/AUD");
        currencyPairs.ShouldContain(p => p.ToString() == "XDR/AUD");

        currencyPairs.ShouldContain(p => p.ToString() == "AUD/USD");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/CNY");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/JPY");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/EUR");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/KRW");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/GBP");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/SGD");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/INR");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/THB");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/NZD");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/TWD");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/MYR");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/IDR");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/VND");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/HKD");
        currencyPairs.ShouldContain(p => p.ToString() == "AUD/XDR");
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            currencyPairs.ShouldContain(p => p == reversedPair);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs003()
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
    public async Task GetCurrencyPairs004()
        => await
            new Func<Task>(async () =>
                    await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(10), default)).ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task GetCurrencyPairs005()
        => await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddDays(-20), default)).ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task GetExchangeRate001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            (await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default) > decimal.Zero).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddDays(-20d), default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var armenia = new RegionInfo("AM");
        var belarus = new RegionInfo("BY");

        var armenianDram = new CurrencyInfo(armenia);
        var belarusianRuble = new CurrencyInfo(belarus);

        var pair = new CurrencyPair(armenianDram, belarusianRuble);

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }
}
