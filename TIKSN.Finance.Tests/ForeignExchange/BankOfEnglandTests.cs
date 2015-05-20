using System.Linq;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class BankOfEnglandTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Calculate001()
		{
			var Bank = new Finance.ForeignExchange.BankOfEngland();

			foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				var Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == rate * Before.Amount);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Calculate002()
		{
			var Bank = new Finance.ForeignExchange.BankOfEngland();
			var TenYearsAgo = System.DateTime.Now.AddYears(-10);

			foreach (var pair in Bank.GetCurrencyPairs(TenYearsAgo))
			{
				var Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = Bank.GetExchangeRate(pair, TenYearsAgo);

				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, TenYearsAgo);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == rate * Before.Amount);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConversionDirection001()
		{
			var Bank = new Finance.ForeignExchange.BankOfEngland();

			var USDollar = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = Bank.ConvertCurrency(BeforeInPound, USDollar, System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency001()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency002()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			CurrencyPair pair = CurrencyPairs.First();

			Money Before = new Money(pair.BaseCurrency, 10m);

			try
			{
				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now.AddMinutes(1d));

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency003()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			CurrencyPair pair = new CurrencyPair(
				new CurrencyInfo(new System.Globalization.RegionInfo("AM")),
				new CurrencyInfo(new System.Globalization.RegionInfo("BY")));

			Money Before = new Money(pair.BaseCurrency, 10m);

			try
			{
				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs001()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "AUD/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "AUD/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CNY/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CNY/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CZK/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CZK/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CZK/EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "DKK/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "DKK/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "DKK/EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "EUR/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HKD/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HKD/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HUF/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HUF/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HUF/EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "INR/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "INR/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ILS/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ILS/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JPY/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JPY/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JPY/EUR"));

			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LVL/USD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LVL/GBP"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LVL/EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LTL/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LTL/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LTL/EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MYR/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MYR/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NZD/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NZD/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NOK/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NOK/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PLN/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PLN/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PLN/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RUB/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RUB/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SAR/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SAR/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SGD/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SGD/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ZAR/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ZAR/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "KRW/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "KRW/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "GBP/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SEK/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SEK/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CHF/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CHF/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CHF/EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TWD/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TWD/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "THB/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "THB/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TRY/USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "USD/GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(61, Bank.GetCurrencyPairs(System.DateTime.Now).Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs002()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			var UniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (var pair in CurrencyPairs)
			{
				UniquePairs.Add(pair);
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(UniquePairs.Count == CurrencyPairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs003()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddYears(-10));

			var UniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (var pair in CurrencyPairs)
			{
				UniquePairs.Add(pair);
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(UniquePairs.Count == CurrencyPairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate001()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			foreach (CurrencyPair pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Bank.GetExchangeRate(pair, System.DateTime.Now) > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate002()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			try
			{
				decimal rate = Bank.GetExchangeRate(CurrencyPairs.First(), System.DateTime.Now.AddMinutes(1d));

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate003()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			CurrencyPair pair = new CurrencyPair(
				new CurrencyInfo(new System.Globalization.RegionInfo("AM")),
				new CurrencyInfo(new System.Globalization.RegionInfo("BY")));

			try
			{
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch (System.Exception ex)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(ex.ToString());
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void KeepCurrenciesPairsUpdated()
		{
			// In case or failure, check currency pair information from BOE website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 3, 1);

			if (System.DateTime.Now > Deadline)
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
		}
	}
}