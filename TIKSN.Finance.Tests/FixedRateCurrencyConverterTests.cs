using System.Linq;

namespace TIKSN.Finance.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class FixedRateCurrencyConverterTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);

			Money Initial = new Money(USDollar, 100);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

			Money Final = converter.ConvertCurrency(Initial, PoundSterling, System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(PoundSterling, Final.Currency);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(200m, Final.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency002()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			System.Globalization.RegionInfo Italy = new System.Globalization.RegionInfo("IT");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);
			CurrencyInfo Euro = new CurrencyInfo(Italy);

			Money Initial = new Money(USDollar, 100);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

			try
			{
				converter.ConvertCurrency(Initial, Euro, System.DateTime.Now);

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
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			System.Globalization.RegionInfo Italy = new System.Globalization.RegionInfo("IT");
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);
			CurrencyInfo Euro = new CurrencyInfo(Italy);
			CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);

			Money Initial = new Money(ArmenianDram, 100);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

			try
			{
				converter.ConvertCurrency(Initial, Euro, System.DateTime.Now);

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
		public void CurrencyPair001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			System.Globalization.RegionInfo Italy = new System.Globalization.RegionInfo("IT");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);
			CurrencyInfo Euro = new CurrencyInfo(Italy);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(object.ReferenceEquals(converter.CurrencyPair.BaseCurrency, USDollar));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(object.ReferenceEquals(converter.CurrencyPair.CounterCurrency, PoundSterling));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void FixedRateCurrencyConverter001()
		{
			try
			{
				FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(null, 0.5m);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentNullException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void FixedRateCurrencyConverter002()
		{
			try
			{
				System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
				System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("AM");

				CurrencyInfo USD = new CurrencyInfo(US);
				CurrencyInfo AMD = new CurrencyInfo(AM);

				CurrencyPair pair = new CurrencyPair(USD, AMD);

				FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(pair, -0.5m);

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
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);

			var pair = new CurrencyPair(USDollar, PoundSterling);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(pair, 2m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(USDollar, converter.CurrencyPair.BaseCurrency);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(PoundSterling, converter.CurrencyPair.CounterCurrency);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(object.ReferenceEquals(pair, converter.GetCurrencyPairs(System.DateTime.Now).Single()));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(2m, converter.GetExchangeRate(new CurrencyPair(USDollar, PoundSterling), System.DateTime.Now));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate002()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

			try
			{
				converter.GetExchangeRate(new CurrencyPair(PoundSterling, USDollar), System.DateTime.Now);

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
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			System.Globalization.RegionInfo Italy = new System.Globalization.RegionInfo("IT");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);
			CurrencyInfo Euro = new CurrencyInfo(Italy);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

			try
			{
				converter.GetExchangeRate(new CurrencyPair(Euro, USDollar), System.DateTime.Now);

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
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			System.Globalization.RegionInfo Italy = new System.Globalization.RegionInfo("IT");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);
			CurrencyInfo Euro = new CurrencyInfo(Italy);

			FixedRateCurrencyConverter converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

			try
			{
				converter.GetExchangeRate(new CurrencyPair(USDollar, Euro), System.DateTime.Now);

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
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");

			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo PoundSterling = new CurrencyInfo(UnitedKingdom);

			CurrencyPair pair = new CurrencyPair(PoundSterling, USDollar);

			var converter = new FixedRateCurrencyConverter(pair, 1.6m);

			var LastMonth = System.DateTime.Now.AddMonths(-1);
			var NextMonth = System.DateTime.Now.AddMonths(1);

			decimal RateInLastMonth = converter.GetExchangeRate(pair, LastMonth);
			decimal RateInNextMonth = converter.GetExchangeRate(pair, NextMonth);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(RateInLastMonth == RateInNextMonth);
		}
	}
}