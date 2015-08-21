using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class BankOfEnglandTests
	{
		[TestMethod]
		public async Task Calculate001()
		{
			var Bank = new Finance.ForeignExchange.BankOfEngland();

			foreach (var pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				var Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount == rate * Before.Amount);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task Calculate002()
		{
			var Bank = new Finance.ForeignExchange.BankOfEngland();
			var TenYearsAgo = System.DateTime.Now.AddYears(-10);

			foreach (var pair in await Bank.GetCurrencyPairsAsync(TenYearsAgo))
			{
				var Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, TenYearsAgo);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, TenYearsAgo);

				Assert.IsTrue(After.Amount == rate * Before.Amount);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task ConversionDirection001()
		{
			var Bank = new Finance.ForeignExchange.BankOfEngland();

			var USDollar = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, USDollar, System.DateTime.Now);

			Assert.IsTrue(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[TestMethod]
		public async Task ConvertCurrency001()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency002()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			CurrencyPair pair = CurrencyPairs.First();

			Money Before = new Money(pair.BaseCurrency, 10m);

			try
			{
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now.AddMinutes(1d));

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
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			CurrencyPair pair = new CurrencyPair(
				new CurrencyInfo(new System.Globalization.RegionInfo("AM")),
				new CurrencyInfo(new System.Globalization.RegionInfo("BY")));

			Money Before = new Money(pair.BaseCurrency, 10m);

			try
			{
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

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
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "AUD/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "AUD/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CNY/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CNY/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CZK/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CZK/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CZK/EUR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "DKK/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "DKK/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "DKK/EUR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "EUR/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HKD/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HKD/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HUF/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HUF/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HUF/EUR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "INR/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "INR/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ILS/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ILS/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JPY/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JPY/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JPY/EUR"));

			//Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LVL/USD"));
			//Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LVL/GBP"));
			//Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LVL/EUR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LTL/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LTL/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LTL/EUR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MYR/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MYR/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NZD/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NZD/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NOK/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NOK/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PLN/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PLN/EUR"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PLN/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RUB/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RUB/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SAR/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SAR/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SGD/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SGD/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ZAR/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ZAR/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "KRW/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "KRW/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "GBP/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SEK/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SEK/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CHF/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CHF/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CHF/EUR"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TWD/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TWD/GBP"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "THB/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "THB/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TRY/USD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "USD/GBP"));

			Assert.AreEqual<int>(61, (await Bank.GetCurrencyPairsAsync(System.DateTime.Now)).Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs002()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			var UniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (var pair in CurrencyPairs)
			{
				UniquePairs.Add(pair);
			}

			Assert.IsTrue(UniquePairs.Count == CurrencyPairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs003()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now.AddYears(-10));

			var UniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (var pair in CurrencyPairs)
			{
				UniquePairs.Add(pair);
			}

			Assert.IsTrue(UniquePairs.Count == CurrencyPairs.Count());
		}

		[TestMethod]
		public async Task GetExchangeRate001()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			foreach (CurrencyPair pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				Assert.IsTrue(await Bank.GetExchangeRateAsync(pair, System.DateTime.Now) > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate002()
		{
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(CurrencyPairs.First(), System.DateTime.Now.AddMinutes(1d));

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
			Finance.ForeignExchange.BankOfEngland Bank = new Finance.ForeignExchange.BankOfEngland();

			CurrencyPair pair = new CurrencyPair(
				new CurrencyInfo(new System.Globalization.RegionInfo("AM")),
				new CurrencyInfo(new System.Globalization.RegionInfo("BY")));

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
				Assert.Fail(ex.ToString());
			}
		}

		[TestMethod]
		public async Task KeepCurrenciesPairsUpdated()
		{
			// In case or failure, check currency pair information from BOE website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 3, 1);

			if (System.DateTime.Now > Deadline)
				Assert.Fail();
		}
	}
}