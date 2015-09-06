using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class SwissNationalBankTests
	{
		[TestMethod]
		public async Task Calculation001()
		{
			var Bank = new SwissNationalBank();

			var AtTheMoment = DateTimeOffset.Now;

			foreach (var pair in await Bank.GetCurrencyPairsAsync(AtTheMoment))
			{
				var Before = new Money(pair.BaseCurrency, 100m);

				var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, AtTheMoment);

				var rate = await Bank.GetExchangeRateAsync(pair, AtTheMoment);

				Assert.IsTrue(After.Amount == Before.Amount * rate);
				Assert.AreEqual<CurrencyInfo>(pair.CounterCurrency, After.Currency);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency001()
		{
			var Bank = new SwissNationalBank();

			var AtTheMoment = DateTimeOffset.Now;

			foreach (var Pair in await Bank.GetCurrencyPairsAsync(AtTheMoment))
			{
				var Before = new Money(Pair.BaseCurrency, 100m);
				var After = await Bank.ConvertCurrencyAsync(Before, Pair.CounterCurrency, AtTheMoment);

				Assert.IsTrue(After.Amount > decimal.Zero);
				Assert.AreEqual<CurrencyInfo>(Pair.CounterCurrency, After.Currency);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency002()
		{
			var Bank = new SwissNationalBank();

			var moment = DateTimeOffset.Now.AddMinutes(10d);

			foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
			{
				try
				{
					var Before = new Money(pair.BaseCurrency, 100m);

					var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, moment);

					Assert.Fail();
				}
				catch (ArgumentException)
				{
				}
			}
		}

		[TestMethod]
		public async Task ConvertCurrency004()
		{
			var Bank = new SwissNationalBank();

			var moment = DateTimeOffset.Now.AddDays(-10d);

			foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
			{
				try
				{
					var Before = new Money(pair.BaseCurrency, 100m);

					var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, moment);

					Assert.Fail();
				}
				catch (ArgumentException)
				{
				}
			}
		}

		[TestMethod]
		public async Task CounterCurrency003()
		{
			var Bank = new SwissNationalBank();

			RegionInfo AO = new RegionInfo("AO");
			RegionInfo BW = new RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			try
			{
				var Before = new Money(AOA, 100m);

				var After = await Bank.ConvertCurrencyAsync(Before, BWP, DateTimeOffset.Now);

				Assert.Fail();
			}
			catch (ArgumentException)
			{
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs001()
		{
			var Bank = new SwissNationalBank();

			var moment = DateTimeOffset.Now.AddMinutes(10d);

			try
			{
				var pairs = await Bank.GetCurrencyPairsAsync(moment);

				Assert.Fail();
			}
			catch (ArgumentException)
			{
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs002()
		{
			var Bank = new SwissNationalBank();

			var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			var DistinctPairs = Pairs.Distinct();

			Assert.IsTrue(Pairs.Count() == DistinctPairs.Count());
		}

		[TestMethod]
		public async Task GetCurrencyPairs003()
		{
			var Bank = new SwissNationalBank();

			var moment = DateTimeOffset.Now.AddDays(-10d);

			try
			{
				var pairs = await Bank.GetCurrencyPairsAsync(moment);

				Assert.Fail();
			}
			catch (ArgumentException)
			{
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs004()
		{
			var Bank = new SwissNationalBank();

			var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (var pair in Pairs)
			{
				var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.IsTrue(Pairs.Any(P => P == reversed));
			}
		}

		[TestMethod]
		public async Task GetCurrencyPairs005()
		{
			var Bank = new SwissNationalBank();

			var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			Assert.IsTrue(Pairs.Any(P => P.ToString() == "EUR/CHF"));
			Assert.IsTrue(Pairs.Any(P => P.ToString() == "USD/CHF"));
			Assert.IsTrue(Pairs.Any(P => P.ToString() == "JPY/CHF"));
			Assert.IsTrue(Pairs.Any(P => P.ToString() == "GBP/CHF"));

			Assert.IsTrue(Pairs.Any(P => P.ToString() == "CHF/EUR"));
			Assert.IsTrue(Pairs.Any(P => P.ToString() == "CHF/USD"));
			Assert.IsTrue(Pairs.Any(P => P.ToString() == "CHF/JPY"));
			Assert.IsTrue(Pairs.Any(P => P.ToString() == "CHF/GBP"));

			Assert.AreEqual<int>(8, Pairs.Count());
		}

		[TestMethod]
		public async Task GetExchangeRate001()
		{
			var Bank = new SwissNationalBank();

			var AtTheMoment = DateTimeOffset.Now;

			foreach (var Pair in await Bank.GetCurrencyPairsAsync(AtTheMoment))
			{
				decimal rate = await Bank.GetExchangeRateAsync(Pair, AtTheMoment);

				Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task GetExchangeRate002()
		{
			var Bank = new SwissNationalBank();

			var moment = DateTimeOffset.Now.AddMinutes(10d);

			foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
			{
				try
				{
					decimal rate = await Bank.GetExchangeRateAsync(pair, moment);

					Assert.Fail();
				}
				catch (ArgumentException)
				{
				}
			}
		}

		[TestMethod]
		public async Task GetExchangeRate003()
		{
			var Bank = new SwissNationalBank();

			RegionInfo AO = new RegionInfo("AO");
			RegionInfo BW = new RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			CurrencyPair Pair = new CurrencyPair(AOA, BWP);

			try
			{
				var rate = await Bank.GetExchangeRateAsync(Pair, DateTimeOffset.Now);

				Assert.Fail();
			}
			catch (ArgumentException)
			{
			}
		}

		[TestMethod]
		public async Task GetExchangeRate004()
		{
			var Bank = new SwissNationalBank();

			var moment = DateTimeOffset.Now.AddDays(-10d);

			foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
			{
				try
				{
					decimal rate = await Bank.GetExchangeRateAsync(pair, moment);

					Assert.Fail();
				}
				catch (ArgumentException)
				{
				}
			}
		}
	}
}