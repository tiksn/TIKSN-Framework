using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class ReserveBankOfAustraliaTests
	{
		[TestMethod]
		public async Task ConversionDirection001()
		{
			var Bank = new ReserveBankOfAustralia();

			var AustralianDollar = new CurrencyInfo(new System.Globalization.RegionInfo("AU"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, AustralianDollar, System.DateTime.Now);

			Assert.IsTrue(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[TestMethod]
		public async Task ConvertCurrency001()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount > decimal.Zero);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency002()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount == Before.Amount * rate);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency003()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				try
				{
					Money Before = new Money(pair.BaseCurrency, 10m);

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
		}

		[TestMethod]
		public async Task ConvertCurrency004()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				try
				{
					Money Before = new Money(pair.BaseCurrency, 10m);

					Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now.AddDays(-20d));

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

		[TestMethod]
		public async Task ConvertCurrency005()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
			System.Globalization.RegionInfo Belarus = new System.Globalization.RegionInfo("BY");

			CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);
			CurrencyInfo BelarusianRuble = new CurrencyInfo(Belarus);

			Money Before = new Money(ArmenianDram, 10m);

			try
			{
				Money After = await Bank.ConvertCurrencyAsync(Before, BelarusianRuble, System.DateTime.Now);

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
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			await Bank.FetchAsync();
		}

		[TestMethod]
		public async Task GetCurrencyPairs001()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "USD/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "CNY/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "JPY/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "EUR/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "KRW/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "GBP/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "SGD/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "INR/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "THB/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "NZD/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "TWD/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "MYR/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "IDR/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "VND/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AED/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "HKD/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "CAD/AUD"));
			//Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "ZAR/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "CHF/AUD"));
			//Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "SEK/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "PHP/AUD"));

			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/USD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/CNY"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/JPY"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/EUR"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/KRW"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/SGD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/INR"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/THB"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/NZD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/TWD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/MYR"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/IDR"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/VND"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/AED"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/HKD"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/CAD"));
			//Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/ZAR"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/CHF"));
			//Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/SEK"));
			Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/PHP"));

			//Assert.IsTrue(CurrencyPairs.Count() == 38);
		}

		[TestMethod]
		public async Task GetCurrencyPairs002()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.IsTrue(CurrencyPairs.Any(P => P == reversedPair));
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs003()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			System.Collections.Generic.HashSet<CurrencyPair> PairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				PairSet.Add(pair);
			}

			Assert.IsTrue(PairSet.Count == CurrencyPairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs004()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			try
			{
				var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now.AddMinutes(10));

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
		public async Task GetCurrencyPairs005()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			try
			{
				var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now.AddDays(-20));

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
		public async Task GetExchangeRate001()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Assert.IsTrue(await Bank.GetExchangeRateAsync(pair, System.DateTime.Now) > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate002()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
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
		}

		[TestMethod]
		public async Task GetExchangeRate003()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				try
				{
					decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now.AddDays(-20d));

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

		[TestMethod]
		public async Task GetExchangeRate004()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
			System.Globalization.RegionInfo Belarus = new System.Globalization.RegionInfo("BY");

			CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);
			CurrencyInfo BelarusianRuble = new CurrencyInfo(Belarus);

			CurrencyPair pair = new CurrencyPair(ArmenianDram, BelarusianRuble);

			try
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

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
		public async Task KeepCurrenciesPairsUpdated()
		{
			// In case or failure, check currency pair information from RBA website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 03, 1);

			if (System.DateTime.Now > Deadline)
				Assert.Fail();
		}
	}
}