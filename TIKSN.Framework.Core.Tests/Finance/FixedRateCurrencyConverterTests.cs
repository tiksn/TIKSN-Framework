using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TIKSN.Finance;
using Xunit;

namespace TIKSN.Tests.Finance;

public class FixedRateCurrencyConverterTests
{
    [Fact]
    public async Task ConvertCurrency001Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var initial = new Money(usDollar, 100);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        var final = await converter.ConvertCurrencyAsync(initial, poundSterling, DateTime.Now, default).ConfigureAwait(true);

        _ = final.Currency.Should().Be(poundSterling);
        _ = final.Amount.Should().Be(200m);
    }

    [Fact]
    public async Task ConvertCurrency002Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);
        var euro = new CurrencyInfo(italy);

        var initial = new Money(usDollar, 100);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.ConvertCurrencyAsync(initial, euro, DateTimeOffset.Now, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task ConvertCurrency003Async()
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

        _ = await new Func<Task>(async () => await converter.ConvertCurrencyAsync(initial, euro, DateTimeOffset.Now, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public Task CurrencyPair001Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        _ = new CurrencyInfo(italy);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = ReferenceEquals(converter.CurrencyPair.BaseCurrency, usDollar).Should().BeTrue();
        _ = ReferenceEquals(converter.CurrencyPair.CounterCurrency, poundSterling).Should().BeTrue();
        return Task.CompletedTask;
    }

    [Fact]
    public Task FixedRateCurrencyConverter001Async()
    {
        _ = new Func<object>(() => new FixedRateCurrencyConverter(null, 0.5m)).Should().ThrowExactly<ArgumentNullException>();
        return Task.CompletedTask;
    }

    [Fact]
    public Task FixedRateCurrencyConverter002Async()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var usDollar = new CurrencyInfo(unitedStates);
        var armenianDram = new CurrencyInfo(armenia);

        var pair = new CurrencyPair(usDollar, armenianDram);

        _ = new Func<object>(() => new FixedRateCurrencyConverter(pair, -0.5m)).Should().ThrowExactly<ArgumentException>();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetCurrencyPairs001Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair = new CurrencyPair(usDollar, poundSterling);

        var converter = new FixedRateCurrencyConverter(pair, 2m);

        _ = converter.CurrencyPair.BaseCurrency.Should().Be(usDollar);
        _ = converter.CurrencyPair.CounterCurrency.Should().Be(poundSterling);

        _ = ReferenceEquals(pair, (await converter.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true)).Single()).Should().BeTrue();
    }

    [Fact]
    public async Task GetExchangeRate001Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = (await converter.GetExchangeRateAsync(new CurrencyPair(usDollar, poundSterling), DateTimeOffset.Now, default).ConfigureAwait(true)).Should().Be(2m);
    }

    [Fact]
    public async Task GetExchangeRate002Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.GetExchangeRateAsync(new CurrencyPair(poundSterling, usDollar), DateTimeOffset.Now, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate003Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);
        var euro = new CurrencyInfo(italy);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.GetExchangeRateAsync(new CurrencyPair(euro, usDollar), DateTimeOffset.Now, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate004Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");
        var italy = new RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);
        var euro = new CurrencyInfo(italy);

        var converter = new FixedRateCurrencyConverter(new CurrencyPair(usDollar, poundSterling), 2m);

        _ = await new Func<Task>(async () => await converter.GetExchangeRateAsync(new CurrencyPair(usDollar, euro), DateTimeOffset.Now, default).ConfigureAwait(true)).Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(true);
    }

    [Fact]
    public async Task GetExchangeRate005Async()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair = new CurrencyPair(poundSterling, usDollar);

        var converter = new FixedRateCurrencyConverter(pair, 1.6m);

        var lastMonth = DateTimeOffset.Now.AddMonths(-1);
        var nextMonth = DateTimeOffset.Now.AddMonths(1);

        var rateInLastMonth = await converter.GetExchangeRateAsync(pair, lastMonth, default).ConfigureAwait(true);
        var rateInNextMonth = await converter.GetExchangeRateAsync(pair, nextMonth, default).ConfigureAwait(true);

        _ = (rateInLastMonth == rateInNextMonth).Should().BeTrue();
    }
}
