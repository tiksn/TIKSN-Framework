namespace TIKSN.Finance.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class CachedCurrencyConverterTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CachedCurrencyConverter_001()
		{
			var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

			var interval = System.TimeSpan.FromDays(10);
			int capacity = 20;

			var cachedConverter = new CachedCurrencyConverter(converter, interval, interval, capacity, capacity);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<System.TimeSpan>(interval, cachedConverter.RatesCacheInterval);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<System.TimeSpan>(interval, cachedConverter.CurrencyPairsCacheInterval);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int?>(20, cachedConverter.RatesCacheCapacity);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int?>(20, cachedConverter.CurrencyPairsCacheCapacity);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int?>(0, cachedConverter.RatesCacheSize);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int?>(0, cachedConverter.CurrencyPairsCacheSize);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CachedCurrencyConverter_002()
		{
			var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

			var interval = System.TimeSpan.FromDays(10);

			var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<System.TimeSpan>(interval, cachedConverter.CurrencyPairsCacheInterval);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<System.TimeSpan>(interval, cachedConverter.RatesCacheInterval);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int?>(null, cachedConverter.CurrencyPairsCacheCapacity);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int?>(null, cachedConverter.RatesCacheCapacity);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int?>(0, cachedConverter.RatesCacheSize);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int?>(0, cachedConverter.CurrencyPairsCacheSize);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CachedCurrencyConverter_003()
		{
			var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

			var interval = System.TimeSpan.FromDays(10);

			var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(0, cachedConverter.RatesCacheSize);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(0, cachedConverter.CurrencyPairsCacheSize);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		[Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(System.ArgumentNullException))]
		public void CachedCurrencyConverter_004()
		{
			var interval = System.TimeSpan.FromDays(10);
			int capacity = 20;

			var cachedConverter = new CachedCurrencyConverter(null, interval, interval, capacity, capacity);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		[Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void CachedCurrencyConverter_NegativeCapacity001()
		{
			var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

			var interval = System.TimeSpan.FromDays(10);
			int negativeCapacity = -10;
			int positiveCapacity = 10;

			var cachedConverter = new CachedCurrencyConverter(converter, interval, interval, negativeCapacity, positiveCapacity);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		[Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void CachedCurrencyConverter_NegativeCapacity002()
		{
			var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

			var interval = System.TimeSpan.FromDays(10);
			int negativeCapacity = -10;
			int positiveCapacity = 10;

			var cachedConverter = new CachedCurrencyConverter(converter, interval, interval, positiveCapacity, negativeCapacity);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		[Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void CachedCurrencyConverter_NegativeInterval001()
		{
			var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

			var negativeInterval = System.TimeSpan.FromDays(-10);
			var positiveInterval = System.TimeSpan.FromDays(10);

			var cachedConverter = new CachedCurrencyConverter(converter, negativeInterval, positiveInterval);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		[Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void CachedCurrencyConverter_NegativeInterval002()
		{
			var converter = new FixedRateCurrencyConverter(Helper.SampleCurrencyPair1, Helper.GetRandomForeignExchangeRate());

			var negativeInterval = System.TimeSpan.FromDays(-10);
			var positiveInterval = System.TimeSpan.FromDays(10);

			var cachedConverter = new CachedCurrencyConverter(converter, positiveInterval, negativeInterval);
		}

		//TODO: rewrite all commented tests
		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate1, cachedRate1);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, cachedConverter.RatesCacheSize);

		// var convertedMoney1 = cachedConverter.ConvertCurrency(testBaseMoney, pair.CounterCurrency, date1);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(new
		// Money(pair.CounterCurrency, testBaseMoney.Amount * rate1), convertedMoney1);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, cachedConverter.RatesCacheSize);

		// var convertedMoney2 = cachedConverter.ConvertCurrency(testBaseMoney, pair.CounterCurrency, date2);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(new
		// Money(pair.CounterCurrency, testBaseMoney.Amount * rate2), convertedMoney2);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, cachedConverter.RatesCacheSize);

		// var cachedRate2 = cachedConverter.GetExchangeRate(pair, date2);

		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate2, cachedRate2);
		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, cachedConverter.RatesCacheSize);
		//}

		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, currencyPairsCallCount);

		// cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1, testDate);
		// cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1, testDate);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, rateCallCount);

		// cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, testDate);
		// cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, testDate);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, rateCallCount);

		// cachedConverter.Clear();

		// cachedConverter.GetCurrencyPairs(testDate);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, currencyPairsCallCount);

		// cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1, testDate);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, rateCallCount);

		//    cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, testDate);
		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, rateCallCount);
		//}

		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.RatesCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, rateCallCount);

		// cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, new
		// System.DateTime(2014, 8, 1));

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.RatesCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, rateCallCount);

		// cachedConverter.ConvertCurrency(Helper.SampleMoney1, Helper.SampleCurrency2, new
		// System.DateTime(2014, 9, 1)); cachedConverter.ConvertCurrency(Helper.SampleMoney1,
		// Helper.SampleCurrency2, new System.DateTime(2014, 8, 1));

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.RatesCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, rateCallCount);

		// cachedConverter.GetExchangeRate(Helper.SampleCurrencyPair1, new System.DateTime(2014, 11, 1));

		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, cachedConverter.RatesCacheSize);
		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(4, rateCallCount);
		//}

		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, callsCount);
		//}

		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(convertedMoney1,
		// cachedConverter.ConvertCurrency(testBaseMoney, testBaseCurrency, date1));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(convertedMoney2,
		// cachedConverter.ConvertCurrency(testBaseMoney, testBaseCurrency, date2));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(convertedMoney3,
		// cachedConverter.ConvertCurrency(testBaseMoney, testBaseCurrency, date3));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(convertedMoney1,
		// cachedConverter.ConvertCurrency(testBaseMoney, testBaseCurrency, new System.DateTime(2014,
		// 11, 22))); Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(convertedMoney2,
		// cachedConverter.ConvertCurrency(testBaseMoney, testBaseCurrency, new System.DateTime(2014,
		// 11, 24))); Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(convertedMoney3,
		// cachedConverter.ConvertCurrency(testBaseMoney, testBaseCurrency, new System.DateTime(2014,
		// 11, 26))); Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);

		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(convertedMoney0, cachedConverter.ConvertCurrency(testBaseMoney, testBaseCurrency, new System.DateTime(2014, 11, 27)));
		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(4, callsCount);
		//}

		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Money>(new Money(Helper.SampleCurrency2, decimal.Zero), converted);
		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(0, cachedConverter.RatesCacheSize);
		//}

		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.CurrencyPairsCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, pairsCallCount);

		// cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 8, 1));

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.CurrencyPairsCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, pairsCallCount);

		// cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 9, 1));
		// cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 8, 1));

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.CurrencyPairsCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, pairsCallCount);

		// cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 11, 1));

		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, cachedConverter.CurrencyPairsCacheSize);
		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(4, pairsCallCount);
		//}

		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		//public void GetCurrencyPairs_InternalConverterCallCount001()
		//{
		//    int callsCount = 0;

		// ICurrencyConverter converter = new TIKSN.Finance.Fakes.StubICurrencyConverter() {
		// GetCurrencyPairsDateTime = (asOn) => { callsCount++; return Helper.SampleCurrencyPairs1; } };

		// var interval = System.TimeSpan.FromDays(10);

		// var cachedConverter = new CachedCurrencyConverter(converter, interval, interval);

		// cachedConverter.GetCurrencyPairs(System.DateTime.Now);
		// cachedConverter.GetCurrencyPairs(System.DateTime.Now); cachedConverter.GetCurrencyPairs(System.DateTime.Now);

		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, callsCount);
		//}

		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.SequenceEqual(cachedConverter.GetCurrencyPairs(date1)));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair2.SequenceEqual(cachedConverter.GetCurrencyPairs(date2)));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.SequenceEqual(cachedConverter.GetCurrencyPairs(new
		// System.DateTime(2014, 11, 22))));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair2.SequenceEqual(cachedConverter.GetCurrencyPairs(new
		// System.DateTime(2014, 11, 24))));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, callsCount);

		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair0.SequenceEqual(cachedConverter.GetCurrencyPairs(new System.DateTime(2014, 11, 25))));
		//    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);
		//}

		//TODO: rewrite
		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.RatesCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, rateCallCount);

		// cachedConverter.ConvertCurrency(testMoney, testPair.CounterCurrency, new
		// System.DateTime(2014, 8, 1));

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.RatesCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, rateCallCount);

		// cachedConverter.GetExchangeRate(testPair, new System.DateTime(2014, 8, 1));
		// cachedConverter.GetExchangeRate(testPair, new System.DateTime(2014, 9, 1));

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2,
		// cachedConverter.RatesCacheSize);
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, rateCallCount);

		// cachedConverter.GetExchangeRate(testPair, new System.DateTime(2014, 11, 1));

		//	Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, cachedConverter.RatesCacheSize);
		//	Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(4, rateCallCount);
		//}

		//TODO: Rewrite
		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		//	Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, callsCount);
		//}

		//TODO: Rewrite
		//[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate1,
		// cachedConverter.GetExchangeRate(currencyPair, date1));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(1, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate2,
		// cachedConverter.GetExchangeRate(currencyPair, date2));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(2, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate3,
		// cachedConverter.GetExchangeRate(currencyPair, date3));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate1,
		// cachedConverter.GetExchangeRate(currencyPair, new System.DateTime(2014, 11, 22)));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate2,
		// cachedConverter.GetExchangeRate(currencyPair, new System.DateTime(2014, 11, 24)));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);

		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate3,
		// cachedConverter.GetExchangeRate(currencyPair, new System.DateTime(2014, 11, 26)));
		// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(3, callsCount);

		//	Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(rate0, cachedConverter.GetExchangeRate(currencyPair, new System.DateTime(2014, 11, 27)));
		//	Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(4, callsCount);
		//}
	}
}