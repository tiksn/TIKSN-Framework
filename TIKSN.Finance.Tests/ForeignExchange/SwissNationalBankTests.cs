using System.Linq;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class SwissNationalBankTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Calculation001()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var AtTheMoment = System.DateTime.Now;

			foreach (var pair in Bank.GetCurrencyPairs(AtTheMoment))
			{
				var Before = new Money(pair.BaseCurrency, 100m);

				var After = Bank.ConvertCurrency(Before, pair.CounterCurrency, AtTheMoment);

				var rate = Bank.GetExchangeRate(pair, AtTheMoment);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == Before.Amount * rate);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(pair.CounterCurrency, After.Currency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency001()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var AtTheMoment = System.DateTime.Now;

			foreach (var Pair in Bank.GetCurrencyPairs(AtTheMoment))
			{
				var Before = new Money(Pair.BaseCurrency, 100m);
				var After = Bank.ConvertCurrency(Before, Pair.CounterCurrency, AtTheMoment);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount > decimal.Zero);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(Pair.CounterCurrency, After.Currency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency002()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var moment = System.DateTime.Now.AddMinutes(10d);

			foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				try
				{
					var Before = new Money(pair.BaseCurrency, 100m);

					var After = Bank.ConvertCurrency(Before, pair.CounterCurrency, moment);

					Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
				}
				catch (System.ArgumentException)
				{
				}
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency004()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var moment = System.DateTime.Now.AddDays(-10d);

			foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				try
				{
					var Before = new Money(pair.BaseCurrency, 100m);

					var After = Bank.ConvertCurrency(Before, pair.CounterCurrency, moment);

					Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
				}
				catch (System.ArgumentException)
				{
				}
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CounterCurrency003()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			System.Globalization.RegionInfo AO = new System.Globalization.RegionInfo("AO");
			System.Globalization.RegionInfo BW = new System.Globalization.RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			try
			{
				var Before = new Money(AOA, 100m);

				var After = Bank.ConvertCurrency(Before, BWP, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs001()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var moment = System.DateTime.Now.AddMinutes(10d);

			try
			{
				var pairs = Bank.GetCurrencyPairs(moment);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs002()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var Pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			var DistinctPairs = Pairs.Distinct();

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Count() == DistinctPairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs003()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var moment = System.DateTime.Now.AddDays(-10d);

			try
			{
				var pairs = Bank.GetCurrencyPairs(moment);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs004()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var Pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pair in Pairs)
			{
				var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P == reversed));
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs005()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var Pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P.ToString() == "EUR/CHF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P.ToString() == "USD/CHF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P.ToString() == "JPY/CHF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P.ToString() == "GBP/CHF"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P.ToString() == "CHF/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P.ToString() == "CHF/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P.ToString() == "CHF/JPY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Pairs.Any(P => P.ToString() == "CHF/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(8, Pairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate001()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var AtTheMoment = System.DateTime.Now;

			foreach (var Pair in Bank.GetCurrencyPairs(AtTheMoment))
			{
				decimal rate = Bank.GetExchangeRate(Pair, AtTheMoment);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate002()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var moment = System.DateTime.Now.AddMinutes(10d);

			foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				try
				{
					decimal rate = Bank.GetExchangeRate(pair, moment);

					Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
				}
				catch (System.ArgumentException)
				{
				}
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate003()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			System.Globalization.RegionInfo AO = new System.Globalization.RegionInfo("AO");
			System.Globalization.RegionInfo BW = new System.Globalization.RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			CurrencyPair Pair = new CurrencyPair(AOA, BWP);

			try
			{
				var rate = Bank.GetExchangeRate(Pair, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate004()
		{
			var Bank = new Finance.ForeignExchange.SwissNationalBank();

			var moment = System.DateTime.Now.AddDays(-10d);

			foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				try
				{
					decimal rate = Bank.GetExchangeRate(pair, moment);

					Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
				}
				catch (System.ArgumentException)
				{
				}
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void KeepCurrenciesPairsUpdated()
		{
			// In case or failure, check currency pair information from SNB website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 03, 1);

			if (System.DateTime.Now > Deadline)
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
		}
	}
}