using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    public class BankOfCanadaTests
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public BankOfCanadaTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            services.AddSingleton<IRegionFactory, RegionFactory>();
            services.AddSingleton<ITimeProvider, TimeProvider>();

            var serviceProvider = services.BuildServiceProvider();
            _currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
            _timeProvider = serviceProvider.GetRequiredService<ITimeProvider>();
        }

        [Fact]
        public async Task Calculate001()
        {
            var bank = new BankOfCanada(_currencyFactory, _timeProvider);

            foreach (var pair in await bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default))
            {
                Money Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);
                Money After = await bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default);

                Assert.True(After.Amount == rate * Before.Amount);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConversionDirection001()
        {
            var Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            var CanadianDollar = new CurrencyInfo(new RegionInfo("CA"));
            var PoundSterling = new CurrencyInfo(new RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, CanadianDollar, DateTimeOffset.Now, default);

            Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                Money Before = new Money(pair.BaseCurrency, decimal.One);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default);

                Assert.True(After.Amount > decimal.Zero);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                Money Before = new Money(pair.BaseCurrency, decimal.One);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default);

                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency003()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);

                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default);

                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

                Assert.True(After.Currency == pair.CounterCurrency);
                Assert.True(After.Amount == rate * Before.Amount);
            }
        }

        [Fact]
        public async Task ConvertCurrency004()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            RegionInfo US = new RegionInfo("US");
            RegionInfo CA = new RegionInfo("CA");

            CurrencyInfo USD = new CurrencyInfo(US);
            CurrencyInfo CAD = new CurrencyInfo(CA);

            Money Before = new Money(USD, 100m);

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.ConvertCurrencyAsync(Before, CAD, DateTimeOffset.Now.AddMinutes(1d), default));
        }

        [Fact]
        public async Task ConvertCurrency006()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            RegionInfo AO = new RegionInfo("AO");
            RegionInfo BW = new RegionInfo("BW");

            CurrencyInfo AOA = new CurrencyInfo(AO);
            CurrencyInfo BWP = new CurrencyInfo(BW);

            Money Before = new Money(AOA, 100m);

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.ConvertCurrencyAsync(Before, BWP, DateTimeOffset.Now, default));
        }

        [Fact]
        public async Task CurrencyPairs001()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/USD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/AUD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/BRL");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/CNY");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/EUR");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/HKD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/INR");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/IDR");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/JPY");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/MXN");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/NZD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/NOK");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/PEN");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/RUB");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/SGD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/ZAR");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/KRW");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/SEK");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/CHF");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/TWD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/TRY");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CAD/GBP");

            Assert.Contains(CurrencyPairs, C => C.ToString() == "USD/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "AUD/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "BRL/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CNY/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "EUR/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "HKD/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "INR/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "IDR/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "JPY/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "MXN/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "NZD/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "NOK/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "PEN/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "RUB/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "SGD/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "ZAR/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "KRW/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "SEK/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "CHF/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "TWD/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "TRY/CAD");
            Assert.Contains(CurrencyPairs, C => C.ToString() == "GBP/CAD");
        }

        [Fact]
        public async Task CurrencyPairs002()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.True(CurrencyPairs.Any(C => C == reversePair));
            }
        }

        [Fact]
        public async Task CurrencyPairs003()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            var pairSet = new HashSet<CurrencyPair>();

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                pairSet.Add(pair);
            }

            Assert.True(pairSet.Count == CurrencyPairs.Count());
        }

        [Fact]
        public async Task CurrencyPairs005()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now.AddDays(10), default));
        }

        [Fact]
        public async Task Fetch001()
        {
            var Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            await Bank.GetExchangeRatesAsync(DateTimeOffset.Now, default);
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            RegionInfo US = new RegionInfo("US");
            RegionInfo CA = new RegionInfo("CA");

            CurrencyInfo USD = new CurrencyInfo(US);
            CurrencyInfo CAD = new CurrencyInfo(CA);

            CurrencyPair pair = new CurrencyPair(CAD, USD);

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now.AddMinutes(1d), default));
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            BankOfCanada Bank = new BankOfCanada(_currencyFactory, _timeProvider);

            RegionInfo AO = new RegionInfo("AO");
            RegionInfo BW = new RegionInfo("BW");

            CurrencyInfo AOA = new CurrencyInfo(AO);
            CurrencyInfo BWP = new CurrencyInfo(BW);

            CurrencyPair pair = new CurrencyPair(BWP, AOA);

            await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default));
        }
    }
}
