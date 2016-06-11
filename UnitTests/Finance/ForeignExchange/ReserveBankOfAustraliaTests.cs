using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	public class ReserveBankOfAustraliaTests
	{
		[Fact]
		public async Task ConversionDirection001()
		{
			var Bank = new ReserveBankOfAustralia();

			var AustralianDollar = new CurrencyInfo(new System.Globalization.RegionInfo("AU"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, AustralianDollar, System.DateTime.Now);

			Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[Fact]
		public async Task ConvertCurrency001()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.True(After.Amount > decimal.Zero);
				Assert.True(After.Currency == pair.CounterCurrency);
			}
		}

		[Fact]
		public async Task ConvertCurrency002()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				decimal rate = await Bank.GetExchangeRateAsync(pair, System.DateTime.Now);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, System.DateTime.Now);

				Assert.True(After.Amount == Before.Amount * rate);
			}
		}

		[Fact]
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

		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task Fetch001()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			await Bank.FetchAsync();
		}

		[Fact]
		public async Task GetCurrencyPairs001()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			Assert.True(CurrencyPairs.Any(P => P.ToString() == "USD/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "CNY/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "JPY/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "EUR/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "KRW/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "GBP/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "SGD/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "INR/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "THB/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "NZD/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "TWD/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "MYR/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "IDR/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "VND/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AED/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "HKD/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "CAD/AUD"));
			//Assert.True(CurrencyPairs.Any(P => P.ToString() == "ZAR/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "CHF/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "PGK/AUD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "XDR/AUD"));
			//Assert.True(CurrencyPairs.Any(P => P.ToString() == "SEK/AUD"));
			//Assert.True(CurrencyPairs.Any(P => P.ToString() == "PHP/AUD"));

			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/USD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/CNY"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/JPY"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/EUR"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/KRW"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/GBP"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/SGD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/INR"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/THB"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/NZD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/TWD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/MYR"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/IDR"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/VND"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/AED"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/HKD"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/CAD"));
			//Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/ZAR"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/CHF"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/PGK"));
			Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/XDR"));
			//Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/SEK"));
			//Assert.True(CurrencyPairs.Any(P => P.ToString() == "AUD/PHP"));

			//Assert.True(CurrencyPairs.Count() == 38);
		}

		[Fact]
		public async Task GetCurrencyPairs002()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.True(CurrencyPairs.Any(P => P == reversedPair));
			}
		}

		[Fact]
		public async Task GetCurrencyPairs003()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			System.Collections.Generic.HashSet<CurrencyPair> PairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				PairSet.Add(pair);
			}

			Assert.True(PairSet.Count == CurrencyPairs.Count());
		}

		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task GetExchangeRate001()
		{
			ReserveBankOfAustralia Bank = new ReserveBankOfAustralia();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(System.DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Assert.True(await Bank.GetExchangeRateAsync(pair, System.DateTime.Now) > decimal.Zero);
			}
		}

		[Fact]
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

		[Fact]
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

		[Fact]
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
	}
}