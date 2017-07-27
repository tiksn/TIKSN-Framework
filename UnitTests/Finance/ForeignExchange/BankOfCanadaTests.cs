using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	public class BankOfCanadaTests
	{
		[Fact]
		public async Task Calculate001()
		{
			var bank = new BankOfCanada();

			foreach (var pair in await bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await bank.GetExchangeRateAsync(pair, DateTimeOffset.Now);
				Money After = await bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

				Assert.True(After.Amount == rate * Before.Amount);
				Assert.True(After.Currency == pair.CounterCurrency);
			}
		}

		[Fact]
		public async Task ConversionDirection001()
		{
			var Bank = new BankOfCanada();

			var CanadianDollar = new CurrencyInfo(new RegionInfo("CA"));
			var PoundSterling = new CurrencyInfo(new RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, CanadianDollar, DateTimeOffset.Now);

			Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[Fact]
		public async Task ConvertCurrency001()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, decimal.One);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

				Assert.True(After.Amount > decimal.Zero);
			}
		}

		[Fact]
		public async Task ConvertCurrency002()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, decimal.One);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

				Assert.True(After.Currency == pair.CounterCurrency);
			}
		}

		[Fact]
		public async Task ConvertCurrency003()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

				decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now);

				Assert.True(After.Currency == pair.CounterCurrency);
				Assert.True(After.Amount == rate * Before.Amount);
			}
		}

		[Fact]
		public async Task ConvertCurrency004()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo US = new RegionInfo("US");
			RegionInfo CA = new RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			Money Before = new Money(USD, 100m);

			await Assert.ThrowsAsync<ArgumentException>(
				async () => await Bank.ConvertCurrencyAsync(Before, CAD, DateTimeOffset.Now.AddMinutes(1d)));
		}

		[Fact]
		public async Task ConvertCurrency006()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo AO = new RegionInfo("AO");
			RegionInfo BW = new RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			Money Before = new Money(AOA, 100m);

			await Assert.ThrowsAsync<ArgumentException>(
				async () => await Bank.ConvertCurrencyAsync(Before, BWP, DateTimeOffset.Now));
		}

		[Fact]
		public async Task CurrencyPairs001()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/USD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/AUD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/BRL"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/CNY"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/EUR"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/HKD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/INR"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/IDR"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/JPY"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/MYR"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/MXN"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/NZD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/NOK"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/PEN"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/RUB"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/SGD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/ZAR"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/KRW"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/SEK"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/CHF"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/TWD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/THB"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/TRY"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/GBP"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/VND"));

			Assert.True(CurrencyPairs.Any(C => C.ToString() == "USD/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "AUD/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "BRL/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CNY/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "EUR/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "HKD/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "INR/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "IDR/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "JPY/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "MYR/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "MXN/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "NZD/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "NOK/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "PEN/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "RUB/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "SGD/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "ZAR/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "KRW/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "SEK/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "CHF/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "TWD/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "THB/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "TRY/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "GBP/CAD"));
			Assert.True(CurrencyPairs.Any(C => C.ToString() == "VND/CAD"));
		}

		[Fact]
		public async Task CurrencyPairs002()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.True(CurrencyPairs.Any(C => C == reversePair));
			}
		}

		[Fact]
		public async Task CurrencyPairs003()
		{
			BankOfCanada Bank = new BankOfCanada();

			var pairSet = new HashSet<CurrencyPair>();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				pairSet.Add(pair);
			}

			Assert.True(pairSet.Count == CurrencyPairs.Count());
		}

		[Fact]
		public async Task CurrencyPairs005()
		{
			BankOfCanada Bank = new BankOfCanada();

			await Assert.ThrowsAsync<ArgumentException>(
				async () => await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now.AddDays(10)));
		}

		[Fact]
		public async Task Fetch001()
		{
			var Bank = new BankOfCanada();

			await Bank.FetchAsync();
		}

		[Fact]
		public async Task GetExchangeRate001()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now);

				Assert.True(rate > decimal.Zero);
			}
		}

		[Fact]
		public async Task GetExchangeRate002()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo US = new RegionInfo("US");
			RegionInfo CA = new RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			CurrencyPair pair = new CurrencyPair(CAD, USD);

			await Assert.ThrowsAsync<ArgumentException>(
				async () => await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now.AddMinutes(1d)));
		}

		[Fact]
		public async Task GetExchangeRate004()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo AO = new RegionInfo("AO");
			RegionInfo BW = new RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			CurrencyPair pair = new CurrencyPair(BWP, AOA);

			await Assert.ThrowsAsync<ArgumentException>(
				async () => await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now));
		}
	}
}