using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
	public class CentralBankOfArmeniaTests
	{
		private readonly ICurrencyFactory _currencyFactory;

		public CentralBankOfArmeniaTests()
		{
			var services = new ServiceCollection();
			services.AddMemoryCache();
			services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
			services.AddSingleton<IRegionFactory, RegionFactory>();

			var serviceProvider = services.BuildServiceProvider();
			_currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
		}

		[Fact]
		public async Task ConversionDirection001()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			var ArmenianDram = new CurrencyInfo(new RegionInfo("AM"));
			var PoundSterling = new CurrencyInfo(new RegionInfo("GB"));

			var BeforeInPound = new Money(PoundSterling, 100m);

			var AfterInDram = await Bank.ConvertCurrencyAsync(BeforeInPound, ArmenianDram, DateTime.Now);

			Assert.True(BeforeInPound.Amount < AfterInDram.Amount);
		}

		[Fact]
		public async Task ConvertCurrency001()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (var pair in CurrencyPairs)
			{
				Money Initial = new Money(pair.BaseCurrency, 10m);
				decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);
				Money Result = await Bank.ConvertCurrencyAsync(Initial, pair.CounterCurrency, DateTime.Now);

				Assert.True(Result.Currency == pair.CounterCurrency);
				Assert.True(Result.Amount > 0m);
				Assert.True(Result.Amount == (rate * Initial.Amount));
			}
		}

		[Fact]
		public async Task ConvertCurrency002()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			RegionInfo UnitedStates = new RegionInfo("US");
			RegionInfo Armenia = new RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			await
					Assert.ThrowsAsync<ArgumentException>(
						async () =>
							await Bank.ConvertCurrencyAsync(Before, Dram, DateTime.Now.AddDays(1d)));
		}

		[Fact]
		public async Task ConvertCurrency003()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			RegionInfo UnitedStates = new RegionInfo("US");
			RegionInfo Armenia = new RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			await
				Assert.ThrowsAsync<ArgumentException>(
					async () =>
						await Bank.ConvertCurrencyAsync(Before, Dram, DateTime.Now.AddMinutes(1d)));
		}

		[Fact]
		public async Task ConvertCurrency004()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			RegionInfo UnitedStates = new RegionInfo("US");
			RegionInfo Armenia = new RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Before = new Money(Dollar, 100m);

			await
				Assert.ThrowsAsync<ArgumentException>(
					async () =>
						await Bank.ConvertCurrencyAsync(Before, Dram, DateTime.Now.AddDays(-20d)));
		}

		[Fact]
		public async Task Fetch001()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			await Bank.GetExchangeRatesAsync(DateTimeOffset.Now);
		}

		[Fact]
		public async Task Fetch002()
		{
			throw new NotImplementedException();

			//var ci = new CultureInfo("ru-RU");
			//Thread.CurrentThread.CurrentCulture = ci;
			//Thread.CurrentThread.CurrentUICulture = ci;

			//Finance.ForeignExchange.CentralBankOfArmenia Bank = new Finance.ForeignExchange.CentralBankOfArmenia();

			//await Bank.FetchAsync();
		}

		[Fact]
		public async Task GetCurrencyPairs001()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			System.Collections.Generic.HashSet<CurrencyPair> pairs = new System.Collections.Generic.HashSet<CurrencyPair>();

			foreach (CurrencyPair pair in await Bank.GetCurrencyPairsAsync(DateTime.Now))
			{
				pairs.Add(pair);
			}

			Assert.True(pairs.Count == (await Bank.GetCurrencyPairsAsync(DateTime.Now)).Count());
		}

		[Fact]
		public async Task GetCurrencyPairs002()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "USD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "USD"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "GBP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "GBP"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AUD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "AUD"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ARS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ARS"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "DKK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "DKK"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "EGP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "EGP"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "EUR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "EUR"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TRY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TRY"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "IRR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "IRR"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ILS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ILS"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "PLN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "PLN"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "LBP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LBP"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CAD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CAD"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "INR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "INR"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "HUF" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "HUF"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "JPY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "JPY"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "NOK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "NOK"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SEK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SEK"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CHF" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CHF"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CZK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CZK"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "CNY" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CNY"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SGD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SGD"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KRW" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KRW"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "MXN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "MXN"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SAR" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SAR"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "SYP" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SYP"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AED" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "AED"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KWD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KWD"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BGN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BGN"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "RON" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "RON"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "ISK" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ISK"));

			// Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "LVL" &&
			// C.CounterCurrency.ISOCurrencySymbol == "AMD")); Assert.True(CurrencyPairs.Any(C =>
			// C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LVL"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KGS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KGS"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "KZT" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KZT"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "MDL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "MDL"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "RUB" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "RUB"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "UAH" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "UAH"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "UZS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "UZS"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BYN" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BYN"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TJS" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TJS"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "TMT" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TMT"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "GEL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "GEL"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "HKD" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "HKD"));

			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "BRL" && C.CounterCurrency.ISOCurrencySymbol == "AMD"));
			Assert.True(CurrencyPairs.Any(C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BRL"));

			//Assert.Equal(88, CurrencyPairs.Count());
		}

		[Fact]
		public async Task GetCurrencyPairs003()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reverse = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.True(CurrencyPairs.Any(C => C == reverse));
			}
		}

		[Fact]
		public async Task GetExchangeRate001()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				Assert.True(await Bank.GetExchangeRateAsync(pair, DateTime.Now) > decimal.Zero);
			}
		}

		[Fact]
		public async Task GetExchangeRate002()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

			foreach (CurrencyPair pair in CurrencyPairs)
			{
				CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

				Assert.Equal<decimal>(decimal.One, System.Math.Round(await Bank.GetExchangeRateAsync(pair, DateTime.Now) * await Bank.GetExchangeRateAsync(reversePair, DateTime.Now), 5));
			}
		}

		[Fact]
		public async Task GetExchangeRate003()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			RegionInfo UnitedStates = new RegionInfo("US");
			RegionInfo Armenia = new RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			await
				Assert.ThrowsAsync<ArgumentException>(
					async () =>
						await Bank.GetExchangeRateAsync(DollarPerDram, DateTime.Now.AddDays(1d)));
		}

		[Fact]
		public async Task GetExchangeRate004()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			RegionInfo UnitedStates = new RegionInfo("US");
			RegionInfo Armenia = new RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			await
				Assert.ThrowsAsync<ArgumentException>(
					async () =>
						await Bank.GetExchangeRateAsync(DollarPerDram, DateTime.Now.AddDays(-20d)));
		}

		[Fact]
		public async Task GetExchangeRate005()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			RegionInfo UnitedStates = new RegionInfo("US");
			RegionInfo Armenia = new RegionInfo("AM");

			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DollarPerDram = new CurrencyPair(Dollar, Dram);

			await
				Assert.ThrowsAsync<ArgumentException>(
					async () =>
						await Bank.GetExchangeRateAsync(DollarPerDram, DateTime.Now.AddMinutes(1d)));
		}

		[Fact]
		public async Task GetExchangeRate006()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			RegionInfo Albania = new RegionInfo("AL");
			RegionInfo Armenia = new RegionInfo("AM");

			CurrencyInfo Lek = new CurrencyInfo(Albania);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair LekPerDram = new CurrencyPair(Lek, Dram);

			await
				Assert.ThrowsAsync<ArgumentException>(
					async () =>
						await Bank.GetExchangeRateAsync(LekPerDram, DateTime.Now));
		}

		[Fact]
		public async Task GetExchangeRate007()
		{
			var Bank = new CentralBankOfArmenia(_currencyFactory);

			RegionInfo Albania = new RegionInfo("AL");
			RegionInfo Armenia = new RegionInfo("AM");

			CurrencyInfo Lek = new CurrencyInfo(Albania);
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			CurrencyPair DramPerLek = new CurrencyPair(Dram, Lek);

			await
				Assert.ThrowsAsync<ArgumentException>(
					async () =>
						await Bank.GetExchangeRateAsync(DramPerLek, DateTime.Now));
		}
	}
}