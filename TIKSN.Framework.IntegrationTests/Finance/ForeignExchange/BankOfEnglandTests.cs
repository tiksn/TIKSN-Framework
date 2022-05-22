using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    public class BankOfEnglandTests
    {
        const string skip = "API changed, code needs to be adopted";

        private readonly ICurrencyFactory _currencyFactory;
        private readonly IRegionFactory _regionFactory;
        private readonly ITimeProvider _timeProvider;

        public BankOfEnglandTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            services.AddSingleton<IRegionFactory, RegionFactory>();
            services.AddSingleton<ITimeProvider, TimeProvider>();

            var serviceProvider = services.BuildServiceProvider();
            _currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
            _regionFactory = serviceProvider.GetRequiredService<IRegionFactory>();
            _timeProvider = serviceProvider.GetRequiredService<ITimeProvider>();
        }

        [Fact(Skip = skip)]
        public async Task Calculate001()
        {
            var Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default);

                Assert.True(After.Amount == rate * Before.Amount);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact(Skip = skip)]
        public async Task Calculate002()
        {
            var Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);
            var TenYearsAgo = DateTimeOffset.Now.AddYears(-10);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(TenYearsAgo, default))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, TenYearsAgo, default);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, TenYearsAgo, default);

                Assert.True(After.Amount == rate * Before.Amount);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact(Skip = skip)]
        public async Task ConversionDirection001()
        {
            var Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            var USDollar = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, USDollar, DateTimeOffset.Now, default);

            Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Fact(Skip = skip)]
        public async Task ConvertCurrency001()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default);

                Assert.True(After.Amount > decimal.Zero);
            }
        }

        [Fact(Skip = skip)]
        public async Task ConvertCurrency002()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            CurrencyPair pair = CurrencyPairs.First();

            Money Before = new Money(pair.BaseCurrency, 10m);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now.AddMinutes(1d), default));
        }

        [Fact(Skip = skip)]
        public async Task ConvertCurrency003()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            CurrencyPair pair = new CurrencyPair(
                new CurrencyInfo(new System.Globalization.RegionInfo("AM")),
                new CurrencyInfo(new System.Globalization.RegionInfo("BY")));

            Money Before = new Money(pair.BaseCurrency, 10m);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default));
        }

        [Fact(Skip = skip)]
        public async Task GetCurrencyPairs001()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            Assert.Contains(CurrencyPairs, C => C.ToString() == "AUD/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "AUD/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "CNY/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CNY/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "CZK/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CZK/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CZK/EUR");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "DKK/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "DKK/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "DKK/EUR");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "EUR/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "HKD/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "HKD/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "HUF/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "HUF/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "HUF/EUR");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "INR/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "INR/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "ILS/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "ILS/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "JPY/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "JPY/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "JPY/EUR");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "MYR/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "MYR/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "NZD/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "NZD/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "NOK/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "NOK/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "PLN/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "PLN/EUR");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "PLN/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "RUB/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "RUB/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "SAR/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "SAR/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "SGD/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "SGD/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "ZAR/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "ZAR/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "KRW/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "KRW/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "GBP/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "SEK/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "SEK/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "CHF/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CHF/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CHF/EUR");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "TWD/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "TWD/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "THB/GBP");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "THB/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "TRY/USD");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "USD/GBP");
        }

        [Fact(Skip = skip)]
        public async Task GetCurrencyPairs002()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            var UniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in CurrencyPairs)
            {
                UniquePairs.Add(pair);
            }

            Assert.True(UniquePairs.Count == CurrencyPairs.Count());
        }

        [Fact(Skip = skip)]
        public async Task GetCurrencyPairs003()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now.AddYears(-10), default);

            var UniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in CurrencyPairs)
            {
                UniquePairs.Add(pair);
            }

            Assert.True(UniquePairs.Count == CurrencyPairs.Count());
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate001()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            foreach (CurrencyPair pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default))
            {
                Assert.True(await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default) > decimal.Zero);
            }
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate002()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.GetExchangeRateAsync(CurrencyPairs.First(), DateTimeOffset.Now.AddMinutes(1d), default));
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate003()
        {
            BankOfEngland Bank = new BankOfEngland(_currencyFactory, _regionFactory, _timeProvider);

            CurrencyPair pair = new CurrencyPair(
                new CurrencyInfo(new System.Globalization.RegionInfo("AM")),
                new CurrencyInfo(new System.Globalization.RegionInfo("BY")));

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default));
        }

        [Fact(Skip = skip)]
        public async Task KeepCurrenciesPairsUpdated()
        {
            // In case or failure, check currency pair information from BOE website and set deadline
            // up to 3 month.

            System.DateTimeOffset Deadline = new DateTime(2019, 10, 01);

            if (System.DateTimeOffset.Now > Deadline)
                throw new Exception("Source is out of date. Please update.");
        }
    }
}
