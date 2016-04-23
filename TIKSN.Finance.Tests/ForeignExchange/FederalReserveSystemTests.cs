using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class FederalReserveSystemTests
	{
		[TestMethod]
		public async Task Calculation001()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				var Before = new Money(pair.BaseCurrency);
				var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount == rate * Before.Amount);
			}
		}

		[TestMethod]
		public async Task ConversionDirection001()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var USDollar = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, USDollar, System.DateTime.Now);

			Assert.IsTrue(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[TestMethod]
		public async Task ConvertCurrency001()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			foreach (var pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				var Before = new Money(pair.BaseCurrency, 10m);
				var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.IsTrue(After.Amount > decimal.Zero);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency002()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var LastYear = System.DateTime.Now.AddYears(-1);

			foreach (var pair in await Bank.GetCurrencyPairsAsync(LastYear))
			{
				var Before = new Money(pair.BaseCurrency, 10m);
				var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, LastYear);

				Assert.IsTrue(After.Amount > decimal.Zero);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency003()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			foreach (var pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				var Before = new Money(pair.BaseCurrency, 10m);

				try
				{
					var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now.AddMinutes(1d));

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
		public async Task ConvertCurrency004()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var Before = new Money(new CurrencyInfo(new System.Globalization.RegionInfo("AL")), 10m);

			try
			{
				var After = await Bank.ConvertCurrencyAsync(Before, new CurrencyInfo(new System.Globalization.RegionInfo("AM")), System.DateTime.Now.AddMinutes(1d));

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
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (var pair in pairs)
			{
				var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.IsTrue(pairs.Any(C => C == reversed));
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs002()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (var pair in pairs)
			{
				Assert.IsTrue(uniquePairs.Add(pair));
			}

			Assert.IsTrue(uniquePairs.Count == pairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs003()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			try
			{
				var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now.AddMinutes(1d));

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
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			Assert.IsTrue(pairs.Any(C => C.ToString() == "AUD/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "BRL/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CAD/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CNY/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "DKK/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "EUR/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "HKD/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "INR/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "JPY/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "MYR/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "MXN/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "NZD/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "NOK/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "SGD/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "ZAR/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "KRW/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "LKR/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "SEK/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "CHF/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "TWD/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "THB/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "GBP/USD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "VEF/USD"));

			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/AUD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/BRL"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/CAD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/CNY"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/DKK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/EUR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/HKD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/INR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/JPY"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/MYR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/MXN"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/NZD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/NOK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/SGD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/ZAR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/KRW"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/LKR"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/SEK"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/CHF"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/TWD"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/THB"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/GBP"));
			Assert.IsTrue(pairs.Any(C => C.ToString() == "USD/VEF"));

			Assert.AreEqual<int>(23 * 2, pairs.Count());
		}

		[TestMethod]
		public async Task GetExchangeRate001()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			foreach (var pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate002()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var LastYear = System.DateTime.Now.AddYears(-1);

			foreach (var pair in await Bank.GetCurrencyPairsAsync(LastYear))
			{
				var rate = await Bank.GetExchangeRateAsync(pair, LastYear);

				Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate003()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			foreach (var pair in await Bank.GetCurrencyPairsAsync(System.DateTime.Now))
			{
				try
				{
					var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now.AddMinutes(1d));

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
		public async Task GetExchangeRate004()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("AL")), new CurrencyInfo(new System.Globalization.RegionInfo("AM")));

			try
			{
				var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now.AddMinutes(1d));

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
		public async Task GetExchangeRate005()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("CN")));

			var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

			Assert.IsTrue(rate > decimal.One);
		}

		[TestMethod]
		public async Task GetExchangeRate006()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("SG")));

			var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

			Assert.IsTrue(rate > decimal.One);
		}

		[TestMethod]
		public async Task GetExchangeRate007()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("DE")));

			var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

			Assert.IsTrue(rate < decimal.One);
		}

		[TestMethod]
		public async Task GetExchangeRate008()
		{
			var Bank = new Finance.ForeignExchange.FederalReserveSystem();

			var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("GB")));

			var rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

			Assert.IsTrue(rate < decimal.One);
		}
	}
}