using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class BankOfRussiaTests
	{
		[TestMethod]
		public async Task Calculate001()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			foreach (var pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount == rate * Before.Amount);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency001()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var Moment = System.DateTime.Now;

			foreach (var pair in await Bank.GetCurrencyPairsAsync(Moment))
			{
				Money BeforeConversion = new Money(pair.BaseCurrency, 100m);

				Money AfterComversion = await Bank.ConvertCurrencyAsync(BeforeConversion, pair.CounterCurrency, Moment);

				Assert.IsTrue(AfterComversion.Amount > 0m);
				Assert.AreEqual<Finance.CurrencyInfo>(pair.CounterCurrency, AfterComversion.Currency);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency002()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo RU = new System.Globalization.RegionInfo("RU");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo RUB = new CurrencyInfo(RU);

			try
			{
				Money Before = new Money(USD, 100m);

				Money After = await Bank.ConvertCurrencyAsync(Before, RUB, System.DateTime.Now.AddMinutes(1d));

				Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Assert.Fail();
			}
		}

		[TestMethod]
		public async Task ConvertCurrency003()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			System.Globalization.RegionInfo AO = new System.Globalization.RegionInfo("AO");
			System.Globalization.RegionInfo BW = new System.Globalization.RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			try
			{
				Money Before = new Money(AOA, 100m);

				Money After = await Bank.ConvertCurrencyAsync(Before, BWP, System.DateTime.Now);

				Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Assert.Fail();
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs001()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.IsTrue(CurrencyPairs.Any(C => C == reversePair));
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs002()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			System.Collections.Generic.HashSet<CurrencyPair> pairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				pairSet.Add(pair);
			}

			Assert.IsTrue(pairSet.Count == CurrencyPairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs003()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			try
			{
				var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now.AddDays(10));

				Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs004()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			Assert.IsTrue(pairs.Any(C => C.ToString() == "AUD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "AZN/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "AMD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "BYR/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "BGN/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "BRL/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "HUF/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "KRW/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "DKK/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "EUR/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "INR/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "KZT/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CAD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "KGS/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CNY/RUB"));
			//Assert.IsTrue(pairs.Any(C => C.ToString() == "LTL/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "MDL/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RON/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "TMT/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "NOK/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "PLN/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "SGD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "TJS/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "TRY/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "UZS/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "UAH/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "GBP/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CZK/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "SEK/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CHF/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "ZAR/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "JPY/RUB"));

			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/AUD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/AZN"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/AMD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/BYR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/BGN"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/BRL"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/HUF"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/KRW"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/DKK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/EUR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/INR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/KZT"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CAD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/KGS"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CNY"));
			//Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/LTL"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/MDL"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/RON"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/TMT"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/NOK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/PLN"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/SGD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/TJS"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/TRY"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/UZS"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/UAH"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/GBP"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CZK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/SEK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CHF"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/ZAR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/JPY"));

			//Assert.AreEqual<int>(66, pairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs005()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var pairs = await Bank.GetCurrencyPairsAsync(new System.DateTime(2010, 01, 01));

			Assert.IsTrue(pairs.Any(C => C.ToString() == "AUD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "BYR/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "DKK/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "EUR/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "ISK/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "KZT/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CAD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CNY/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "NOK/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "SGD/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "TRY/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "UAH/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "GBP/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "SEK/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CHF/RUB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "JPY/RUB"));

			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/AUD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/BYR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/DKK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/EUR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/ISK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/KZT"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CAD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CNY"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/NOK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/SGD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/TRY"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/UAH"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/GBP"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/SEK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/CHF"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "RUB/JPY"));

			Assert.AreEqual<int>(34, pairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs006()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var AtTheMoment = System.DateTime.Now;

			var pairs = await Bank.GetCurrencyPairsAsync(AtTheMoment);

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
				Assert.IsTrue(WebPairs.Any(WP => WP == Pair.ToString()));
			}

			foreach (var WebPair in WebPairs)
			{
				Assert.IsTrue(pairs.Any(P => P.ToString() == WebPair));
			}

			Assert.IsTrue(pairs.Count() == WebPairs.Count);
		}

		[TestMethod]
		public async Task GetCurrencyPairs007()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			for (int year = 1994; year <= System.DateTime.Now.Year; year++)
			{
				for (int month = 1; (year < System.DateTime.Now.Year && month <= 12) || month <= System.DateTime.Now.Month; month++)
				{
					var date = new System.DateTime(year, month, 1);

					await Bank.GetCurrencyPairsAsync(date);
				}
			}
		}

		[TestMethod]
		public async Task GetExchangeRate001()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			foreach (var pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate002()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo RU = new System.Globalization.RegionInfo("RU");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo RUB = new CurrencyInfo(RU);

			CurrencyPair pair = new CurrencyPair(RUB, USD);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now.AddMinutes(1d));

				Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Assert.Fail();
			}
		}

		[TestMethod]
		public async Task GetExchangeRate003()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			System.Globalization.RegionInfo AO = new System.Globalization.RegionInfo("AO");
			System.Globalization.RegionInfo BW = new System.Globalization.RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			CurrencyPair pair = new CurrencyPair(BWP, AOA);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
		}

		[TestMethod]
		public async Task GetExchangeRate004()
		{
			var Bank = new Finance.ForeignExchange.BankOfRussia();

			var Moment = System.DateTime.Now.AddYears(-1);

			foreach (var pair in await Bank.GetCurrencyPairsAsync(Moment))
			{
				var rate = await Bank.GetExchangeRateAsync(pair, Moment);

				Assert.IsTrue(rate > decimal.Zero);
			}
		}
	}
}