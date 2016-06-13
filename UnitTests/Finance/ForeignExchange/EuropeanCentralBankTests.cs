using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	public class EuropeanCentralBankTests
	{
		[Fact]
		public async Task Calculation001()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now);

				Assert.True(After.Amount == Before.Amount * rate);
				Assert.True(After.Currency == pair.CounterCurrency);
			}
		}

		[Fact]
		public async Task Calculation002()
		{
			var Bank = new EuropeanCentralBank();

			var OneYearsAgo = DateTime.Now.AddYears(-1);
			var pairs = await Bank.GetCurrencyPairsAsync(OneYearsAgo);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, OneYearsAgo);
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, OneYearsAgo);

				Assert.True(After.Amount == Before.Amount * rate);
				Assert.True(After.Currency == pair.CounterCurrency);
			}
		}

		[Fact]
		public async Task ConversionDirection001()
		{
			var Bank = new EuropeanCentralBank();

			var Euro = new CurrencyInfo(new System.Globalization.RegionInfo("DE"));
			var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

			var BeforeInEuro = new Money(Euro, 100m);

			var AfterInPound = await Bank.ConvertCurrencyAsync(BeforeInEuro, PoundSterling, DateTime.Now);

			Assert.True(BeforeInEuro.Amount > AfterInPound.Amount);
		}

		[Fact]
		public async Task ConvertCurrency001()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (var pair in pairs)
			{
				Money Before = new Money(pair.BaseCurrency, 10m);
				Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now);

				Assert.True(After.Amount > 0m);
			}
		}

		[Fact]
		public async Task ConvertCurrency002()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (var pair in pairs)
			{
                Money Before = new Money(pair.BaseCurrency, 10m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddMinutes(10d)));
			}
		}

		[Fact]
		public async Task ConvertCurrency003()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
			var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

			Money Before = new Money(AMD, 10m);

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.ConvertCurrencyAsync(Before, ALL, DateTime.Now));
		}

		[Fact]
		public async Task GetCurrencyPairs001()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (var pair in pairs)
			{
				var ReversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.True(pairs.Any(P => P == ReversedPair));
			}
		}

		[Fact]
		public async Task GetCurrencyPairs002()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);
			var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (var pair in pairs)
			{
				bool WasUnique = uniquePairs.Add(pair);

				if (!WasUnique)
				{
					System.Diagnostics.Debug.WriteLine(pair);
				}
			}

			Assert.True(uniquePairs.Count == pairs.Count());
		}

		[Fact]
		public async Task GetCurrencyPairs003()
		{
			var Bank = new EuropeanCentralBank();

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetCurrencyPairsAsync(DateTime.Now.AddMinutes(10d)));
		}

		[Fact]
		public async Task GetCurrencyPairs004()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			//Assert.True(pairs.Any(P => P.ToString() == "ARS/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "AUD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "BGN/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "BRL/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "CAD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "CHF/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "CNY/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "CYP/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "CZK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "DKK/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "DZD/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "EEK/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "EGP/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "GBP/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "GRD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "HKD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "HRK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "HUF/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "IDR/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "ILS/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "INR/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "ISK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "JPY/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "KRW/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "LTL/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "MAD/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "MTL/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "MXN/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "MYR/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "NOK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "NZD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "PHP/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "PLN/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "RON/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "RUB/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "SEK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "SGD/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "SIT/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "THB/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "TRY/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "TWD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "USD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "ZAR/EUR"));

			//Assert.True(pairs.Any(P => P.ToString() == "EUR/ARS"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/AUD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/BGN"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/BRL"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/CAD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/CHF"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/CNY"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/CYP"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/CZK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/DKK"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/DZD"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/EEK"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/EGP"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/GBP"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/GRD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/HKD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/HRK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/HUF"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/IDR"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/ILS"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/INR"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/ISK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/JPY"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/KRW"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/LTL"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/MAD"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/MTL"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/MXN"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/MYR"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/NOK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/NZD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/PHP"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/PLN"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/RON"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/RUB"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/SEK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/SGD"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/SIT"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/THB"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/TRY"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/TWD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/USD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/ZAR"));

			//Assert.Equal(37 * 2, pairs.Count());
		}

		[Fact]
		public async Task GetCurrencyPairs005()
		{
			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(new System.DateTime(2010, 1, 1));

			//Assert.True(pairs.Any(P => P.ToString() == "ARS/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "AUD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "BGN/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "BRL/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "CAD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "CHF/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "CNY/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "CYP/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "CZK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "DKK/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "DZD/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "EEK/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "EGP/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "GBP/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "GRD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "HKD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "HRK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "HUF/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "IDR/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "ILS/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "INR/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "ISK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "JPY/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "KRW/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "LTL/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "MAD/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "MTL/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "MXN/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "MYR/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "NOK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "NZD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "PHP/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "PLN/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "RON/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "RUB/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "SEK/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "SGD/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "SIT/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "THB/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "TRY/EUR"));
			//Assert.True(pairs.Any(P => P.ToString() == "TWD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "USD/EUR"));
			Assert.True(pairs.Any(P => P.ToString() == "ZAR/EUR"));

			//Assert.True(pairs.Any(P => P.ToString() == "EUR/ARS"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/AUD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/BGN"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/BRL"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/CAD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/CHF"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/CNY"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/CYP"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/CZK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/DKK"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/DZD"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/EEK"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/EGP"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/GBP"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/GRD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/HKD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/HRK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/HUF"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/IDR"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/ILS"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/INR"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/ISK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/JPY"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/KRW"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/LTL"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/MAD"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/MTL"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/MXN"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/MYR"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/NOK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/NZD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/PHP"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/PLN"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/RON"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/RUB"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/SEK"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/SGD"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/SIT"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/THB"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/TRY"));
			//Assert.True(pairs.Any(P => P.ToString() == "EUR/TWD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/USD"));
			Assert.True(pairs.Any(P => P.ToString() == "EUR/ZAR"));

			//Assert.Equal(37 * 2, pairs.Count());
		}

		[Fact]
		public async Task GetCurrencyPairs006()
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

			// Replaced by Euro or not supported anymore.
			Assert.True(CurrencyPairs.Remove("CYP/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/CYP"));
			Assert.True(CurrencyPairs.Remove("EEK/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/EEK"));
			Assert.True(CurrencyPairs.Remove("GRD/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/GRD"));
			Assert.True(CurrencyPairs.Remove("EUR/LVL"));
			Assert.True(CurrencyPairs.Remove("LVL/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/MTL"));
			Assert.True(CurrencyPairs.Remove("MTL/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/SKK"));
			Assert.True(CurrencyPairs.Remove("SKK/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/SIT"));
			Assert.True(CurrencyPairs.Remove("SIT/EUR"));

			Assert.True(CurrencyPairs.Remove("EUR/ARS"));
			Assert.True(CurrencyPairs.Remove("ARS/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/DZD"));
			Assert.True(CurrencyPairs.Remove("DZD/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/ISK"));
			Assert.True(CurrencyPairs.Remove("ISK/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/LTL"));
			Assert.True(CurrencyPairs.Remove("LTL/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/MAD"));
			Assert.True(CurrencyPairs.Remove("MAD/EUR"));
			Assert.True(CurrencyPairs.Remove("EUR/TWD"));
			Assert.True(CurrencyPairs.Remove("TWD/EUR"));
			//Assert.True(CurrencyPairs.Remove(""));
			//Assert.True(CurrencyPairs.Remove(""));
			//Assert.True(CurrencyPairs.Remove(""));
			//Assert.True(CurrencyPairs.Remove(""));
			//Assert.True(CurrencyPairs.Remove(""));

			var Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (var pairCode in CurrencyPairs)
			{
				Assert.True(pairs.Any(P => P.ToString() == pairCode));
			}

			Assert.True(pairs.Count() == CurrencyPairs.Count);
		}

		[Fact]
		public async Task GetExchangeRate001()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (var pair in pairs)
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);

				Assert.True(rate > 0m);
			}
		}

		[Fact]
		public async Task GetExchangeRate002()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (var pair in pairs)
			{
                await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(10d)));
			}
		}

		[Fact]
		public async Task GetExchangeRate003()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now.AddYears(-1));

			foreach (var pair in pairs)
			{
				decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddYears(-1));

				Assert.True(rate > 0m);
			}
		}

		[Fact]
		public async Task GetExchangeRate004()
		{
			EuropeanCentralBank Bank = new EuropeanCentralBank();

			var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
			var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

			var pair = new CurrencyPair(AMD, ALL);

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetExchangeRateAsync(pair, DateTime.Now));
		}
	}
}