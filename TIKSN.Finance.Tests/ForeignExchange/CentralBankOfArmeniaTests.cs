using System.Linq;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class CentralBankOfArmeniaTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConversionDirection001()
		{
			var Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var ArmenianDram = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDram = Bank.ConvertCurrency(BeforeInPound, ArmenianDram, System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(BeforeInPound.Amount < AfterInDram.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency001()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pair in CurrencyPairs)
			{
				Money Initial = new Money(pair.BaseCurrency, 10m);
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);
				Money Result = Bank.ConvertCurrency(Initial, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Result.Currency == pair.CounterCurrency);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Result.Amount > 0m);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Result.Amount == (rate * Initial.Amount));
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency002()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			try
			{
				Money After = Bank.ConvertCurrency(Before, Dram, System.DateTime.Now.AddDays(1d));

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
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			try
			{
				Money After = Bank.ConvertCurrency(Before, Dram, System.DateTime.Now.AddMinutes(1d));

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
		public void ConvertCurrency004()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			try
			{
				Money After2 = Bank.ConvertCurrency(Before, Dram, System.DateTime.Now.AddDays(-20d));

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
		public void Fetch001()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			Bank.Fetch();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs001()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Collections.Generic.HashSet<CurrencyPair> pairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (CurrencyPair pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				pairs.Add(pair);
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Count == Bank.GetCurrencyPairs(System.DateTime.Now).Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs002()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "USD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "USD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "GBP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "GBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AUD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "AUD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ARS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ARS"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "DKK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "DKK"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "EGP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "EGP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "EUR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TRY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TRY"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "IRR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "IRR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ILS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ILS"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "PLN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "PLN"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "LBP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LBP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CAD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CAD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "INR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "INR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "HUF" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "HUF"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "JPY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "JPY"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "NOK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "NOK"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SEK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SEK"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CHF" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CHF"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CZK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CZK"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CNY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CNY"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SGD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SGD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KRW" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KRW"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "MXN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "MXN"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SAR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SAR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SYP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SYP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AED" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "AED"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KWD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KWD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BGN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BGN"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "RON" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "RON"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ISK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ISK"));

			// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "LVL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			// Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LVL"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "LTL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LTL"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KGS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KGS"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KZT" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KZT"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "MDL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "MDL"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "RUB" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "RUB"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "UAH" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "UAH"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "UZS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "UZS"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BYR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BYR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TJS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TJS"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TMT" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TMT"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "GEL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "GEL"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "HKD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "HKD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BRL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BRL"));

			// TODO: Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(88, CurrencyPairs.Count());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(88, CurrencyPairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs003()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reverse = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C == reverse));
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate001()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Bank.GetExchangeRate(pair, System.DateTime.Now) > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate002()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(decimal.One, System.Math.Round(Bank.GetExchangeRate(pair, System.DateTime.Now) * Bank.GetExchangeRate(reversePair, System.DateTime.Now), 5));
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate003()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			try
			{
				decimal rate = Bank.GetExchangeRate(DollarPerDram, System.DateTime.Now.AddDays(1d));

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
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			try
			{
				decimal rate2 = Bank.GetExchangeRate(DollarPerDram, System.DateTime.Now.AddDays(-20d));

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
		public void GetExchangeRate005()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			try
			{
				decimal rate = Bank.GetExchangeRate(DollarPerDram, System.DateTime.Now.AddMinutes(1d));

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
		public void GetExchangeRate006()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo Albania = new System.Globalization.RegionInfo("AL");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Lek = new CurrencyInfo(Albania);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair LekPerDram = new CurrencyPair(Lek, Dram);

			try
			{
				decimal rate = Bank.GetExchangeRate(LekPerDram, System.DateTime.Now);

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
		public void GetExchangeRate007()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo Albania = new System.Globalization.RegionInfo("AL");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Lek = new CurrencyInfo(Albania);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DramPerLek = new CurrencyPair(Dram, Lek);

			try
			{
				decimal rate = Bank.GetExchangeRate(DramPerLek, System.DateTime.Now);

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
			// In case or failure, check currency pair information from CBA website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 3, 1);

			if (System.DateTime.Now > Deadline)
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
		}
	}
}