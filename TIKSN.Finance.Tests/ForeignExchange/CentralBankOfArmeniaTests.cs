using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class CentralBankOfArmeniaTests
	{
		[TestMethod]
		public async Task ConversionDirection001()
		{
			var Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var ArmenianDram = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDram = await Bank.ConvertCurrencyAsync(BeforeInPound, ArmenianDram, System.DateTime.Now);

			Assert.IsTrue(BeforeInPound.Amount < AfterInDram.Amount);
		}

		[TestMethod]
		public async Task ConvertCurrency001()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in CurrencyPairs)
			{
				Money Initial = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);
				Money Result = await Bank.ConvertCurrencyAsync(Initial, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(Result.Currency == pair.CounterCurrency);
				Assert.IsTrue(Result.Amount > 0m);
				Assert.IsTrue(Result.Amount == (rate * Initial.Amount));
			}
		}

		[TestMethod]
		public async Task ConvertCurrency002()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			try
			{
				Money After = await Bank.ConvertCurrencyAsync(Before, Dram, System.DateTime.Now.AddDays(1d));

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
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			try
			{
				Money After = await Bank.ConvertCurrencyAsync(Before, Dram, System.DateTime.Now.AddMinutes(1d));

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
		public async Task ConvertCurrency004()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			try
			{
				Money After2 = await Bank.ConvertCurrencyAsync(Before, Dram, System.DateTime.Now.AddDays(-20d));

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
		public async Task Fetch001()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			await Bank.FetchAsync();
		}

		[TestMethod]
		public async Task GetCurrencyPairs001()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Collections.Generic.HashSet<CurrencyPair> pairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (CurrencyPair pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				pairs.Add(pair);
			}

			Assert.IsTrue(pairs.Count == (await Bank.GetCurrencyPairsAsync(System.DateTime.Now)).Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs002()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "USD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "GBP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AUD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "AUD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ARS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ARS"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "DKK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "DKK"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "EGP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "EGP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "EUR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "EUR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TRY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TRY"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "IRR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "IRR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ILS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ILS"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "PLN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "PLN"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "LBP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CAD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CAD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "INR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "INR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "HUF" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "HUF"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "JPY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "JPY"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "NOK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "NOK"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SEK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SEK"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CHF" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CHF"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CZK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CZK"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CNY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CNY"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SGD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SGD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KRW" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KRW"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "MXN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "MXN"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SAR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SAR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SYP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SYP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AED" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "AED"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KWD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KWD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BGN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BGN"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "RON" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "RON"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ISK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ISK"));

			// Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "LVL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			// Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LVL"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "LTL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LTL"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KGS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KGS"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KZT" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KZT"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "MDL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "MDL"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "RUB" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "RUB"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "UAH" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "UAH"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "UZS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "UZS"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BYR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BYR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TJS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TJS"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TMT" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TMT"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "GEL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "GEL"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "HKD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "HKD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BRL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BRL"));

			//Assert.AreEqual<int>(88, CurrencyPairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs003()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reverse = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.IsTrue(CurrencyPairs.Any(C => C == reverse));
			}
		}

		[TestMethod]
		public async Task GetExchangeRate001()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Assert.IsTrue(await Bank.GetExchangeRateAsync(pair, System.DateTime.Now) > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate002()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.AreEqual<decimal>(decimal.One, System.Math.Round(await Bank.GetExchangeRateAsync(pair, System.DateTime.Now) * await Bank.GetExchangeRateAsync(reversePair, System.DateTime.Now), 5));
			}
		}

		[TestMethod]
		public async Task GetExchangeRate003()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(DollarPerDram, System.DateTime.Now.AddDays(1d));

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
		public async Task GetExchangeRate004()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			try
			{
				decimal rate2 = await Bank.GetExchangeRateAsync(DollarPerDram, System.DateTime.Now.AddDays(-20d));

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
		public async Task GetExchangeRate005()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(DollarPerDram, System.DateTime.Now.AddMinutes(1d));

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
		public async Task GetExchangeRate006()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo Albania = new System.Globalization.RegionInfo("AL");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Lek = new CurrencyInfo(Albania);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair LekPerDram = new CurrencyPair(Lek, Dram);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(LekPerDram, System.DateTime.Now);

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
		public async Task GetExchangeRate007()
		{
			Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			System.Globalization.RegionInfo Albania = new System.Globalization.RegionInfo("AL");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo Lek = new CurrencyInfo(Albania);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DramPerLek = new CurrencyPair(Dram, Lek);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(DramPerLek, System.DateTime.Now);

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
	}
}