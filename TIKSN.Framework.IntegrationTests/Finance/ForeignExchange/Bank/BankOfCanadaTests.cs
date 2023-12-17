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
        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            _ = (after.Amount == rate * before.Amount).Should().BeTrue();
            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
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
            default).ConfigureAwait(true);

        _ = (beforeInPound.Amount < afterInDollar.Amount).Should().BeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, decimal.One);

            var after = await this.bank.ConvertCurrencyAsync(
                before,
                pair.CounterCurrency,
                this.timeProvider.GetUtcNow(),
                default).ConfigureAwait(true);

            _ = (after.Amount > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, decimal.One);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
            _ = (after.Amount == rate * before.Amount).Should().BeTrue();
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

        _ = await new Func<Task>(async () => await this.bank.ConvertCurrencyAsync(before, cad, this.timeProvider.GetUtcNow().AddMinutes(1d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task ConvertCurrency006()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var before = new Money(aoa, 100m);

        _ = await new Func<Task>(async () => await this.bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task CurrencyPairs001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default).ConfigureAwait(true);

        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/USD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/AUD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/BRL");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/CNY");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/EUR");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/HKD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/INR");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/IDR");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/JPY");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/MXN");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/NZD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/NOK");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/PEN");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/RUB");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/SGD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/ZAR");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/KRW");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/SEK");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/CHF");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/TWD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/TRY");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CAD/GBP");

        _ = currencyPairs.Should().Contain(c => c.ToString() == "USD/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "AUD/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "BRL/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CNY/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "EUR/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "HKD/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "INR/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "IDR/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "JPY/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "MXN/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "NZD/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "NOK/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "PEN/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "RUB/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "SGD/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "ZAR/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "KRW/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "SEK/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "CHF/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "TWD/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "TRY/CAD");
        _ = currencyPairs.Should().Contain(c => c.ToString() == "GBP/CAD");
    }

    [Fact]
    public async Task CurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            _ = currencyPairs.Should().Contain(c => c == reversePair);
        }
    }

    [Fact]
    public async Task CurrencyPairs003()
    {
        var pairSet = new HashSet<CurrencyPair>();

        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            _ = pairSet.Add(pair);
        }

        _ = (pairSet.Count == currencyPairs.Count()).Should().BeTrue();
    }

    [Fact]
    public async Task CurrencyPairs005()
        => _ = await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddDays(10), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);

    [Fact]
    public async Task Fetch001()
        => _ = await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

    [Fact]
    public async Task GetExchangeRate001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(
            this.timeProvider.GetUtcNow(),
            default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            _ = (rate > decimal.Zero).Should().BeTrue();
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

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(1d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var ao = new RegionInfo("AO");
        var bw = new RegionInfo("BW");

        var aoa = new CurrencyInfo(ao);
        var bwp = new CurrencyInfo(bw);

        var pair = new CurrencyPair(bwp, aoa);

        _ = await new Func<Task>(async () => await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }
}
