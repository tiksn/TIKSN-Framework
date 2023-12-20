using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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

        var afterInDram = await this.bank.ConvertCurrencyAsync(beforeInPound, armenianDram, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        _ = (beforeInPound.Amount < afterInDram.Amount).Should().BeTrue();
    }

    [Fact]
    public async Task ConvertCurrency001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var initial = new Money(pair.BaseCurrency, 10m);
            var rate = await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);
            var result = await this.bank.ConvertCurrencyAsync(initial, pair.CounterCurrency, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            _ = (result.Currency == pair.CounterCurrency).Should().BeTrue();
            _ = (result.Amount > 0m).Should().BeTrue();
            _ = (result.Amount == (rate * initial.Amount)).Should().BeTrue();
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
                        await this.bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetUtcNow().AddDays(1d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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
                    await this.bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetUtcNow().AddMinutes(1d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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
                    await this.bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetUtcNow().AddDays(-20d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task Fetch001()
        => _ = await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

    [Fact]
    public async Task Fetch002()
    {
        var passed = false;

        await Task.Run(async () =>
        {
            var ci = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            _ = await this.bank.GetExchangeRatesAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

            passed = true;
        }).ConfigureAwait(true);

        _ = passed.Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var pairs = new System.Collections.Generic.HashSet<CurrencyPair>();

        foreach (var pair in await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true))
        {
            _ = pairs.Add(pair);
        }

        _ = (pairs.Count == (await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Count()).Should().BeTrue();
    }

    [Fact]
    public async Task GetCurrencyPairs002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "USD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "USD");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "GBP" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "GBP");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AUD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "AUD");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "EUR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "EUR");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "XDR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "XDR");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "IRR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "IRR");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "PLN" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "PLN");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "CAD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CAD");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "INR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "INR");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "JPY" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "JPY");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "NOK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "NOK");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "SEK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "SEK");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "CHF" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CHF");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "CZK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CZK");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "CNY" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CNY");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "SGD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "SGD");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AED" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "AED");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "KGS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "KGS");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "KZT" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "KZT");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "RUB" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "RUB");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "UAH" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "UAH");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "UZS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "UZS");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "BYN" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "BYN");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "TJS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "TJS");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "GEL" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "GEL");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "HKD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "HKD");

        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "BRL" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
        _ = currencyPairs.Should().Contain(c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "BRL");
    }

    [Fact]
    public async Task GetCurrencyPairs003()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var reverse = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            _ = currencyPairs.Should().Contain(c => c == reverse);
        }
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            _ = (await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true) > decimal.Zero).Should().BeTrue();
        }
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var currencyPairs = await this.bank.GetCurrencyPairsAsync(this.timeProvider.GetUtcNow(), default).ConfigureAwait(true);

        foreach (var pair in currencyPairs)
        {
            var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

            _ = Math.Round(await this.bank.GetExchangeRateAsync(pair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true) * await this.bank.GetExchangeRateAsync(reversePair, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true), 5).Should().Be(decimal.One);
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
                    await this.bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetUtcNow().AddDays(1d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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
                    await this.bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetUtcNow().AddDays(-20d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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
                    await this.bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetUtcNow().AddMinutes(1d), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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
                    await this.bank.GetExchangeRateAsync(lekPerDram, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
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
                    await this.bank.GetExchangeRateAsync(dramPerLek, this.timeProvider.GetUtcNow(), default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }
}
