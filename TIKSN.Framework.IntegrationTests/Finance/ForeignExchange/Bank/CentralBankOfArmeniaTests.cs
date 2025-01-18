using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using Xunit;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public class CentralBankOfArmeniaTests
{
    private readonly ICentralBankOfArmenia bank;
    private readonly TimeProvider timeProvider;

    public CentralBankOfArmeniaTests()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        this.timeProvider = serviceProvider.GetRequiredService<TimeProvider>();
        this.bank = serviceProvider.GetRequiredService<ICentralBankOfArmenia>();
    }

    [Fact]
    public async Task ConversionDirection001()
    {
        var armenianDram = new CurrencyInfo(new RegionInfo("AM"));
        var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

        var beforeInPound = new Money(poundSterling, 100m);

        var afterInDram = await this.bank.ConvertCurrencyAsync(beforeInPound, armenianDram, this.timeProvider.GetUtcNow(), default);

        (beforeInPound.Amount < afterInDram.Amount).ShouldBeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var initial = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default);
            var result = await this.bank.ConvertCurrencyAsync(initial, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default);

            (result.Currency == pair.CounterCurrency).ShouldBeTrue();
            (result.Amount > 0m).ShouldBeTrue();
            (result.Amount == (rate * initial.Amount)).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var dollar = new CurrencyInfo(unitedStates);
        var dram = new CurrencyInfo(armenia);

        var before = new Money(dollar, 100m);

        _ = await
                new Func<Task>(async () =>
                        await this.bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetUtcNow().AddDays(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var dollar = new CurrencyInfo(unitedStates);
        var dram = new CurrencyInfo(armenia);

        var before = new Money(dollar, 100m);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertCurrency004()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var dollar = new CurrencyInfo(unitedStates);
        var dram = new CurrencyInfo(armenia);

        var before = new Money(dollar, 100m);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetUtcNow().AddDays(-20d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Fetch001()
        => await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), default);

    [Fact]
    public async Task Fetch002()
    {
        var passed = false;

        await Task.Run(async () =>
        {
            var ci = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            _ = await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), default);

            passed = true;
        });

        passed.ShouldBeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var pairs = new System.Collections.Generic.HashSet<CurrencyPair>();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default))
        {
            _ = pairs.Add(pair);
        }

        (pairs.Count == (await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default)).Count).ShouldBeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "USD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "USD");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "GBP" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "GBP");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AUD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "AUD");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "EUR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "EUR");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "XDR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "XDR");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "IRR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "IRR");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "PLN" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "PLN");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "CAD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CAD");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "INR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "INR");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "JPY" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "JPY");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "NOK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "NOK");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "SEK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "SEK");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "CHF" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CHF");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "CZK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CZK");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "CNY" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CNY");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "SGD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "SGD");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AED" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "AED");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "KGS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "KGS");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "KZT" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "KZT");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "RUB" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "RUB");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "UAH" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "UAH");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "UZS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "UZS");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "BYN" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "BYN");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "TJS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "TJS");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "GEL" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "GEL");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "HKD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "HKD");

        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "BRL" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        currencyPairs.ShouldContain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "BRL");
    }

    [Fact]
    public async Task GetCurrencyPairs003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default);

        foreach (var pair in currencyPairs)
        {
            var reverse = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            currencyPairs.ShouldContain(c => c == reverse);
        }
    }

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
            var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            Math.Round(await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default) * await this.bank.GetExchangeRateAsync(reversePair, this.timeProvider.GetUtcNow(), default), 5).ShouldBe(decimal.One);
        }
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var dollar = new CurrencyInfo(unitedStates);
        var dram = new CurrencyInfo(armenia);

        var dollarPerDram = new CurrencyPair(dollar, dram);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetUtcNow().AddDays(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var dollar = new CurrencyInfo(unitedStates);
        var dram = new CurrencyInfo(armenia);

        var dollarPerDram = new CurrencyPair(dollar, dram);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetUtcNow().AddDays(-20d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate005()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var dollar = new CurrencyInfo(unitedStates);
        var dram = new CurrencyInfo(armenia);

        var dollarPerDram = new CurrencyPair(dollar, dram);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetUtcNow().AddMinutes(1d), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate006()
    {
        var albania = new RegionInfo("AL");
        var armenia = new RegionInfo("AM");

        var lek = new CurrencyInfo(albania);
        var dram = new CurrencyInfo(armenia);

        var lekPerDram = new CurrencyPair(lek, dram);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(lekPerDram, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate007()
    {
        var albania = new RegionInfo("AL");
        var armenia = new RegionInfo("AM");

        var lek = new CurrencyInfo(albania);
        var dram = new CurrencyInfo(armenia);

        var dramPerLek = new CurrencyPair(dram, lek);

        _ = await
            new Func<Task>(async () =>
                    await this.bank.GetExchangeRateAsync(dramPerLek, this.timeProvider.GetUtcNow(), default)).ShouldThrowAsync<ArgumentException>();
    }
}
