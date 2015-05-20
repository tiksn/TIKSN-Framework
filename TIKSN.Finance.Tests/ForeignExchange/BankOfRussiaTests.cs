using System.Linq;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class BankOfRussiaTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Calculate001()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

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
		public void ConvertCurrency001()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var Moment = System.DateTime.Now;

			foreach (var pair in Bank.GetCurrencyPairs(Moment))
			{
				Money BeforeConversion = new Money(pair.BaseCurrency, 100m);

				Money AfterComversion = Bank.ConvertCurrency(BeforeConversion, pair.CounterCurrency, Moment);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(AfterComversion.Amount > 0m);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<Finance.CurrencyInfo>(pair.CounterCurrency, AfterComversion.Currency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency002()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo RU = new System.Globalization.RegionInfo("RU");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo RUB = new CurrencyInfo(RU);

			try
			{
				Money Before = new Money(USD, 100m);

				Money After = Bank.ConvertCurrency(Before, RUB, System.DateTime.Now.AddMinutes(1d));

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
			var Bank = new Finance.ForeignExchange.BankOfRussia();

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
		public void GetCurrencyPairs001()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(C => C == reversePair));
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs002()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			System.Collections.Generic.HashSet<CurrencyPair> pairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				pairSet.Add(pair);
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairSet.Count == CurrencyPairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs003()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			try
			{
				var pairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddDays(10));

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs004()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "AUD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "AZN/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "AMD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "BYR/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "BGN/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "BRL/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "HUF/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "KRW/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "DKK/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "EUR/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "INR/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "KZT/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CAD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "KGS/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CNY/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "LTL/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "MDL/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RON/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "TMT/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "NOK/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "PLN/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "SGD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "TJS/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "TRY/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "UZS/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "UAH/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "GBP/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CZK/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "SEK/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CHF/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "ZAR/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "JPY/RUB"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/AZN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/AMD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/BYR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/BGN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/BRL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/HUF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/KRW"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/DKK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/INR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/KZT"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/KGS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CNY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/LTL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/MDL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/RON"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/TMT"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/NOK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/PLN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/SGD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/TJS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/TRY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/UZS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/UAH"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CZK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/SEK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CHF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/ZAR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/JPY"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(66, pairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs005()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var pairs = Bank.GetCurrencyPairs(new System.DateTime(2010, 01, 01));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "AUD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "BYR/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "DKK/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "EUR/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "ISK/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "KZT/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CAD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CNY/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "NOK/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "SGD/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "TRY/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "UAH/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "GBP/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "SEK/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "CHF/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "JPY/RUB"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/BYR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/DKK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/ISK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/KZT"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CNY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/NOK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/SGD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/TRY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/UAH"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/SEK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CHF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/JPY"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(34, pairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs006()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var AtTheMoment = System.DateTime.Now;

			var pairs = Bank.GetCurrencyPairs(AtTheMoment);

			var WebUrl = string.Format("http://www.cbr.ru/scripts/XML_daily.asp?date_req={2:00}/{1:00}/{0}", AtTheMoment.Year, AtTheMoment.Month, AtTheMoment.Day);

			System.Xml.Linq.XDocument Xdoc = System.Xml.Linq.XDocument.Load(WebUrl);

			var WebPairs = new System.Collections.Generic.List<string>();

			foreach (var CodeElement in Xdoc.Element("ValCurs").Elements("Valute"))
			{
				string code = CodeElement.Element("CharCode").Value.Trim().ToUpper();

				if (code == "XDR")
					continue;

				WebPairs.Add(string.Format("{0}/RUB", code));
				WebPairs.Add(string.Format("RUB/{0}", code));
			}

			foreach (var Pair in pairs)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(WebPairs.Any(WP => WP == Pair.ToString()));
			}

			foreach (var WebPair in WebPairs)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == WebPair));
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Count() == WebPairs.Count);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs007()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			for (int year = 1994; year <= System.DateTime.Now.Year; year++)
			{
				for (int month = 1; (year < System.DateTime.Now.Year && month <= 12) || month <= System.DateTime.Now.Month; month++)
				{
					var date = new System.DateTime(year, month, 1);

					Bank.GetCurrencyPairs(date);
				}
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate001()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			foreach (var pair in Bank.GetCurrencyPairs(System.DateTime.Now))
			{
				var rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate002()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo RU = new System.Globalization.RegionInfo("RU");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo RUB = new CurrencyInfo(RU);

			CurrencyPair pair = new CurrencyPair(RUB, USD);

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
			var Bank = new Finance.ForeignExchange.BankOfRussia();

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
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate004()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var Moment = System.DateTime.Now.AddYears(-1);

			foreach (var pair in Bank.GetCurrencyPairs(Moment))
			{
				var rate = Bank.GetExchangeRate(pair, Moment);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void KeepCurrenciesPairsUpdated()
		{
			// In case or failure, check currency pair information from CBR website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 3, 1);

			if (System.DateTime.Now > Deadline)
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
		}
	}
}