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
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var capacity = 20;

        var cachedConverter = new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval, capacity, capacity);

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
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);

        var cachedConverter = new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval);

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
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);

        var cachedConverter = new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval);

        cachedConverter.RatesCacheSize.ShouldBe(0);
        cachedConverter.CurrencyPairsCacheSize.ShouldBe(0);
    }

    [Fact]
    public void CachedCurrencyConverter_004()
    {
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var capacity = 20;

        _ = new Func<object>(() => new CachedCurrencyConverter(null, fakeTimeProvider, interval, interval, capacity, capacity)).ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeCapacity001()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var negativeCapacity = -10;
        var positiveCapacity = 10;

        _ = new Func<object>(() => new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval, negativeCapacity, positiveCapacity)).ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeCapacity002()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var negativeCapacity = -10;
        var positiveCapacity = 10;

        _ = new Func<object>(() => new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval, positiveCapacity, negativeCapacity)).ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeInterval001()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var negativeInterval = TimeSpan.FromDays(-10);
        var positiveInterval = TimeSpan.FromDays(10);

        _ = new Func<object>(() => new CachedCurrencyConverter(converter, fakeTimeProvider, negativeInterval, positiveInterval)).ShouldThrow<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeInterval002()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var negativeInterval = TimeSpan.FromDays(-10);
        var positiveInterval = TimeSpan.FromDays(10);

        _ = new Func<object>(() => new CachedCurrencyConverter(converter, fakeTimeProvider, positiveInterval, negativeInterval)).ShouldThrow<ArgumentOutOfRangeException>();
    }
}
