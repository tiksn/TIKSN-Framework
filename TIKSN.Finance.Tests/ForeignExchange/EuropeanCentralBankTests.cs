using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class EuropeanCentralBankTests
	{
		[TestMethod]
		public async Task Calculation001()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount == Before.Amount * rate);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task Calculation002()
		{
			var Bank = new EuropeanCentralBank();

			var OneYearsAgo = System.DateTime.Now.AddYears(-1);
			var pairs = await Bank.GetCurrencyPairsAsync(OneYearsAgo);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, OneYearsAgo);
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, OneYearsAgo);

				Assert.IsTrue(After.Amount == Before.Amount * rate);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task ConversionDirection001()
		{
			var Bank = new EuropeanCentralBank();

			var Euro = new CurrencyInfo(new System.Globalization.RegionInfo("DE"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInEuro = new Money(Euro, 100m);

			var AfterInPound = await Bank.ConvertCurrencyAsync(BeforeInEuro, PoundSterling, System.DateTime.Now);

			Assert.IsTrue(BeforeInEuro.Amount > AfterInPound.Amount);
		}

		[TestMethod]
		public async Task ConvertCurrency001()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount > 0m);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency002()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				try
				{
					Money Before = new Money(pair.BaseCurrency, 10m);
					Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now.AddMinutes(10d));

					Assert.Fail();
				}
				catch (System.ArgumentException)
				{
				}
				catch (System.Exception ex)
				{
					Assert.Fail(ex.Message);
				}
			}
		}

		[TestMethod]
		public async Task ConvertCurrency003()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
			var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

			Money Before = new Money(AMD, 10m);

			try
			{
				Money After = await Bank.ConvertCurrencyAsync(Before, ALL, System.DateTime.Now);

				Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch (System.Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs001()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				var ReversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.IsTrue(pairs.Any(P => P == ReversedPair));
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs002()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);
			var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (var pair in pairs)
			{
				bool WasUnique = uniquePairs.Add(pair);

				if (!WasUnique)
				{
					System.Diagnostics.Debug.WriteLine(pair);
				}
			}

			Assert.IsTrue(uniquePairs.Count == pairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs003()
		{
			var Bank = new EuropeanCentralBank();

			try
			{
				var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now.AddMinutes(10d));

				Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch (System.Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs004()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			//Assert.IsTrue(pairs.Any(P => P.ToString() == "ARS/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "AUD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "BGN/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "BRL/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "CAD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "CHF/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "CNY/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "CYP/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "CZK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "DKK/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "DZD/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EEK/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EGP/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "GBP/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "GRD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "HKD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "HRK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "HUF/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "IDR/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "ILS/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "INR/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "ISK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "JPY/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "KRW/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "LTL/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "MAD/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "MTL/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "MXN/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "MYR/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "NOK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "NZD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "PHP/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "PLN/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "RON/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "RUB/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "SEK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "SGD/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "SIT/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "THB/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "TRY/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "TWD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "USD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "ZAR/EUR"));

			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ARS"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/AUD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/BGN"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/BRL"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CAD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CHF"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CNY"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CYP"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CZK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/DKK"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/DZD"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/EEK"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/EGP"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/GBP"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/GRD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HKD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HRK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HUF"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/IDR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ILS"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/INR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ISK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/JPY"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/KRW"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/LTL"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MAD"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MTL"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MXN"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MYR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/NOK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/NZD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/PHP"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/PLN"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/RON"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/RUB"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SEK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SGD"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SIT"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/THB"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/TRY"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/TWD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/USD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ZAR"));

			//Assert.AreEqual<int>(37 * 2, pairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs005()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(new System.DateTime(2010, 1, 1));

			//Assert.IsTrue(pairs.Any(P => P.ToString() == "ARS/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "AUD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "BGN/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "BRL/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "CAD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "CHF/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "CNY/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "CYP/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "CZK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "DKK/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "DZD/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EEK/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EGP/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "GBP/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "GRD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "HKD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "HRK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "HUF/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "IDR/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "ILS/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "INR/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "ISK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "JPY/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "KRW/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "LTL/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "MAD/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "MTL/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "MXN/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "MYR/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "NOK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "NZD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "PHP/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "PLN/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "RON/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "RUB/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "SEK/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "SGD/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "SIT/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "THB/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "TRY/EUR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "TWD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "USD/EUR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "ZAR/EUR"));

			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ARS"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/AUD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/BGN"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/BRL"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CAD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CHF"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CNY"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CYP"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CZK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/DKK"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/DZD"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/EEK"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/EGP"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/GBP"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/GRD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HKD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HRK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HUF"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/IDR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ILS"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/INR"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ISK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/JPY"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/KRW"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/LTL"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MAD"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MTL"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MXN"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MYR"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/NOK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/NZD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/PHP"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/PLN"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/RON"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/RUB"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SEK"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SGD"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SIT"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/THB"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/TRY"));
			//Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/TWD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/USD"));
			Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ZAR"));

			//Assert.AreEqual<int>(37 * 2, pairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs006()
		{
			const string RequestUrl = "http://sdw.ecb.europa.eu/export.do?node=2018794&CURRENCY=&FREQ=D&sfl1=4&sfl3=4&DATASET=0&exportType=sdmx";

			System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(RequestUrl);

			var CurrencyPairs = new System.Collections.Generic.List<string>();

			foreach (var item in xdoc.Element("{http://www.SDMX.org/resources/SDMXML/schemas/v2_0/message}MessageGroup").Element("{http://www.ecb.int/vocabulary/stats/exr}DataSet").Elements("{http://www.ecb.int/vocabulary/stats/exr}Group"))
			{
				string BaseCurrency = item.Attribute("CURRENCY_DENOM").Value;
				string CounterCurrency = item.Attribute("CURRENCY").Value;

				CurrencyPairs.Add(string.Format("{0}/{1}", BaseCurrency, CounterCurrency));
				CurrencyPairs.Add(string.Format("{1}/{0}", BaseCurrency, CounterCurrency));
			}

			// Replaced by Euro
			Assert.IsTrue(CurrencyPairs.Remove("CYP/EUR"));
			Assert.IsTrue(CurrencyPairs.Remove("EUR/CYP"));
			Assert.IsTrue(CurrencyPairs.Remove("EEK/EUR"));
			Assert.IsTrue(CurrencyPairs.Remove("EUR/EEK"));
			Assert.IsTrue(CurrencyPairs.Remove("GRD/EUR"));
			Assert.IsTrue(CurrencyPairs.Remove("EUR/GRD"));
			Assert.IsTrue(CurrencyPairs.Remove("EUR/LVL"));
			Assert.IsTrue(CurrencyPairs.Remove("LVL/EUR"));
			Assert.IsTrue(CurrencyPairs.Remove("EUR/MTL"));
			Assert.IsTrue(CurrencyPairs.Remove("MTL/EUR"));
			Assert.IsTrue(CurrencyPairs.Remove("EUR/SKK"));
			Assert.IsTrue(CurrencyPairs.Remove("SKK/EUR"));
			Assert.IsTrue(CurrencyPairs.Remove("EUR/SIT"));
			Assert.IsTrue(CurrencyPairs.Remove("SIT/EUR"));
			//Assert.IsTrue(CurrencyPairs.Remove());
			//Assert.IsTrue(CurrencyPairs.Remove());
			//Assert.IsTrue(CurrencyPairs.Remove());
			//Assert.IsTrue(CurrencyPairs.Remove());
			//Assert.IsTrue(CurrencyPairs.Remove());
			//Assert.IsTrue(CurrencyPairs.Remove());

			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pairCode in CurrencyPairs)
			{
				Assert.IsTrue(pairs.Any(P => P.ToString() == pairCode));
			}

			Assert.IsTrue(pairs.Count() == CurrencyPairs.Count);
		}

		[TestMethod]
		public async Task GetExchangeRate001()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				Assert.IsTrue(rate > 0m);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate002()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				try
				{
					decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now.AddMinutes(10d));

					Assert.Fail();
				}
				catch (System.ArgumentException)
				{
				}
				catch (System.Exception ex)
				{
					Assert.Fail(ex.Message);
				}
			}
		}

		[TestMethod]
		public async Task GetExchangeRate003()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now.AddYears(-1));

			foreach (var pair in pairs)
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now.AddYears(-1));

				Assert.IsTrue(rate > 0m);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate004()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
			var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

			var pair = new CurrencyPair(AMD, ALL);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch (System.Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}
	}
}