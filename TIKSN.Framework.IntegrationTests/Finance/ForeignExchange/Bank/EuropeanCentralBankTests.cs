using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            _ = (after.Amount == before.Amount * rate).Should().BeTrue();
            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
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

            _ = (after.Amount == before.Amount * rate).Should().BeTrue();
            _ = (after.Currency == pair.CounterCurrency).Should().BeTrue();
        }
    }

    [Fact]
    public async Task ConversionDirection001Async()
    {
        var euro = new CurrencyInfo(new RegionInfo("DE"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInEuro = new Money(euro, 100m);

        var afterInPound = await this.bank.ConvertCurrencyAsync(beforeInEuro, poundSterling, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        _ = (beforeInEuro.Amount > afterInPound.Amount).Should().BeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            _ = (after.Amount > 0m).Should().BeTrue();
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
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(10d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
        }
    }

    [Fact]
    public async Task ConvertCurrency003Async()
    {
        var amd = new CurrencyInfo(new RegionInfo("AM"));
        var all = new CurrencyInfo(new RegionInfo("AL"));

        var before = new Money(amd, 10m);

        _ = await new Func<Task>(async () =>
                await this.bank.ConvertCurrencyAsync(before, all, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetCurrencyPairs001Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            _ = pairs.Should().Contain(p => p == reversedPair);
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

        _ = (uniquePairs.Count == pairs.Count()).Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003Async() =>
        _ = await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(10d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);

    [Fact]
    public async Task GetCurrencyPairs004Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        _ = pairs.Should().Contain(p => p.ToString() == "AUD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "BGN/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "BRL/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CAD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CHF/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CNY/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CZK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "DKK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "GBP/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "HKD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "HUF/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "IDR/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "ILS/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "INR/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "JPY/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "KRW/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "MXN/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "MYR/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "NOK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "NZD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "PHP/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "PLN/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "RON/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "SEK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "SGD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "THB/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "TRY/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "USD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "ZAR/EUR");

        _ = pairs.Should().Contain(p => p.ToString() == "EUR/AUD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/BGN");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/BRL");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CAD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CHF");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CNY");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CZK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/DKK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/GBP");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/HKD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/HUF");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/IDR");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/ILS");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/INR");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/JPY");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/KRW");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/MXN");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/MYR");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/NOK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/NZD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/PHP");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/PLN");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/RON");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/SEK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/SGD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/THB");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/TRY");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/USD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/ZAR");
    }

    [Fact]
    public async Task GetCurrencyPairs005Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero), default).ConfigureAwait(true);

        _ = pairs.Should().Contain(p => p.ToString() == "AUD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "BGN/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "BRL/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CAD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CHF/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CNY/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "CZK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "DKK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "GBP/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "HKD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "HRK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "HUF/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "IDR/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "INR/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "JPY/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "KRW/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "LTL/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "MXN/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "MYR/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "NOK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "NZD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "PHP/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "PLN/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "RON/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "RUB/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "SEK/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "SGD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "THB/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "TRY/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "USD/EUR");
        _ = pairs.Should().Contain(p => p.ToString() == "ZAR/EUR");

        _ = pairs.Should().Contain(p => p.ToString() == "EUR/AUD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/BGN");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/BRL");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CAD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CHF");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CNY");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/CZK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/DKK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/GBP");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/HKD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/HRK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/HUF");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/IDR");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/INR");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/JPY");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/KRW");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/LTL");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/MXN");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/MYR");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/NOK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/NZD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/PHP");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/PLN");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/RON");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/RUB");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/SEK");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/SGD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/THB");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/TRY");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/USD");
        _ = pairs.Should().Contain(p => p.ToString() == "EUR/ZAR");
    }

    [Fact]
    public async Task GetExchangeRate001Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            _ = (rate > 0m).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            _ = await new Func<Task>(async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(10d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
        }
    }

    [Fact]
    public async Task GetExchangeRate003Async()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddYears(-1), default).ConfigureAwait(true);

        foreach (var pair in pairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddYears(-1), default).ConfigureAwait(true);

            _ = (rate > 0m).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate004Async()
    {
        var amd = new CurrencyInfo(new RegionInfo("AM"));
        var all = new CurrencyInfo(new RegionInfo("AL"));

        var pair = new CurrencyPair(amd, all);

        _ = await new Func<Task>(async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }
}
