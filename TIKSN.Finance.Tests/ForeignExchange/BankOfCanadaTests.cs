using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	[TestClass]
	public class BankOfCanadaTests
	{
		[TestMethod]
		public async Task Calculate001()
		{
			var bank = new BankOfCanada();

			foreach (var pair in await bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await bank.GetExchangeRateAsync(pair, DateTimeOffset.Now);
				Money After = await bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

				Assert.IsTrue(After.Amount == rate * Before.Amount);
				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task ConversionDirection001()
		{
			var Bank = new BankOfCanada();

			var CanadianDollar = new CurrencyInfo(new RegionInfo("CA"));
			var PoundSterling = new CurrencyInfo(new RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, CanadianDollar, DateTimeOffset.Now);

			Assert.IsTrue(BeforeInPound.Amount < AfterInDollar.Amount);
		}

		[TestMethod]
		public async Task ConvertCurrency001()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, decimal.One);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

				Assert.IsTrue(After.Amount > decimal.Zero);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency002()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, decimal.One);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

				Assert.IsTrue(After.Currency == pair.CounterCurrency);
			}
		}

		[TestMethod]
		public async Task ConvertCurrency003()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);

				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

				decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now);

				Assert.IsTrue(After.Currency == pair.CounterCurrency);
				Assert.IsTrue(After.Amount == rate * Before.Amount);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task ConvertCurrency004()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo US = new RegionInfo("US");
			RegionInfo CA = new RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			Money Before = new Money(USD, 100m);

			Money After = await Bank.ConvertCurrencyAsync(Before, CAD, DateTimeOffset.Now.AddMinutes(1d));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task ConvertCurrency005()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo US = new RegionInfo("US");
			RegionInfo CA = new RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			Money Before = new Money(USD, 100m);

			Money After = await Bank.ConvertCurrencyAsync(Before, CAD, DateTimeOffset.Now.AddDays(-20d));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task ConvertCurrency006()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo AO = new RegionInfo("AO");
			RegionInfo BW = new RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			Money Before = new Money(AOA, 100m);

			Money After = await Bank.ConvertCurrencyAsync(Before, BWP, DateTimeOffset.Now);
		}

		[TestMethod]
		public async Task CurrencyPairs001()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/USD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/ARS"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/AUD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/BRL"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/CLP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/CNY"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/COP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/HRK"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/CZK"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/DKK"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/EUR"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/GTQ"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/HNL"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/HKD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/HUF"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/ISK"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/INR"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/IDR"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/ILS"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/JMD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/JPY"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/MYR"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/MXN"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/MAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/MMK"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/NZD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/NOK"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PKR"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PAB"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PEN"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PHP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/PLN"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/RON"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/RUB"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/RSD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/SGD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/ZAR"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/KRW"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/LKR"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/SEK"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/CHF"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/TWD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/THB"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/TTD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/TND"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/TRY"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/AED"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/GBP"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/VEF"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/VND"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/XAF"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CAD/XCD"));

			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "USD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ARS/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "AUD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "BRL/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CLP/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CNY/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "COP/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HRK/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CZK/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "DKK/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "EUR/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "GTQ/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HNL/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HKD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "HUF/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ISK/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "INR/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "IDR/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ILS/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JMD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "JPY/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MYR/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MXN/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MAD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "MMK/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NZD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "NOK/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PKR/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PAB/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PEN/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PHP/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "PLN/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RON/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RUB/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "RSD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SGD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "ZAR/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "KRW/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "LKR/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "SEK/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "CHF/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TWD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "THB/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TTD/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TND/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "TRY/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "AED/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "GBP/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "VEF/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "VND/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "XAF/CAD"));
			Assert.IsTrue(CurrencyPairs.Any(C => C.ToString() == "XCD/CAD"));

			//Assert.AreEqual<int>(104, CurrencyPairs.Count());
		}

		[TestMethod]
		public async Task CurrencyPairs002()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.IsTrue(CurrencyPairs.Any(C => C == reversePair));
			}
		}

		[TestMethod]
		public async Task CurrencyPairs003()
		{
			BankOfCanada Bank = new BankOfCanada();

			var pairSet = new HashSet<CurrencyPair>();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				pairSet.Add(pair);
			}

			Assert.IsTrue(pairSet.Count == CurrencyPairs.Count());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task CurrencyPairs004()
		{
			BankOfCanada Bank = new BankOfCanada();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now.AddDays(-10));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task CurrencyPairs005()
		{
			BankOfCanada Bank = new BankOfCanada();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now.AddDays(10));
		}

		[TestMethod]
		public async Task Fetch001()
		{
			var Bank = new BankOfCanada();

			await Bank.FetchAsync();
		}

		[TestMethod]
		public async Task GetExchangeRate001()
		{
			BankOfCanada Bank = new BankOfCanada();

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now);

				Assert.IsTrue(rate > decimal.Zero);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetExchangeRate002()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo US = new RegionInfo("US");
			RegionInfo CA = new RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			CurrencyPair pair = new CurrencyPair(CAD, USD);

			decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now.AddMinutes(1d));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetExchangeRate003()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo US = new RegionInfo("US");
			RegionInfo CA = new RegionInfo("CA");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo CAD = new CurrencyInfo(CA);

			CurrencyPair pair = new CurrencyPair(CAD, USD);

			decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now.AddDays(-20d));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task GetExchangeRate004()
		{
			BankOfCanada Bank = new BankOfCanada();

			RegionInfo AO = new RegionInfo("AO");
			RegionInfo BW = new RegionInfo("BW");

			CurrencyInfo AOA = new CurrencyInfo(AO);
			CurrencyInfo BWP = new CurrencyInfo(BW);

			CurrencyPair pair = new CurrencyPair(BWP, AOA);

			decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now);
		}
	}
}