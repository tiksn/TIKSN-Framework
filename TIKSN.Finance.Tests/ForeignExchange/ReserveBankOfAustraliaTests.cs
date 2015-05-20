using System.Linq;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class ReserveBankOfAustraliaTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConversionDirection001()
		{
			var Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var AustralianDollar = new CurrencyInfo(new System.Globalization.RegionInfo("AU"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = Bank.ConvertCurrency(BeforeInPound, AustralianDollar, System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency001()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount > decimal.Zero);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency002()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == Before.Amount * rate);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency003()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				try
				{
					Money Before = new Money(pair.BaseCurrency, 10m);

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
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency004()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				try
				{
					Money Before = new Money(pair.BaseCurrency, 10m);

					Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now.AddDays(-20d));

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
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency005()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
			System.Globalization.RegionInfo Belarus = new System.Globalization.RegionInfo("BY");

			CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);
			CurrencyInfo BelarusianRuble = new CurrencyInfo(Belarus);

			Money Before = new Money(ArmenianDram, 10m);

			try
			{
				Money After = Bank.ConvertCurrency(Before, BelarusianRuble, System.DateTime.Now);

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
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			Bank.Fetch();
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs001()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "USD/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "CNY/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "JPY/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "EUR/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "KRW/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "GBP/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "SGD/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "INR/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "THB/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "NZD/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "TWD/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "MYR/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "IDR/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "VND/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AED/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "HKD/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "CAD/AUD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "ZAR/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "CHF/AUD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "SEK/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "PHP/AUD"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/CNY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/JPY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/KRW"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/GBP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/SGD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/INR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/THB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/NZD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/TWD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/MYR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/IDR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/VND"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/AED"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/HKD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/CAD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/ZAR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/CHF"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/SEK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P.ToString() == "AUD/PHP"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Count() == 38);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs002()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Any(P => P == reversedPair));
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs003()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			System.Collections.Generic.HashSet<CurrencyPair> PairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				PairSet.Add(pair);
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(PairSet.Count == CurrencyPairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs004()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			try
			{
				var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddMinutes(10));

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
		public void GetCurrencyPairs005()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			try
			{
				var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddDays(-20));

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch (System.Exception ex)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(ex.Message);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate001()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Bank.GetExchangeRate(pair, System.DateTime.Now) > decimal.Zero);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate002()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
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
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate003()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			var CurrencyPairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
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
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate004()
		{
			Finance.ForeignExchange.ReserveBankOfAustralia Bank = new Finance.ForeignExchange.ReserveBankOfAustralia();

			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
			System.Globalization.RegionInfo Belarus = new System.Globalization.RegionInfo("BY");

			CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);
			CurrencyInfo BelarusianRuble = new CurrencyInfo(Belarus);

			CurrencyPair pair = new CurrencyPair(ArmenianDram, BelarusianRuble);

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
			// In case or failure, check currency pair information from RBA website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 03, 1);

			if (System.DateTime.Now > Deadline)
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
		}
	}
}