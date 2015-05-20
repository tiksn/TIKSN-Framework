using System.Linq;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class BankOfCanadaTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Calculate001()
		{
			var Bank = new Finance.ForeignExchange.BankOfCanada();

			foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);
				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == rate * Before.Amount);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConversionDirection001()
		{
			var Bank = new Finance.ForeignExchange.BankOfCanada();

			var CanadianDollar = new CurrencyInfo(new System.Globalization.RegionInfo("CA"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = Bank.ConvertCurrency(BeforeInPound, CanadianDollar, System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency001()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, decimal.One);

				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency002()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, decimal.One);

				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency003()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == rate * Before.Amount);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency004()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo CA = new System.Globalization.RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			try
			{
				Money Before = new Money(USD, 100m);

				Money After = Bank.ConvertCurrency(Before, CAD, System.DateTime.Now.AddMinutes(1d));

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
		public void ConvertCurrency005()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo CA = new System.Globalization.RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			try
			{
				Money Before = new Money(USD, 100m);

				Money After = Bank.ConvertCurrency(Before, CAD, System.DateTime.Now.AddDays(-20d));

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
		public void ConvertCurrency006()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			System.Globalization.RegionInfo AO = new System.Globalization.RegionInfo("AO");
			System.Globalization.RegionInfo BW = new System.Globalization.RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			try
			{
				Money Before = new Money(AOA, 100m);

				Money After = Bank.ConvertCurrency(Before, BWP, System.DateTime.Now);

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
		public void CurrencyPairs001()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/ARS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/BRL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/CLP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/CNY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/COP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/HRK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/CZK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/DKK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/GTQ"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/HNL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/HKD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/HUF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/ISK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/INR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/IDR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/ILS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/JMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/JPY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/MYR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/MXN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/MAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/MMK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/NZD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/NOK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PKR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PAB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PEN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PHP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PLN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/RON"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/RSD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/SGD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/ZAR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/KRW"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/LKR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/SEK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/CHF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/TWD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/THB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/TTD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/TND"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/TRY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/AED"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/VEF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/VND"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/XAF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/XCD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "USD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ARS/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "AUD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "BRL/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CLP/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CNY/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "COP/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HRK/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CZK/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "DKK/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "EUR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "GTQ/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HNL/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HKD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HUF/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ISK/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "INR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "IDR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ILS/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JMD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JPY/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MYR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MXN/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MAD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MMK/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NZD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NOK/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PKR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PAB/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PEN/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PHP/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PLN/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RON/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RUB/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RSD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SGD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ZAR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "KRW/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LKR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SEK/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CHF/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TWD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "THB/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TTD/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TND/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TRY/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "AED/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "GBP/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "VEF/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "VND/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "XAF/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "XCD/CAD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(104, CurrencyPairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyPairs002()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C == reversePair));
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyPairs003()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			System.Collections.Generic.HashSet<CurrencyPair> pairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				pairSet.Add(pair);
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairSet.Count == CurrencyPairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyPairs004()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			try
			{
				var pairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddDays(-10));

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch (System.Exception)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyPairs005()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			try
			{
				var pairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddDays(10));

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch (System.Exception)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Fetch001()
		{
			var Bank = new Finance.ForeignExchange.BankOfCanada();

			Bank.Fetch();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate001()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate002()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo CA = new System.Globalization.RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			CurrencyPair pair = new CurrencyPair(CAD, USD);

			try
			{
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now.AddMinutes(1d));

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
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo CA = new System.Globalization.RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			CurrencyPair pair = new CurrencyPair(CAD, USD);

			try
			{
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now.AddDays(-20d));

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
		public void GetExchangeRate004()
		{
			Finance.ForeignExchange.BankOfCanada Bank = new Finance.ForeignExchange.BankOfCanada();

			System.Globalization.RegionInfo AO = new System.Globalization.RegionInfo("AO");
			System.Globalization.RegionInfo BW = new System.Globalization.RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			CurrencyPair pair = new CurrencyPair(BWP, AOA);

			try
			{
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

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
		public void KeepCurrenciesPairsUpdated()
		{
			// In case or failure, check currency pair information from BOC website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 3, 1);

			if (System.DateTime.Now > Deadline)
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
		}
	}
}