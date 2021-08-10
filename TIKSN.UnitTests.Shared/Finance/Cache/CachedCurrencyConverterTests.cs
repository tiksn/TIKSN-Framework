using System;
using TIKSN.Finance.Tests;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.Cache.Tests
{
    public class CachedCurrencyConverterTests
    {
        [Fact]
        public void CachedCurrencyConverter_001()
        {
            var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

            var interval = TimeSpan.FromDays(10);
            var capacity = 20;

            var cachedConverter = new CachedCurrencyConverter(converter, new TimeProvider(), interval, interval, capacity, capacity);

            Assert.Equal(interval, cachedConverter.RatesCacheInterval);
            Assert.Equal(interval, cachedConverter.CurrencyPairsCacheInterval);

            Assert.Equal(20, cachedConverter.RatesCacheCapacity);
            Assert.Equal(20, cachedConverter.CurrencyPairsCacheCapacity);
            Assert.Equal(0, cachedConverter.RatesCacheSize);
            Assert.Equal(0, cachedConverter.CurrencyPairsCacheSize);
        }

        [Fact]
        public void CachedCurrencyConverter_002()
        {
            var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

            var interval = TimeSpan.FromDays(10);

            var cachedConverter = new CachedCurrencyConverter(converter, new TimeProvider(), interval, interval);

            Assert.Equal(interval, cachedConverter.CurrencyPairsCacheInterval);
            Assert.Equal(interval, cachedConverter.RatesCacheInterval);

            Assert.Null(cachedConverter.CurrencyPairsCacheCapacity);
            Assert.Null(cachedConverter.RatesCacheCapacity);

            Assert.Equal(0, cachedConverter.RatesCacheSize);
            Assert.Equal(0, cachedConverter.CurrencyPairsCacheSize);
        }

        [Fact]
        public void CachedCurrencyConverter_003()
        {
            var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

            var interval = TimeSpan.FromDays(10);

            var cachedConverter = new CachedCurrencyConverter(converter, new TimeProvider(), interval, interval);

            Assert.Equal(0, cachedConverter.RatesCacheSize);
            Assert.Equal(0, cachedConverter.CurrencyPairsCacheSize);
        }

        [Fact]
        public void CachedCurrencyConverter_004()
        {
            var interval = TimeSpan.FromDays(10);
            var capacity = 20;

            _ = Assert.Throws<ArgumentNullException>(
                () => new CachedCurrencyConverter(null, new TimeProvider(), interval, interval, capacity, capacity));
        }

        [Fact]
        public void CachedCurrencyConverter_NegativeCapacity001()
        {
            var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

            var interval = TimeSpan.FromDays(10);
            var negativeCapacity = -10;
            var positiveCapacity = 10;

            _ = Assert.Throws<ArgumentOutOfRangeException>(
                () => new CachedCurrencyConverter(converter, new TimeProvider(), interval, interval, negativeCapacity, positiveCapacity));
        }

        [Fact]
        public void CachedCurrencyConverter_NegativeCapacity002()
        {
            var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

            var interval = TimeSpan.FromDays(10);
            var negativeCapacity = -10;
            var positiveCapacity = 10;

            _ = Assert.Throws<ArgumentOutOfRangeException>(
                () => new CachedCurrencyConverter(converter, new TimeProvider(), interval, interval, positiveCapacity, negativeCapacity));
        }

        [Fact]
        public void CachedCurrencyConverter_NegativeInterval001()
        {
            var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

            var negativeInterval = TimeSpan.FromDays(-10);
            var positiveInterval = TimeSpan.FromDays(10);

            _ = Assert.Throws<ArgumentOutOfRangeException>(
                () => new CachedCurrencyConverter(converter, new TimeProvider(), negativeInterval, positiveInterval));
        }

        [Fact]
        public void CachedCurrencyConverter_NegativeInterval002()
        {
            var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

            var negativeInterval = TimeSpan.FromDays(-10);
            var positiveInterval = TimeSpan.FromDays(10);

            _ = Assert.Throws<ArgumentOutOfRangeException>(
                () => new CachedCurrencyConverter(converter, new TimeProvider(), positiveInterval, negativeInterval));
        }

        //TODO: rewrite all commented tests
        //[Fact]
        //public void Calculate001()
        //{
        //    var interval = System.TimeSpan.FromDays(10);

        // int callCount = 0;

        // var date1 = new System.DateTime(2014, 2, 1); var date2 = new System.DateTime(2014, 3, 1);

        // decimal rate1 = 2m; decimal rate2 = 3m; decimal rate0 = 4m;

        // ICurrencyConverter fakedCurrencyConverter = new
        // TIKSN.Finance.Fakes.StubICurrencyConverter() { GetExchangeRateCurrencyPairDateTime =
        // (currencyPair, asOn) => { callCount++;

        // if (asOn == date1) return rate1;

        // if (asOn == date2) return rate2;

        // return rate0; }, ConvertCurrencyMoneyCurrencyInfoDateTime = (baseMoney, counterCurrency,
        // asOn) => { callCount++;

        // if (asOn == date1) return new Money(counterCurrency, baseMoney.Amount * rate1);

        // if (asOn == date2) return new Money(counterCurrency, baseMoney.Amount * rate2);

        // return new Money(counterCurrency, baseMoney.Amount * rate0); } };

        // CachedCurrencyConverter cachedConverter = new
        // CachedCurrencyConverter(fakedCurrencyConverter, interval, interval);

        // var pair = Helper.SampleCurrencyPair1; var testBaseMoney = new Money(pair.BaseCurrency, 100m);

        // var cachedRate1 = cachedConverter.GetExchangeRate(pair, date1);

        // Assert.Equal<decimal>(rate1, cachedRate1); Assert.Equal(1, cachedConverter.RatesCacheSize);

        // var convertedMoney1 = cachedConverter.ConvertCurrency(testBaseMoney, pair.CounterCurrency, date1);

        // Assert.Equal<Money>(new Money(pair.CounterCurrency, testBaseMoney.Amount * rate1),
        // convertedMoney1); Assert.Equal(1, cachedConverter.RatesCacheSize);

        // var convertedMoney2 = cachedConverter.ConvertCurrency(testBaseMoney, pair.CounterCurrency, date2);

        // Assert.Equal<Money>(new Money(pair.CounterCurrency, testBaseMoney.Amount * rate2),
        // convertedMoney2); Assert.Equal(2, cachedConverter.RatesCacheSize);

        // var cachedRate2 = cachedConverter.GetExchangeRate(pair, date2);

        //    Assert.Equal<decimal>(rate2, cachedRate2);
        //    Assert.Equal(2, cachedConverter.RatesCacheSize);
        //}

        //[Fact]
        //public void Clear_001()
        //{
        //    var testDate = new System.DateTime(2014, 11, 30);

        // int rateCallCount = 0; int currencyPairsCallCount = 0;

        // var interval = System.TimeSpan.FromDays(10);

        // ICurrencyConverter converter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // ConvertCurrencyMoneyCurrencyInfoDateTime = (baseMoney, targetCurrency, asOn) => {
        // rateCallCount++; return new Money(targetCurrency, Helper.GetRandomForeignExchangeRate() *
        // baseMoney.Amount); },

        // GetCurrencyPairsDateTime = (asOn) => { currencyPairsCallCount++; return
        // Helper.SampleCurrencyPairs1; }, GetExchangeRateCurrencyPairDateTime = (currencyPair, asOn)
        // => { rateCallCount++; return Helper.GetRandomForeignExchangeRate(); }, };

        // var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

        // cachedConverter.GetCurrencyPairs(testDate); cachedConverter.GetCurrencyPairs(testDate);
        // Assert.Equal(1, currencyPairsCallCount);

        // cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1, testDate);
        // cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1, testDate); Assert.Equal(1, rateCallCount);

        // cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, testDate);
        // cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, testDate);
        // Assert.Equal(1, rateCallCount);

        // cachedConverter.Clear();

        // cachedConverter.GetCurrencyPairs(testDate); Assert.Equal(2, currencyPairsCallCount);

        // cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1, testDate); Assert.Equal(2, rateCallCount);

        //    cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, testDate);
        //    Assert.Equal(2, rateCallCount);
        //}

        //[Fact]
        //public void ConvertCurrency_CachePrioritization001()
        //{
        //    int capacity = 2;
        //    var interval = System.TimeSpan.FromDays(1);
        //    int rateCallCount = 0;

        // ICurrencyConverter fakedConverter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // GetExchangeRateCurrencyPairDateTime = (pair, asOn) => { rateCallCount++; return
        // Helper.GetRandomForeignExchangeRate(); }, ConvertCurrencyMoneyCurrencyInfoDateTime =
        // (money, currency, asOn) => { rateCallCount++; return new Money(currency, money.Amount *
        // Helper.GetRandomForeignExchangeRate()); } };

        // CachedCurrencyConverter cachedConverter = new CachedCurrencyConverter(fakedConverter,
        // interval, interval, capacity, null);

        // cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, new
        // System.DateTime(2014, 10, 1)); cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1,
        // new System.DateTime(2014, 9, 1));

        // Assert.Equal(2, cachedConverter.RatesCacheSize); Assert.Equal(2, rateCallCount);

        // cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, new
        // System.DateTime(2014, 8, 1));

        // Assert.Equal(2, cachedConverter.RatesCacheSize); Assert.Equal(3, rateCallCount);

        // cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, new
        // System.DateTime(2014, 9, 1)); cachedConverter.ConvertCurrency(Helper.SampleMoney1,
        // Helper.SampleCurrency2, new System.DateTime(2014, 8, 1));

        // Assert.Equal(2, cachedConverter.RatesCacheSize); Assert.Equal(3, rateCallCount);

        // cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1, new System.DateTime(2014, 11, 1));

        //    Assert.Equal(2, cachedConverter.RatesCacheSize);
        //    Assert.Equal(4, rateCallCount);
        //}

        //[Fact]
        //public void ConvertCurrency_InternalConverterCallCount001()
        //{
        //    var testBaseCurrency = Helper.SampleCurrency1;
        //    var testTargetCurrency = Helper.SampleCurrency2;
        //    var testBaseMoney = new Money(testBaseCurrency, 10.34m);

        // int callsCount = 0;

        // ICurrencyConverter converter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // ConvertCurrencyMoneyCurrencyInfoDateTime = (baseMoney, targetCurrency, asOn) => {
        // callsCount++; return new Money(targetCurrency, baseMoney.Amount *
        // Helper.GetRandomForeignExchangeRate()); } };

        // var interval = System.TimeSpan.FromDays(10);

        // var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

        // cachedConverter.ConvertCurrency(testBaseMoney, testTargetCurrency, System.DateTime.Now);
        // cachedConverter.ConvertCurrency(testBaseMoney, testTargetCurrency, System.DateTime.Now);
        // cachedConverter.ConvertCurrency(testBaseMoney, testTargetCurrency, System.DateTime.Now);

        //    Assert.Equal(1, callsCount);
        //}

        //[Fact]
        //public void ConvertCurrency_IntervalBoundery001()
        //{
        //    var testBaseCurrency = Helper.SampleCurrency1;
        //    var testBaseMoney = new Money(Helper.SampleCurrency2, 123m);

        // var currencyPair = Helper.SampleCurrencyPair1;

        // int callsCount = 0;

        // var date1 = new System.DateTime(2014, 11, 21); var date2 = new System.DateTime(2014, 11,
        // 23); var date3 = new System.DateTime(2014, 11, 25);

        // Money convertedMoney1 = new Money(Helper.SampleCurrency1, 101m); Money convertedMoney2 =
        // new Money(Helper.SampleCurrency1, 102m); Money convertedMoney3 = new
        // Money(Helper.SampleCurrency1, 103m);

        // Money convertedMoney0 = new Money(Helper.SampleCurrency1, 100m);

        // var interval = System.TimeSpan.FromDays(1);

        // ICurrencyConverter converter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // ConvertCurrencyMoneyCurrencyInfoDateTime = (baseMoney, targetCurrency, asOn) => { callsCount++;

        // if (asOn == date1) return convertedMoney1;

        // if (asOn == date2) return convertedMoney2;

        // if (asOn == date3) return convertedMoney3;

        // return convertedMoney0; } };

        // var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

        // Assert.Equal<Money>(convertedMoney1, cachedConverter.ConvertCurrency(testBaseMoney,
        // testBaseCurrency, date1)); Assert.Equal(1, callsCount);

        // Assert.Equal<Money>(convertedMoney2, cachedConverter.ConvertCurrency(testBaseMoney,
        // testBaseCurrency, date2)); Assert.Equal(2, callsCount);

        // Assert.Equal<Money>(convertedMoney3, cachedConverter.ConvertCurrency(testBaseMoney,
        // testBaseCurrency, date3)); Assert.Equal(3, callsCount);

        // Assert.Equal<Money>(convertedMoney1, cachedConverter.ConvertCurrency(testBaseMoney,
        // testBaseCurrency, new System.DateTime(2014, 11, 22))); Assert.Equal(3, callsCount);

        // Assert.Equal<Money>(convertedMoney2, cachedConverter.ConvertCurrency(testBaseMoney,
        // testBaseCurrency, new System.DateTime(2014, 11, 24))); Assert.Equal(3, callsCount);

        // Assert.Equal<Money>(convertedMoney3, cachedConverter.ConvertCurrency(testBaseMoney,
        // testBaseCurrency, new System.DateTime(2014, 11, 26))); Assert.Equal(3, callsCount);

        //    Assert.Equal<Money>(convertedMoney0, cachedConverter.ConvertCurrency(testBaseMoney, testBaseCurrency, new System.DateTime(2014, 11, 27)));
        //    Assert.Equal(4, callsCount);
        //}

        //[Fact]
        //public void ConvertCurrency_Zero001()
        //{
        //    int capacity = 2;
        //    var interval = System.TimeSpan.FromDays(1);
        //    int rateCallCount = 0;
        //    var zero = new Money(Helper.SampleCurrency1, decimal.Zero);

        // ICurrencyConverter fakedConverter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // GetExchangeRateCurrencyPairDateTime = (pair, asOn) => { rateCallCount++; return
        // Helper.GetRandomForeignExchangeRate(); }, ConvertCurrencyMoneyCurrencyInfoDateTime =
        // (money, currency, asOn) => { rateCallCount++; return new Money(currency, money.Amount *
        // Helper.GetRandomForeignExchangeRate()); } };

        // CachedCurrencyConverter cachedConverter = new CachedCurrencyConverter(fakedConverter,
        // interval, interval, capacity, null);

        // var converted = cachedConverter.ConvertCurrency(zero, Helper.SampleCurrency2, System.DateTime.Now);

        //    Assert.Equal<Money>(new Money(Helper.SampleCurrency2, decimal.Zero), converted);
        //    Assert.Equal(0, cachedConverter.RatesCacheSize);
        //}

        //[Fact]
        //public void GetCurrencyPairs_CachePrioritization001()
        //{
        //    int pairsCallCount = 0;

        // var interval = System.TimeSpan.FromDays(1); int capacity = 2;

        // ICurrencyConverter fakedConverter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // GetCurrencyPairsDateTime = (asOn) => { pairsCallCount++; return
        // Helper.SampleCurrencyPairs1; } };

        // CachedCurrencyConverter cachedConverter = new CachedCurrencyConverter(fakedConverter,
        // interval, interval, null, capacity);

        // cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 10, 1));
        // cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 9, 1));

        // Assert.Equal(2, cachedConverter.CurrencyPairsCacheSize); Assert.Equal(2, pairsCallCount);

        // cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 8, 1));

        // Assert.Equal(2, cachedConverter.CurrencyPairsCacheSize); Assert.Equal(3, pairsCallCount);

        // cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 9, 1));
        // cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 8, 1));

        // Assert.Equal(2, cachedConverter.CurrencyPairsCacheSize); Assert.Equal(3, pairsCallCount);

        // cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 11, 1));

        //    Assert.Equal(2, cachedConverter.CurrencyPairsCacheSize);
        //    Assert.Equal(4, pairsCallCount);
        //}

        //[Fact]
        //public void GetCurrencyPairs_InternalConverterCallCount001()
        //{
        //    int callsCount = 0;

        // ICurrencyConverter converter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // GetCurrencyPairsDateTime = (asOn) => { callsCount++; return Helper.SampleCurrencyPairs1; } };

        // var interval = System.TimeSpan.FromDays(10);

        // var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

        // cachedConverter.GetCurrencyPairs(System.DateTime.Now);
        // cachedConverter.GetCurrencyPairs(System.DateTime.Now); cachedConverter.GetCurrencyPairs(System.DateTime.Now);

        //    Assert.Equal(1, callsCount);
        //}

        //[Fact]
        //public void GetCurrencyPairs_IntervalBoundery001()
        //{
        //    int callsCount = 0;

        // var date1 = new System.DateTime(2014, 11, 21); var date2 = new System.DateTime(2014, 11, 23);

        // var pair1 = Helper.SampleCurrencyPairs1; var pair2 = Helper.SampleCurrencyPairs2;

        // var pair0 = Helper.SampleCurrencyPairs3;

        // var interval = System.TimeSpan.FromDays(1);

        // ICurrencyConverter converter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // GetCurrencyPairsDateTime = (asOn) => { callsCount++;

        // if (asOn == date1) return pair1;

        // if (asOn == date2) return pair2;

        // return pair0; } };

        // var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

        // Assert.True(pair1.SequenceEqual(cachedConverter.GetCurrencyPairs(date1))); Assert.Equal(1, callsCount);

        // Assert.True(pair2.SequenceEqual(cachedConverter.GetCurrencyPairs(date2))); Assert.Equal(2, callsCount);

        // Assert.True(pair1.SequenceEqual(cachedConverter.GetCurrencyPairs(new System.DateTime(2014,
        // 11, 22)))); Assert.Equal(2, callsCount);

        // Assert.True(pair2.SequenceEqual(cachedConverter.GetCurrencyPairs(new System.DateTime(2014,
        // 11, 24)))); Assert.Equal(2, callsCount);

        //    Assert.True(pair0.SequenceEqual(cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 11, 25))));
        //    Assert.Equal(3, callsCount);
        //}

        //TODO: rewrite
        //[Fact]
        //public void GetExchangeRate_CachePrioritization001()
        //{
        //	int capacity = 2;
        //	var interval = System.TimeSpan.FromDays(1);
        //	int rateCallCount = 0;

        // ICurrencyConverter fakedConverter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // GetExchangeRateCurrencyPairDateTime = (pair, asOn) => { rateCallCount++; return
        // Helper.GetRandomForeignExchangeRate(); }, ConvertCurrencyMoneyCurrencyInfoDateTime =
        // (money, currency, asOn) => { rateCallCount++; return new Money(currency, money.Amount *
        // Helper.GetRandomForeignExchangeRate()); } };

        // CachedCurrencyConverter cachedConverter = new CachedCurrencyConverter(fakedConverter,
        // interval, interval, capacity, null);

        // var testPair = Helper.SampleCurrencyPair1; var testMoney = new
        // Money(testPair.BaseCurrency, 111m);

        // cachedConverter.GetExchangeRate(testPair, new System.DateTime(2014, 10, 1));
        // cachedConverter.GetExchangeRate(testPair, new System.DateTime(2014, 9, 1));

        // Assert.Equal(2, cachedConverter.RatesCacheSize); Assert.Equal(2, rateCallCount);

        // cachedConverter.ConvertCurrency(testMoney, testPair.CounterCurrency, new
        // System.DateTime(2014, 8, 1));

        // Assert.Equal(2, cachedConverter.RatesCacheSize); Assert.Equal(3, rateCallCount);

        // cachedConverter.GetExchangeRate(testPair, new System.DateTime(2014, 8, 1));
        // cachedConverter.GetExchangeRate(testPair, new System.DateTime(2014, 9, 1));

        // Assert.Equal(2, cachedConverter.RatesCacheSize); Assert.Equal(3, rateCallCount);

        // cachedConverter.GetExchangeRate(testPair, new System.DateTime(2014, 11, 1));

        //	Assert.Equal(2, cachedConverter.RatesCacheSize);
        //	Assert.Equal(4, rateCallCount);
        //}

        //TODO: Rewrite
        //[Fact]
        //public void GetExchangeRate_InternalConverterCallCount001()
        //{
        //	var currencyPair = Helper.SampleCurrencyPair1;

        // int callsCount = 0;

        // ICurrencyConverter converter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // GetExchangeRateCurrencyPairDateTime = (pair, dateTime) => { callsCount++; return
        // Helper.GetRandomForeignExchangeRate(); } };

        // var interval = System.TimeSpan.FromDays(10);

        // var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

        // cachedConverter.GetExchangeRate(currencyPair, System.DateTime.Now);
        // cachedConverter.GetExchangeRate(currencyPair, System.DateTime.Now);
        // cachedConverter.GetExchangeRate(currencyPair, System.DateTime.Now);

        //	Assert.Equal(1, callsCount);
        //}

        //TODO: Rewrite
        //[Fact]
        //public void GetExchangeRate_IntervalBoundery001()
        //{
        //	var currencyPair = Helper.SampleCurrencyPair1;

        // int callsCount = 0;

        // var date1 = new System.DateTime(2014, 11, 21); var date2 = new System.DateTime(2014, 11,
        // 23); var date3 = new System.DateTime(2014, 11, 25);

        // decimal rate1 = 1.11m; decimal rate2 = 1.12m; decimal rate3 = 1.13m;

        // decimal rate0 = 1.01m;

        // var interval = System.TimeSpan.FromDays(1);

        // ICurrencyConverter converter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
        // GetExchangeRateCurrencyPairDateTime = (pair, asOn) => { callsCount++;

        // if (asOn == date1) return rate1;

        // if (asOn == date2) return rate2;

        // if (asOn == date3) return rate3;

        // return rate0; } };

        // var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

        // Assert.Equal<decimal>(rate1, cachedConverter.GetExchangeRate(currencyPair, date1));
        // Assert.Equal(1, callsCount);

        // Assert.Equal<decimal>(rate2, cachedConverter.GetExchangeRate(currencyPair, date2));
        // Assert.Equal(2, callsCount);

        // Assert.Equal<decimal>(rate3, cachedConverter.GetExchangeRate(currencyPair, date3));
        // Assert.Equal(3, callsCount);

        // Assert.Equal<decimal>(rate1, cachedConverter.GetExchangeRate(currencyPair, new
        // System.DateTime(2014, 11, 22))); Assert.Equal(3, callsCount);

        // Assert.Equal<decimal>(rate2, cachedConverter.GetExchangeRate(currencyPair, new
        // System.DateTime(2014, 11, 24))); Assert.Equal(3, callsCount);

        // Assert.Equal<decimal>(rate3, cachedConverter.GetExchangeRate(currencyPair, new
        // System.DateTime(2014, 11, 26))); Assert.Equal(3, callsCount);

        //	Assert.Equal<decimal>(rate0, cachedConverter.GetExchangeRate(currencyPair, new System.DateTime(2014, 11, 27)));
        //	Assert.Equal(4, callsCount);
        //}
    }
}
