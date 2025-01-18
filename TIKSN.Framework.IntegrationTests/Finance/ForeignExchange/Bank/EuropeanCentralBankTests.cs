using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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
    public async Task Calculation001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            (after.Amount == before.Amount * rate).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task Calculation002()
    {
        var oneYearsAgo = this.timeProvider.GetUtcNow().AddYears(-1);
        var pairs = await this.bank.GetCurrencyPairsAsync(oneYearsAgo, default);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, oneYearsAgo, default);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, oneYearsAgo, default);

            (after.Amount == before.Amount * rate).ShouldBeTrue();
            (after.Currency == pair.CounterCurrency).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var euro = new CurrencyInfo(new RegionInfo("DE"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInEuro = new Money(euro, 100m);

        var afterInPound = await this.bank.ConvertCurrencyAsync(beforeInEuro, poundSterling, this.timeProvider.GetUtcNow(), default);

        (beforeInEuro.Amount > afterInPound.Amount).ShouldBeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);
            var after = await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            (after.Amount > 0m).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var before = new Money(pair.BaseCurrency, 10m);

            _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetUtcNow().AddMinutes(10d), default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var amd = new CurrencyInfo(new RegionInfo("AM"));
        var all = new CurrencyInfo(new RegionInfo("AL"));

        var before = new Money(amd, 10m);

        _ = await new Func<Task>(async () =>
                await this.bank.ConvertCurrencyAsync(before, all, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            pairs.ShouldContain(p => p == reversedPair);
        }
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);
        var uniquePairs = new HashSet<CurrencyPair>();

        foreach (var pair in pairs)
        {
            var wasUnique = uniquePairs.Add(pair);

            if (!wasUnique)
            {
                Debug.WriteLine(pair);
            }
        }

        (uniquePairs.Count == pairs.Count).ShouldBeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs003() =>
        await new Func<Task>(async () => await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddMinutes(10d), default)).ShouldThrowAsync<ArgumentException>();

    [Fact]
    public async Task GetCurrencyPairs004()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        pairs.ShouldContain(p => p.ToString() == "AUD/EUR");
        pairs.ShouldContain(p => p.ToString() == "BGN/EUR");
        pairs.ShouldContain(p => p.ToString() == "BRL/EUR");
        pairs.ShouldContain(p => p.ToString() == "CAD/EUR");
        pairs.ShouldContain(p => p.ToString() == "CHF/EUR");
        pairs.ShouldContain(p => p.ToString() == "CNY/EUR");
        pairs.ShouldContain(p => p.ToString() == "CZK/EUR");
        pairs.ShouldContain(p => p.ToString() == "DKK/EUR");
        pairs.ShouldContain(p => p.ToString() == "GBP/EUR");
        pairs.ShouldContain(p => p.ToString() == "HKD/EUR");
        pairs.ShouldContain(p => p.ToString() == "HUF/EUR");
        pairs.ShouldContain(p => p.ToString() == "IDR/EUR");
        pairs.ShouldContain(p => p.ToString() == "ILS/EUR");
        pairs.ShouldContain(p => p.ToString() == "INR/EUR");
        pairs.ShouldContain(p => p.ToString() == "JPY/EUR");
        pairs.ShouldContain(p => p.ToString() == "KRW/EUR");
        pairs.ShouldContain(p => p.ToString() == "MXN/EUR");
        pairs.ShouldContain(p => p.ToString() == "MYR/EUR");
        pairs.ShouldContain(p => p.ToString() == "NOK/EUR");
        pairs.ShouldContain(p => p.ToString() == "NZD/EUR");
        pairs.ShouldContain(p => p.ToString() == "PHP/EUR");
        pairs.ShouldContain(p => p.ToString() == "PLN/EUR");
        pairs.ShouldContain(p => p.ToString() == "RON/EUR");
        pairs.ShouldContain(p => p.ToString() == "SEK/EUR");
        pairs.ShouldContain(p => p.ToString() == "SGD/EUR");
        pairs.ShouldContain(p => p.ToString() == "THB/EUR");
        pairs.ShouldContain(p => p.ToString() == "TRY/EUR");
        pairs.ShouldContain(p => p.ToString() == "USD/EUR");
        pairs.ShouldContain(p => p.ToString() == "ZAR/EUR");

        pairs.ShouldContain(p => p.ToString() == "EUR/AUD");
        pairs.ShouldContain(p => p.ToString() == "EUR/BGN");
        pairs.ShouldContain(p => p.ToString() == "EUR/BRL");
        pairs.ShouldContain(p => p.ToString() == "EUR/CAD");
        pairs.ShouldContain(p => p.ToString() == "EUR/CHF");
        pairs.ShouldContain(p => p.ToString() == "EUR/CNY");
        pairs.ShouldContain(p => p.ToString() == "EUR/CZK");
        pairs.ShouldContain(p => p.ToString() == "EUR/DKK");
        pairs.ShouldContain(p => p.ToString() == "EUR/GBP");
        pairs.ShouldContain(p => p.ToString() == "EUR/HKD");
        pairs.ShouldContain(p => p.ToString() == "EUR/HUF");
        pairs.ShouldContain(p => p.ToString() == "EUR/IDR");
        pairs.ShouldContain(p => p.ToString() == "EUR/ILS");
        pairs.ShouldContain(p => p.ToString() == "EUR/INR");
        pairs.ShouldContain(p => p.ToString() == "EUR/JPY");
        pairs.ShouldContain(p => p.ToString() == "EUR/KRW");
        pairs.ShouldContain(p => p.ToString() == "EUR/MXN");
        pairs.ShouldContain(p => p.ToString() == "EUR/MYR");
        pairs.ShouldContain(p => p.ToString() == "EUR/NOK");
        pairs.ShouldContain(p => p.ToString() == "EUR/NZD");
        pairs.ShouldContain(p => p.ToString() == "EUR/PHP");
        pairs.ShouldContain(p => p.ToString() == "EUR/PLN");
        pairs.ShouldContain(p => p.ToString() == "EUR/RON");
        pairs.ShouldContain(p => p.ToString() == "EUR/SEK");
        pairs.ShouldContain(p => p.ToString() == "EUR/SGD");
        pairs.ShouldContain(p => p.ToString() == "EUR/THB");
        pairs.ShouldContain(p => p.ToString() == "EUR/TRY");
        pairs.ShouldContain(p => p.ToString() == "EUR/USD");
        pairs.ShouldContain(p => p.ToString() == "EUR/ZAR");
    }

    [Fact]
    public async Task GetCurrencyPairs005()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.Zero), default);

        pairs.ShouldContain(p => p.ToString() == "AUD/EUR");
        pairs.ShouldContain(p => p.ToString() == "BGN/EUR");
        pairs.ShouldContain(p => p.ToString() == "BRL/EUR");
        pairs.ShouldContain(p => p.ToString() == "CAD/EUR");
        pairs.ShouldContain(p => p.ToString() == "CHF/EUR");
        pairs.ShouldContain(p => p.ToString() == "CNY/EUR");
        pairs.ShouldContain(p => p.ToString() == "CZK/EUR");
        pairs.ShouldContain(p => p.ToString() == "DKK/EUR");
        pairs.ShouldContain(p => p.ToString() == "GBP/EUR");
        pairs.ShouldContain(p => p.ToString() == "HKD/EUR");
        pairs.ShouldContain(p => p.ToString() == "HRK/EUR");
        pairs.ShouldContain(p => p.ToString() == "HUF/EUR");
        pairs.ShouldContain(p => p.ToString() == "IDR/EUR");
        pairs.ShouldContain(p => p.ToString() == "INR/EUR");
        pairs.ShouldContain(p => p.ToString() == "JPY/EUR");
        pairs.ShouldContain(p => p.ToString() == "KRW/EUR");
        pairs.ShouldContain(p => p.ToString() == "LTL/EUR");
        pairs.ShouldContain(p => p.ToString() == "MXN/EUR");
        pairs.ShouldContain(p => p.ToString() == "MYR/EUR");
        pairs.ShouldContain(p => p.ToString() == "NOK/EUR");
        pairs.ShouldContain(p => p.ToString() == "NZD/EUR");
        pairs.ShouldContain(p => p.ToString() == "PHP/EUR");
        pairs.ShouldContain(p => p.ToString() == "PLN/EUR");
        pairs.ShouldContain(p => p.ToString() == "RON/EUR");
        pairs.ShouldContain(p => p.ToString() == "RUB/EUR");
        pairs.ShouldContain(p => p.ToString() == "SEK/EUR");
        pairs.ShouldContain(p => p.ToString() == "SGD/EUR");
        pairs.ShouldContain(p => p.ToString() == "THB/EUR");
        pairs.ShouldContain(p => p.ToString() == "TRY/EUR");
        pairs.ShouldContain(p => p.ToString() == "USD/EUR");
        pairs.ShouldContain(p => p.ToString() == "ZAR/EUR");

        pairs.ShouldContain(p => p.ToString() == "EUR/AUD");
        pairs.ShouldContain(p => p.ToString() == "EUR/BGN");
        pairs.ShouldContain(p => p.ToString() == "EUR/BRL");
        pairs.ShouldContain(p => p.ToString() == "EUR/CAD");
        pairs.ShouldContain(p => p.ToString() == "EUR/CHF");
        pairs.ShouldContain(p => p.ToString() == "EUR/CNY");
        pairs.ShouldContain(p => p.ToString() == "EUR/CZK");
        pairs.ShouldContain(p => p.ToString() == "EUR/DKK");
        pairs.ShouldContain(p => p.ToString() == "EUR/GBP");
        pairs.ShouldContain(p => p.ToString() == "EUR/HKD");
        pairs.ShouldContain(p => p.ToString() == "EUR/HRK");
        pairs.ShouldContain(p => p.ToString() == "EUR/HUF");
        pairs.ShouldContain(p => p.ToString() == "EUR/IDR");
        pairs.ShouldContain(p => p.ToString() == "EUR/INR");
        pairs.ShouldContain(p => p.ToString() == "EUR/JPY");
        pairs.ShouldContain(p => p.ToString() == "EUR/KRW");
        pairs.ShouldContain(p => p.ToString() == "EUR/LTL");
        pairs.ShouldContain(p => p.ToString() == "EUR/MXN");
        pairs.ShouldContain(p => p.ToString() == "EUR/MYR");
        pairs.ShouldContain(p => p.ToString() == "EUR/NOK");
        pairs.ShouldContain(p => p.ToString() == "EUR/NZD");
        pairs.ShouldContain(p => p.ToString() == "EUR/PHP");
        pairs.ShouldContain(p => p.ToString() == "EUR/PLN");
        pairs.ShouldContain(p => p.ToString() == "EUR/RON");
        pairs.ShouldContain(p => p.ToString() == "EUR/RUB");
        pairs.ShouldContain(p => p.ToString() == "EUR/SEK");
        pairs.ShouldContain(p => p.ToString() == "EUR/SGD");
        pairs.ShouldContain(p => p.ToString() == "EUR/THB");
        pairs.ShouldContain(p => p.ToString() == "EUR/TRY");
        pairs.ShouldContain(p => p.ToString() == "EUR/USD");
        pairs.ShouldContain(p => p.ToString() == "EUR/ZAR");
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);

            (rate > 0m).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in pairs)
        {
            _ = await new Func<Task>(async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddMinutes(10d), default)).ShouldThrowAsync<ArgumentException>();
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var pairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow().AddYears(-1), default);

        foreach (var pair in pairs)
        {
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow().AddYears(-1), default);

            (rate > 0m).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var amd = new CurrencyInfo(new RegionInfo("AM"));
        var all = new CurrencyInfo(new RegionInfo("AL"));

        var pair = new CurrencyPair(amd, all);

        _ = await new Func<Task>(async () =>
                await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }
}
