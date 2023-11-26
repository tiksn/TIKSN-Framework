using System;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using TIKSN.Finance.Tests;
using Xunit;

namespace TIKSN.Finance.Cache.Tests;

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

        _ = cachedConverter.RatesCacheInterval.Should().Be(interval);
        _ = cachedConverter.CurrencyPairsCacheInterval.Should().Be(interval);

        _ = cachedConverter.RatesCacheCapacity.Should().Be(20);
        _ = cachedConverter.CurrencyPairsCacheCapacity.Should().Be(20);
        _ = cachedConverter.RatesCacheSize.Should().Be(0);
        _ = cachedConverter.CurrencyPairsCacheSize.Should().Be(0);
    }

    [Fact]
    public void CachedCurrencyConverter_002()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);

        var cachedConverter = new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval);

        _ = cachedConverter.CurrencyPairsCacheInterval.Should().Be(interval);
        _ = cachedConverter.RatesCacheInterval.Should().Be(interval);

        _ = cachedConverter.CurrencyPairsCacheCapacity.Should().BeNull();
        _ = cachedConverter.RatesCacheCapacity.Should().BeNull();

        _ = cachedConverter.RatesCacheSize.Should().Be(0);
        _ = cachedConverter.CurrencyPairsCacheSize.Should().Be(0);
    }

    [Fact]
    public void CachedCurrencyConverter_003()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);

        var cachedConverter = new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval);

        _ = cachedConverter.RatesCacheSize.Should().Be(0);
        _ = cachedConverter.CurrencyPairsCacheSize.Should().Be(0);
    }

    [Fact]
    public void CachedCurrencyConverter_004()
    {
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var capacity = 20;

        _ = new Func<object>(() => new CachedCurrencyConverter(null, fakeTimeProvider, interval, interval, capacity, capacity)).Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeCapacity001()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var negativeCapacity = -10;
        var positiveCapacity = 10;

        _ = new Func<object>(() => new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval, negativeCapacity, positiveCapacity)).Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeCapacity002()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var interval = TimeSpan.FromDays(10);
        var negativeCapacity = -10;
        var positiveCapacity = 10;

        _ = new Func<object>(() => new CachedCurrencyConverter(converter, fakeTimeProvider, interval, interval, positiveCapacity, negativeCapacity)).Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeInterval001()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var negativeInterval = TimeSpan.FromDays(-10);
        var positiveInterval = TimeSpan.FromDays(10);

        _ = new Func<object>(() => new CachedCurrencyConverter(converter, fakeTimeProvider, negativeInterval, positiveInterval)).Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CachedCurrencyConverter_NegativeInterval002()
    {
        var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));

        var negativeInterval = TimeSpan.FromDays(-10);
        var positiveInterval = TimeSpan.FromDays(10);

        _ = new Func<object>(() => new CachedCurrencyConverter(converter, fakeTimeProvider, positiveInterval, negativeInterval)).Should().ThrowExactly<ArgumentOutOfRangeException>();
    }
}
