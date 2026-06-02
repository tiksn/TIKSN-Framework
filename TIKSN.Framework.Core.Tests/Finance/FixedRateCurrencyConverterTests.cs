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

        var initial = new Money(usDollar, amount: 100);

        var converter = CreateFixedRateCurrencyConverter(
            Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
            rate: 2m);

        var final = await converter.ConvertCurrencyAsync(initial, poundSterling, DateTime.Now,
            cancellationToken: TestContext.Current.CancellationToken);

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

        var initial = new Money(usDollar, amount: 100);

        var converter = CreateFixedRateCurrencyConverter(
            Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
            rate: 2m);

        _ = await new Func<Task>(async () =>
                await converter.ConvertCurrencyAsync(initial, euro, DateTimeOffset.Now,
                        cancellationToken: TestContext.Current.CancellationToken)
                    .ConfigureAwait(true))
            .ShouldThrowAsync<ArgumentException>();
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

        var initial = new Money(armenianDram, amount: 100);

        var converter = CreateFixedRateCurrencyConverter(
            Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
            rate: 2m);

        _ = await new Func<Task>(async () =>
                await converter.ConvertCurrencyAsync(initial, euro, DateTimeOffset.Now,
                        cancellationToken: TestContext.Current.CancellationToken)
                    .ConfigureAwait(true))
            .ShouldThrowAsync<ArgumentException>();
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

        var converter = CreateFixedRateCurrencyConverter(
            Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
            rate: 2m);

        converter.CurrencyPair.BaseCurrency.ShouldBe(usDollar);
        converter.CurrencyPair.CounterCurrency.ShouldBe(poundSterling);
        return Task.CompletedTask;
    }

    [Fact]
    public Task FixedRateCurrencyConverter001()
    {
        _ = new Func<object>(() => CreateFixedRateCurrencyConverter(pair: null, rate: 0.5m))
            .ShouldThrow<ArgumentNullException>();
        return Task.CompletedTask;
    }

    [Fact]
    public Task FixedRateCurrencyConverter002()
    {
        var unitedStates = new RegionInfo("US");
        var armenia = new RegionInfo("AM");

        var usDollar = new CurrencyInfo(unitedStates);
        var armenianDram = new CurrencyInfo(armenia);

        var pair = Helper.CurrencyPairFactory.Create(usDollar, armenianDram);

        _ = new Func<object>(() => CreateFixedRateCurrencyConverter(pair, rate: -0.5m))
            .ShouldThrow<ArgumentException>();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetCurrencyPairs001()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair = Helper.CurrencyPairFactory.Create(usDollar, poundSterling);

        var converter = CreateFixedRateCurrencyConverter(pair, rate: 2m);

        converter.CurrencyPair.BaseCurrency.ShouldBe(usDollar);
        converter.CurrencyPair.CounterCurrency.ShouldBe(poundSterling);

        ReferenceEquals(pair,
                (await converter.GetCurrencyPairsAsync(DateTimeOffset.Now,
                        cancellationToken: TestContext.Current.CancellationToken)
                    .ConfigureAwait(true)).Single())
            .ShouldBeTrue();
    }

    [Fact]
    public async Task GetExchangeRate001()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var converter = CreateFixedRateCurrencyConverter(
            Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
            rate: 2m);

        (await converter.GetExchangeRateAsync(Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
                DateTimeOffset.Now,
                cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true)).ShouldBe(2m);
    }

    [Fact]
    public async Task GetExchangeRate002()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var converter = CreateFixedRateCurrencyConverter(
            Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
            rate: 2m);

        _ = await new Func<Task>(async () =>
            await converter.GetExchangeRateAsync(Helper.CurrencyPairFactory.Create(poundSterling, usDollar),
                    DateTimeOffset.Now,
                    cancellationToken: TestContext.Current.CancellationToken)
                .ConfigureAwait(true)).ShouldThrowAsync<ArgumentException>();
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

        var converter = CreateFixedRateCurrencyConverter(
            Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
            rate: 2m);

        _ = await new Func<Task>(async () =>
            await converter.GetExchangeRateAsync(Helper.CurrencyPairFactory.Create(euro, usDollar),
                    DateTimeOffset.Now,
                    cancellationToken: TestContext.Current.CancellationToken)
                .ConfigureAwait(true)).ShouldThrowAsync<ArgumentException>();
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

        var converter = CreateFixedRateCurrencyConverter(
            Helper.CurrencyPairFactory.Create(usDollar, poundSterling),
            rate: 2m);

        _ = await new Func<Task>(async () =>
            await converter.GetExchangeRateAsync(Helper.CurrencyPairFactory.Create(usDollar, euro), DateTimeOffset.Now,
                    cancellationToken: TestContext.Current.CancellationToken)
                .ConfigureAwait(true)).ShouldThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetExchangeRate005()
    {
        var unitedStates = new RegionInfo("US");
        var unitedKingdom = new RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair = Helper.CurrencyPairFactory.Create(poundSterling, usDollar);

        var converter = CreateFixedRateCurrencyConverter(pair, rate: 1.6m);

        var lastMonth = DateTimeOffset.Now.AddMonths(-1);
        var nextMonth = DateTimeOffset.Now.AddMonths(1);

        var rateInLastMonth = await converter.GetExchangeRateAsync(pair, lastMonth,
            cancellationToken: TestContext.Current.CancellationToken);
        var rateInNextMonth = await converter.GetExchangeRateAsync(pair, nextMonth,
            cancellationToken: TestContext.Current.CancellationToken);

        (rateInLastMonth == rateInNextMonth).ShouldBeTrue();
    }

    private static FixedRateCurrencyConverter CreateFixedRateCurrencyConverter(CurrencyPair pair, decimal rate)
        => new(pair, rate, Helper.CurrencyPairFactory);
}
