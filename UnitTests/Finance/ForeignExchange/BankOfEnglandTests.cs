using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    public class BankOfEnglandTests
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly IRegionFactory _regionFactory;

        public BankOfEnglandTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            services.AddSingleton<IRegionFactory, RegionFactory>();

            var serviceProvider = services.BuildServiceProvider();
            _currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
            _regionFactory = serviceProvider.GetRequiredService<IRegionFactory>();
        }

        [Fact]
        public async Task Calculate001()
        {
            var Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

                Assert.True(After.Amount == rate * Before.Amount);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task Calculate002()
        {
            var Bank = new BankOfEngland(_currencyFactory, _regionFactory);
            var TenYearsAgo = DateTimeOffset.Now.AddYears(-10);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(TenYearsAgo))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, TenYearsAgo);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, TenYearsAgo);

                Assert.True(After.Amount == rate * Before.Amount);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConversionDirection001()
        {
            var Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            var USDollar = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, USDollar, DateTimeOffset.Now);

            Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now);

                Assert.True(After.Amount > decimal.Zero);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

            CurrencyPair pair = CurrencyPairs.First();

            Money Before = new Money(pair.BaseCurrency, 10m);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now.AddMinutes(1d)));
        }

        [Fact]
        public async Task ConvertCurrency003()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            CurrencyPair pair = new CurrencyPair(
                new CurrencyInfo(new System.Globalization.RegionInfo("AM")),
                new CurrencyInfo(new System.Globalization.RegionInfo("BY")));

            Money Before = new Money(pair.BaseCurrency, 10m);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now));
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "AUD/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "AUD/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CAD/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CNY/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CNY/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CZK/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CZK/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CZK/EUR"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "DKK/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "DKK/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "DKK/EUR"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "EUR/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "HKD/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "HKD/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "HUF/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "HUF/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "HUF/EUR"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "INR/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "INR/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "ILS/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "ILS/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "JPY/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "JPY/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "JPY/EUR"));

            //Assert.True(CurrencyPairs.Any(C => C.ToString() == "LVL/USD"));
            //Assert.True(CurrencyPairs.Any(C => C.ToString() == "LVL/GBP"));
            //Assert.True(CurrencyPairs.Any(C => C.ToString() == "LVL/EUR"));

            //Assert.True(CurrencyPairs.Any(C => C.ToString() == "LTL/USD"));
            //Assert.True(CurrencyPairs.Any(C => C.ToString() == "LTL/GBP"));
            //Assert.True(CurrencyPairs.Any(C => C.ToString() == "LTL/EUR"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "MYR/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "MYR/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "NZD/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "NZD/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "NOK/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "NOK/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "PLN/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "PLN/EUR"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "PLN/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "RUB/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "RUB/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "SAR/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "SAR/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "SGD/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "SGD/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "ZAR/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "ZAR/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "KRW/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "KRW/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "GBP/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "SEK/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "SEK/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CHF/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CHF/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "CHF/EUR"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "TWD/USD"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "TWD/GBP"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "THB/GBP"));
            Assert.True(CurrencyPairs.Any(C => C.ToString() == "THB/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "TRY/USD"));

            Assert.True(CurrencyPairs.Any(C => C.ToString() == "USD/GBP"));

            //Assert.Equal(61, (await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now)).Count());
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

            var UniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in CurrencyPairs)
            {
                UniquePairs.Add(pair);
            }

            Assert.True(UniquePairs.Count == CurrencyPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs003()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now.AddYears(-10));

            var UniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in CurrencyPairs)
            {
                UniquePairs.Add(pair);
            }

            Assert.True(UniquePairs.Count == CurrencyPairs.Count());
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            foreach (CurrencyPair pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
            {
                Assert.True(await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now) > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.GetExchangeRateAsync(CurrencyPairs.First(), DateTimeOffset.Now.AddMinutes(1d)));
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory);

            CurrencyPair pair = new CurrencyPair(
                new CurrencyInfo(new System.Globalization.RegionInfo("AM")),
                new CurrencyInfo(new System.Globalization.RegionInfo("BY")));

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now));
        }

        [Fact]
        public async Task KeepCurrenciesPairsUpdated()
        {
            // In case or failure, check currency pair information from BOE website and set deadline
            // up to 3 month.

            System.DateTimeOffset Deadline = new DateTime(2016, 7, 1);

            if (System.DateTimeOffset.Now > Deadline)
                throw new Exception("Source is out of date. Please update.");
        }
    }
}