using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Bank.IntegrationTests;

public class EuropeanCentralBankTests
{
    private readonly IEuropeanCentralBank bank;
    private readonly TimeProvider timeProvider;

    public EuropeanCentralBankTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        this.bank = serviceProvider.GetRequiredService<IEuropeanCentralBank>();
    }

    [Fact]
    public async Task Calculation001Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            Assert.True(after.Amount == before.Amount * rate);
            Assert.True(after.Currency == pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task Calculation002Async()
    {
        var oneYearsAgo = this.timeProvider.GetUtcNow().AddYears(-1);
        var pairs = await this.bank.GetCurrencyPairsAsync(oneYearsAgo, default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, oneYearsAgo, default).ConfigureAwait(true);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, oneYearsAgo, default).ConfigureAwait(true);

            Assert.True(after.Amount == before.Amount * rate);
            Assert.True(after.Currency == pair.CounterCurrency);
        }
    }

    [Fact]
    public async Task ConversionDirection001Async()
    {
        var euro = new CurrencyInfo(new RegionInfo("DE"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInEuro = new Money(euro, 100m);

        var afterInPound = await this.bank.ConvertCurrencyAsync(beforeInEuro, poundSterling, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        Assert.True(beforeInEuro.Amount > afterInPound.Amount);
    }

    [Fact]
    public async Task ConvertCurrency001Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            Assert.True(after.Amount > 0m);
        }
    }

    [Fact]
    public async Task ConvertCurrency002Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(10d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }

    [Fact]
    public async Task ConvertCurrency003Async()
    {
        var amd = new CurrencyInfo(new RegionInfo("AM"));
        var all = new CurrencyInfo(new RegionInfo("AL"));

        var before = new Money(amd, 10m);

        _ = await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await this.bank.ConvertCurrencyAsync(before, all, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs001Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            Assert.Contains(pairs, p => p == reversedPair);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs002Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);
        var uniquePairs = new HashSet<CurrencyPair>();

        foreach (var pair in pairs)
        {
            var wasUnique = uniquePairs.Add(pair);

            if (!wasUnique)
            {
                Debug.WriteLine(pair);
            }
        }

        Assert.True(uniquePairs.Count == pairs.Count());
    }

    [Fact]
    public async Task GetCurrencyPairs003Async()
    {
        _ = await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(10d), default).ConfigureAwait(true)).ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs004Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        Assert.Contains(pairs, p => p.ToString() == "AUD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "BGN/EUR");
        Assert.Contains(pairs, p => p.ToString() == "BRL/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CAD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CHF/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CNY/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CZK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "DKK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "GBP/EUR");
        Assert.Contains(pairs, p => p.ToString() == "HKD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "HUF/EUR");
        Assert.Contains(pairs, p => p.ToString() == "IDR/EUR");
        Assert.Contains(pairs, p => p.ToString() == "ILS/EUR");
        Assert.Contains(pairs, p => p.ToString() == "INR/EUR");
        Assert.Contains(pairs, p => p.ToString() == "JPY/EUR");
        Assert.Contains(pairs, p => p.ToString() == "KRW/EUR");
        Assert.Contains(pairs, p => p.ToString() == "MXN/EUR");
        Assert.Contains(pairs, p => p.ToString() == "MYR/EUR");
        Assert.Contains(pairs, p => p.ToString() == "NOK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "NZD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "PHP/EUR");
        Assert.Contains(pairs, p => p.ToString() == "PLN/EUR");
        Assert.Contains(pairs, p => p.ToString() == "RON/EUR");
        Assert.Contains(pairs, p => p.ToString() == "SEK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "SGD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "THB/EUR");
        Assert.Contains(pairs, p => p.ToString() == "TRY/EUR");
        Assert.Contains(pairs, p => p.ToString() == "USD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "ZAR/EUR");

        Assert.Contains(pairs, p => p.ToString() == "EUR/AUD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/BGN");
        Assert.Contains(pairs, p => p.ToString() == "EUR/BRL");
        Assert.Contains(pairs, p => p.ToString() == "EUR/CAD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/CHF");
        Assert.Contains(pairs, p => p.ToString() == "EUR/CNY");
        Assert.Contains(pairs, p => p.ToString() == "EUR/CZK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/DKK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/GBP");
        Assert.Contains(pairs, p => p.ToString() == "EUR/HKD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/HUF");
        Assert.Contains(pairs, p => p.ToString() == "EUR/IDR");
        Assert.Contains(pairs, p => p.ToString() == "EUR/ILS");
        Assert.Contains(pairs, p => p.ToString() == "EUR/INR");
        Assert.Contains(pairs, p => p.ToString() == "EUR/JPY");
        Assert.Contains(pairs, p => p.ToString() == "EUR/KRW");
        Assert.Contains(pairs, p => p.ToString() == "EUR/MXN");
        Assert.Contains(pairs, p => p.ToString() == "EUR/MYR");
        Assert.Contains(pairs, p => p.ToString() == "EUR/NOK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/NZD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/PHP");
        Assert.Contains(pairs, p => p.ToString() == "EUR/PLN");
        Assert.Contains(pairs, p => p.ToString() == "EUR/RON");
        Assert.Contains(pairs, p => p.ToString() == "EUR/SEK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/SGD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/THB");
        Assert.Contains(pairs, p => p.ToString() == "EUR/TRY");
        Assert.Contains(pairs, p => p.ToString() == "EUR/USD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/ZAR");
    }

    [Fact]
    public async Task GetCurrencyPairs005Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(new DateTime(2010, 1, 1), default).ConfigureAwait(true);

        Assert.Contains(pairs, p => p.ToString() == "AUD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "BGN/EUR");
        Assert.Contains(pairs, p => p.ToString() == "BRL/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CAD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CHF/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CNY/EUR");
        Assert.Contains(pairs, p => p.ToString() == "CZK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "DKK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "GBP/EUR");
        Assert.Contains(pairs, p => p.ToString() == "HKD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "HRK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "HUF/EUR");
        Assert.Contains(pairs, p => p.ToString() == "IDR/EUR");
        Assert.Contains(pairs, p => p.ToString() == "INR/EUR");
        Assert.Contains(pairs, p => p.ToString() == "JPY/EUR");
        Assert.Contains(pairs, p => p.ToString() == "KRW/EUR");
        Assert.Contains(pairs, p => p.ToString() == "LTL/EUR");
        Assert.Contains(pairs, p => p.ToString() == "MXN/EUR");
        Assert.Contains(pairs, p => p.ToString() == "MYR/EUR");
        Assert.Contains(pairs, p => p.ToString() == "NOK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "NZD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "PHP/EUR");
        Assert.Contains(pairs, p => p.ToString() == "PLN/EUR");
        Assert.Contains(pairs, p => p.ToString() == "RON/EUR");
        Assert.Contains(pairs, p => p.ToString() == "RUB/EUR");
        Assert.Contains(pairs, p => p.ToString() == "SEK/EUR");
        Assert.Contains(pairs, p => p.ToString() == "SGD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "THB/EUR");
        Assert.Contains(pairs, p => p.ToString() == "TRY/EUR");
        Assert.Contains(pairs, p => p.ToString() == "USD/EUR");
        Assert.Contains(pairs, p => p.ToString() == "ZAR/EUR");

        Assert.Contains(pairs, p => p.ToString() == "EUR/AUD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/BGN");
        Assert.Contains(pairs, p => p.ToString() == "EUR/BRL");
        Assert.Contains(pairs, p => p.ToString() == "EUR/CAD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/CHF");
        Assert.Contains(pairs, p => p.ToString() == "EUR/CNY");
        Assert.Contains(pairs, p => p.ToString() == "EUR/CZK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/DKK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/GBP");
        Assert.Contains(pairs, p => p.ToString() == "EUR/HKD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/HRK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/HUF");
        Assert.Contains(pairs, p => p.ToString() == "EUR/IDR");
        Assert.Contains(pairs, p => p.ToString() == "EUR/INR");
        Assert.Contains(pairs, p => p.ToString() == "EUR/JPY");
        Assert.Contains(pairs, p => p.ToString() == "EUR/KRW");
        Assert.Contains(pairs, p => p.ToString() == "EUR/LTL");
        Assert.Contains(pairs, p => p.ToString() == "EUR/MXN");
        Assert.Contains(pairs, p => p.ToString() == "EUR/MYR");
        Assert.Contains(pairs, p => p.ToString() == "EUR/NOK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/NZD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/PHP");
        Assert.Contains(pairs, p => p.ToString() == "EUR/PLN");
        Assert.Contains(pairs, p => p.ToString() == "EUR/RON");
        Assert.Contains(pairs, p => p.ToString() == "EUR/RUB");
        Assert.Contains(pairs, p => p.ToString() == "EUR/SEK");
        Assert.Contains(pairs, p => p.ToString() == "EUR/SGD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/THB");
        Assert.Contains(pairs, p => p.ToString() == "EUR/TRY");
        Assert.Contains(pairs, p => p.ToString() == "EUR/USD");
        Assert.Contains(pairs, p => p.ToString() == "EUR/ZAR");
    }

    [Fact]
    public async Task GetExchangeRate001Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            Assert.True(rate > 0m);
        }
    }

    [Fact]
    public async Task GetExchangeRate002Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            _ = await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(10d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }

    [Fact]
    public async Task GetExchangeRate003Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddYears(-1), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddYears(-1), default).ConfigureAwait(true);

            Assert.True(rate > 0m);
        }
    }

    [Fact]
    public async Task GetExchangeRate004Async()
    {
        var amd = new CurrencyInfo(new RegionInfo("AM"));
        var all = new CurrencyInfo(new RegionInfo("AL"));

        var pair = new CurrencyPair(amd, all);

        _ = await Assert.ThrowsAsync<ArgumentException>(
            async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).ConfigureAwait(true);
    }
}
