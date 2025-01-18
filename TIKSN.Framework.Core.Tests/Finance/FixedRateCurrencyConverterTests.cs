using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using TIKSN.Finance;
using Xunit;

namespace TIKSN.Tests.Finance;

public class FixedRateCurrencyConverterTests
{
    [Fact]
    public async Task ConvertCurrency001()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var initial = new Money(usDollar, 100);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        var final = await converter.ConvertCurrencyAsync(initial, poundSterling, DateTime.Now, default);

        final.Currency.ShouldBe(poundSterling);
        final.Amount.ShouldBe(200m);
    }

    [Fact]
    public async Task ConvertCurrency002()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);
        var euro = new CurrencyInfo(italy);

        var initial = new Money(usDollar, 100);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.ConvertCurrencyAsync(initial, euro, DateTimeOffset.Now, default).ConfigureAwait(true)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertCurrency003()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");
        var armenia = new RegionInfo("AM");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);
        var euro = new CurrencyInfo(italy);
        var armenianDram = new CurrencyInfo(armenia);

        var initial = new Money(armenianDram, 100);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.ConvertCurrencyAsync(initial, euro, DateTimeOffset.Now, default).ConfigureAwait(true)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public Task CurrencyPair001()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        _ = new CurrencyInfo(italy);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        ReferenceEquals(converter.CurrencyPair.BaseCurrency, usDollar).ShouldBeTrue();
        ReferenceEquals(converter.CurrencyPair.CounterCurrency, poundSterling).ShouldBeTrue();
        return Task.CompletedTask;
    }

    [Fact]
    public Task FixedRateCurrencyConverter001()
    {
        _ = new Func<object>(() => new FixedRateCurrencyConverter(null, 0.5m)).ShouldThrow<ArgumentNullException>();
        return Task.CompletedTask;
    }

    [Fact]
    public Task FixedRateCurrencyConverter002()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var usDollar = new CurrencyInfo(unitedStates);
        var armenianDram = new CurrencyInfo(armenia);

        var pair = new CurrencyPair(usDollar, armenianDram);

        _ = new Func<object>(() => new FixedRateCurrencyConverter(pair, -0.5m)).ShouldThrow<ArgumentException>();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair = new CurrencyPair(usDollar, poundSterling);

        var converter = new FixedRateCurrencyConverter(pair, 2m);

        converter.CurrencyPair.BaseCurrency.ShouldBe(usDollar);
        converter.CurrencyPair.CounterCurrency.ShouldBe(poundSterling);

        ReferenceEquals(pair, (await converter.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true)).Single()).ShouldBeTrue();
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        (await converter.GetExchangeRateAsync(new CurrencyPair(usDollar, poundSterling), DateTimeOffset.Now, default).ConfigureAwait(true)).ShouldBe(2m);
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.GetExchangeRateAsync(new CurrencyPair(poundSterling, usDollar), DateTimeOffset.Now, default).ConfigureAwait(true)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate003()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);
        var euro = new CurrencyInfo(italy);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.GetExchangeRateAsync(new CurrencyPair(euro, usDollar), DateTimeOffset.Now, default).ConfigureAwait(true)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate004()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);
        var euro = new CurrencyInfo(italy);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.GetExchangeRateAsync(new CurrencyPair(usDollar, euro), DateTimeOffset.Now, default).ConfigureAwait(true)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate005()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair = new CurrencyPair(poundSterling, usDollar);

        var converter = new FixedRateCurrencyConverter(pair, 1.6m);

        var lastMonth = DateTimeOffset.Now.AddMonths(-1);
        var nextMonth = DateTimeOffset.Now.AddMonths(1);

        var rateInLastMonth = await converter.GetExchangeRateAsync(pair, lastMonth, default);
        var rateInNextMonth = await converter.GetExchangeRateAsync(pair, nextMonth, default);

        (rateInLastMonth == rateInNextMonth).ShouldBeTrue();
    }
}
