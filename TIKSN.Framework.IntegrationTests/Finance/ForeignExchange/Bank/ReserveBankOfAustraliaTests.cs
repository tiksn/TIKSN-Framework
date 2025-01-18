using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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

        _ = (beforeInPound.Amount < afterInDollar.Amount).Should().BeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
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
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            _ = (after.Amount == before.Amount * rate).Should().BeTrue();
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
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();
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
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddDays(-20d), default)).Should().ThrowExactlyAsync<ArgumentException>();
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
                        await this.bank.ConvertCurrencyAsync(before, belarusianRuble, this.timeProvider.GetUtcNow(), default)).Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task Fetch001()
    {
        var rates = await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), default);

        _ = rates.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        _ = currencyPairs.Should().Contain(p => p.ToString() == "USD/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "CNY/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "JPY/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "EUR/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "KRW/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "GBP/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "SGD/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "INR/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "THB/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "NZD/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "TWD/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "MYR/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "IDR/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "VND/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "HKD/AUD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "XDR/AUD");

        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/USD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/CNY");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/JPY");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/EUR");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/KRW");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/GBP");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/SGD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/INR");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/THB");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/NZD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/TWD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/MYR");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/IDR");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/VND");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/HKD");
        _ = currencyPairs.Should().Contain(p => p.ToString() == "AUD/XDR");
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            _ = currencyPairs.Should().Contain(p => p == reversedPair);
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

        _ = (pairSet.Count == currencyPairs.Count).Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs004()
        => _ = await
            new Func<Task>(async () =>
                    await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(10), default)).Should().ThrowExactlyAsync<ArgumentException>();

    [Fact]
    public async Task GetCurrencyPairs005()
        => _ = await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddDays(-20), default)).Should().ThrowExactlyAsync<ArgumentException>();

    [Fact]
    public async Task GetExchangeRate001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            _ = (await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default) > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).Should().ThrowExactlyAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddDays(-20d), default)).Should().ThrowExactlyAsync<ArgumentException>();
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

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default)).Should().ThrowExactlyAsync<ArgumentException>();
    }
}
