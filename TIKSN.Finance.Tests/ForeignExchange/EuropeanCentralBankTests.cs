using System.Linq;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class EuropeanCentralBankTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Calculation001()
		{
			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);
				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == Before.Amount * rate);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Calculation002()
		{
			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var OneYearsAgo = System.DateTime.Now.AddYears(-1);
			var pairs = Bank.GetCurrencyPairs(OneYearsAgo);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = Bank.GetExchangeRate(pair, OneYearsAgo);
				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, OneYearsAgo);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount == Before.Amount * rate);
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConversionDirection001()
		{
			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var Euro = new CurrencyInfo(new System.Globalization.RegionInfo("DE"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInEuro = new Money(Euro, 100m);

			var AfterInPound = Bank.ConvertCurrency(BeforeInEuro, PoundSterling, System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(BeforeInEuro.Amount > AfterInPound.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency001()
		{
			Finance.ForeignExchange.EuropeanCentralBank Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(After.Amount > 0m);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency002()
		{
			Finance.ForeignExchange.EuropeanCentralBank Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				try
				{
					Money Before = new Money(pair.BaseCurrency, 10m);
					Money After = Bank.ConvertCurrency(Before, pair.CounterCurrency, System.DateTime.Now.AddMinutes(10d));

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
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ConvertCurrency003()
		{
			Finance.ForeignExchange.EuropeanCentralBank Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
			var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

			Money Before = new Money(AMD, 10m);

			try
			{
				Money After = Bank.ConvertCurrency(Before, ALL, System.DateTime.Now);

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
		public void GetCurrencyPairs001()
		{
			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				var ReversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P == ReversedPair));
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs002()
		{
			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);
			var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (var pair in pairs)
			{
				bool WasUnique = uniquePairs.Add(pair);

				if (!WasUnique)
				{
					System.Diagnostics.Debug.WriteLine(pair);
				}
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(uniquePairs.Count == pairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs003()
		{
			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			try
			{
				var pairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddMinutes(10d));

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
		public void GetCurrencyPairs004()
		{
			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "ARS/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "AUD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "BGN/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "BRL/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CAD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CHF/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CNY/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CYP/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CZK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "DKK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "DZD/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EEK/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EGP/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "GBP/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "GRD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "HKD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "HRK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "HUF/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "IDR/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "ILS/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "INR/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "ISK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "JPY/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "KRW/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "LTL/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "MAD/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "MTL/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "MXN/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "MYR/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "NOK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "NZD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "PHP/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "PLN/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "RON/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "RUB/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "SEK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "SGD/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "SIT/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "THB/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "TRY/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "TWD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "USD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "ZAR/EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ARS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/BGN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/BRL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CHF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CNY"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CYP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CZK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/DKK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/DZD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/EEK"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/EGP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/GBP"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/GRD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HKD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HRK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HUF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/IDR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ILS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/INR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ISK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/JPY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/KRW"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/LTL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MAD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MTL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MXN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MYR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/NOK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/NZD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/PHP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/PLN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/RON"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SEK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SGD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SIT"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/THB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/TRY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/TWD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ZAR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(37 * 2, pairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs005()
		{
			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(new System.DateTime(2010, 1, 1));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "ARS/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "AUD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "BGN/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "BRL/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CAD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CHF/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CNY/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CYP/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "CZK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "DKK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "DZD/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EEK/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EGP/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "GBP/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "GRD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "HKD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "HRK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "HUF/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "IDR/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "ILS/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "INR/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "ISK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "JPY/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "KRW/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "LTL/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "MAD/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "MTL/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "MXN/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "MYR/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "NOK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "NZD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "PHP/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "PLN/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "RON/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "RUB/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "SEK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "SGD/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "SIT/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "THB/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "TRY/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "TWD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "USD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "ZAR/EUR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ARS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/AUD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/BGN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/BRL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CAD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CHF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CNY"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CYP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/CZK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/DKK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/DZD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/EEK"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/EGP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/GBP"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/GRD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HKD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HRK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/HUF"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/IDR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ILS"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/INR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ISK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/JPY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/KRW"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/LTL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MAD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MTL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MXN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/MYR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/NOK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/NZD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/PHP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/PLN"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/RON"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/RUB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SEK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SGD"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/SIT"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/THB"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/TRY"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/TWD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/USD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == "EUR/ZAR"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(37 * 2, pairs.Count());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetCurrencyPairs006()
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
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("CYP/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("EUR/CYP"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("EEK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("EUR/EEK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("GRD/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("EUR/GRD"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("EUR/LVL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("LVL/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("EUR/MTL"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("MTL/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("EUR/SKK"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("SKK/EUR"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("EUR/SIT"));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove("SIT/EUR"));
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove());
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove());
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove());
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove());
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove());
			//Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(CurrencyPairs.Remove());

			var Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pairCode in CurrencyPairs)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Any(P => P.ToString() == pairCode));
			}

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pairs.Count() == CurrencyPairs.Count);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate001()
		{
			Finance.ForeignExchange.EuropeanCentralBank Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > 0m);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate002()
		{
			Finance.ForeignExchange.EuropeanCentralBank Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				try
				{
					decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now.AddMinutes(10d));

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
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate003()
		{
			Finance.ForeignExchange.EuropeanCentralBank Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var pairs = Bank.GetCurrencyPairs(System.DateTime.Now.AddYears(-1));

			foreach (var pair in pairs)
			{
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now.AddYears(-1));

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(rate > 0m);
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GetExchangeRate004()
		{
			Finance.ForeignExchange.EuropeanCentralBank Bank = new Finance.ForeignExchange.EuropeanCentralBank();

			var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
			var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

			var pair = new CurrencyPair(AMD, ALL);

			try
			{
				decimal rate = Bank.GetExchangeRate(pair, System.DateTime.Now);

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
		public void KeepCurrenciesPairsUpdated()
		{
			// In case or failure, check currency pair information from ECB website and set deadline up to 3 month.

			System.DateTime Deadline = new System.DateTime(2015, 03, 1);

			if (System.DateTime.Now > Deadline)
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
		}
	}
}