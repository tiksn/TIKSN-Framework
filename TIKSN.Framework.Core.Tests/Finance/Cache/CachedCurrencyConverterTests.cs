using System;
using Microsoft.Extensions.Time.Testing;
using Shouldly;
using TIKSN.Finance;
using TIKSN.Finance.Cache;
using Xunit;

namespace TIKSN.Tests.Finance.Cache;

public class CachedCurrencyConverterTests
{
    [Fact]
    public void CachedCurrencyConverter_001()
    {
        var converter =
            CreateFixedRateCurrencyConverter(Helper.SampleCurrencyPair1);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2020, month: 12, day: 31, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var capacity = 20;

        var cachedConverter =
            CreateCachedCurrencyConverter(converter, fakeTimeProvider, interval, interval, capacity, capacity);

        cachedConverter.RatesCacheInterval.ShouldBe(interval);
        cachedConverter.CurrencyPairsCacheInterval.ShouldBe(interval);

        cachedConverter.RatesCacheCapacity.ShouldBe(20);
        cachedConverter.CurrencyPairsCacheCapacity.ShouldBe(20);
        cachedConverter.RatesCacheSize.ShouldBe(0);
        cachedConverter.CurrencyPairsCacheSize.ShouldBe(0);
    }

    [Fact]
    public void CachedCurrencyConverter_002()
    {
        var converter =
            CreateFixedRateCurrencyConverter(Helper.SampleCurrencyPair1);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2020, month: 12, day: 31, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);

        var cachedConverter = CreateCachedCurrencyConverter(converter, fakeTimeProvider, interval, interval);

        cachedConverter.CurrencyPairsCacheInterval.ShouldBe(interval);
        cachedConverter.RatesCacheInterval.ShouldBe(interval);

        cachedConverter.CurrencyPairsCacheCapacity.ShouldBeNull();
        cachedConverter.RatesCacheCapacity.ShouldBeNull();

        cachedConverter.RatesCacheSize.ShouldBe(0);
        cachedConverter.CurrencyPairsCacheSize.ShouldBe(0);
    }

    [Fact]
    public void CachedCurrencyConverter_003()
    {
        var converter =
            CreateFixedRateCurrencyConverter(Helper.SampleCurrencyPair1);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2020, month: 12, day: 31, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);

        var cachedConverter = CreateCachedCurrencyConverter(converter, fakeTimeProvider, interval, interval);

        cachedConverter.RatesCacheSize.ShouldBe(0);
        cachedConverter.CurrencyPairsCacheSize.ShouldBe(0);
    }

    [Fact]
    public void CachedCurrencyConverter_004()
    {
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2020, month: 12, day: 31, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var capacity = 20;

        _ = new Func<object>(() =>
                new CachedCurrencyConverter(null, Helper.CurrencyPairFactory, fakeTimeProvider, interval, interval,
                    capacity,
                    capacity))
            .ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeCapacity001()
    {
        var converter =
            CreateFixedRateCurrencyConverter(Helper.SampleCurrencyPair1);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2020, month: 12, day: 31, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var negativeCapacity = -10;
        var positiveCapacity = 10;

        _ = new Func<object>(() =>
            CreateCachedCurrencyConverter(converter, fakeTimeProvider, interval, interval, negativeCapacity,
                positiveCapacity)).ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeCapacity002()
    {
        var converter =
            CreateFixedRateCurrencyConverter(Helper.SampleCurrencyPair1);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2020, month: 12, day: 31, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var negativeCapacity = -10;
        var positiveCapacity = 10;

        _ = new Func<object>(() =>
            CreateCachedCurrencyConverter(converter, fakeTimeProvider, interval, interval, positiveCapacity,
                negativeCapacity)).ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeInterval001()
    {
        var converter =
            CreateFixedRateCurrencyConverter(Helper.SampleCurrencyPair1);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2020, month: 12, day: 31, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));

        var negativeInterval = TimeSpan.FromDays(-10);
        var positiveInterval = TimeSpan.FromDays(10);

        _ = new Func<object>(() =>
                CreateCachedCurrencyConverter(converter, fakeTimeProvider, negativeInterval, positiveInterval))
            .ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeInterval002()
    {
        var converter =
            CreateFixedRateCurrencyConverter(Helper.SampleCurrencyPair1);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2020, month: 12, day: 31, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));

        var negativeInterval = TimeSpan.FromDays(-10);
        var positiveInterval = TimeSpan.FromDays(10);

        _ = new Func<object>(() =>
                CreateCachedCurrencyConverter(converter, fakeTimeProvider, positiveInterval, negativeInterval))
            .ShouldThrow<ArgumentOutOfRangeException>();
    }

    private static CachedCurrencyConverter CreateCachedCurrencyConverter(
        ICurrencyConverter converter,
        TimeProvider timeProvider,
        TimeSpan ratesCacheInterval,
        TimeSpan currencyPairsCacheInterval,
        int? ratesCacheCapacity = null,
        int? currencyPairsCacheCapacity = null)
        => new(
            converter,
            Helper.CurrencyPairFactory,
            timeProvider,
            ratesCacheInterval,
            currencyPairsCacheInterval,
            ratesCacheCapacity,
            currencyPairsCacheCapacity);

    private static FixedRateCurrencyConverter CreateFixedRateCurrencyConverter(CurrencyPair pair)
        => new(pair, Helper.GetRandomForeignExchangeRate(), Helper.CurrencyPairFactory);
}
