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
            _ = services.AddMemoryCache();
            _ = services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            _ = services.AddSingleton<IRegionFactory, RegionFactory>();
            _ = services.AddSingleton<ITimeProvider, TimeProvider>();

            var serviceProvider = services.BuildServiceProvider();
            this._currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
            this._timeProvider = serviceProvider.GetRequiredService<ITimeProvider>();
        }

        [Fact]
        public async Task Calculate001Async()
        {
            var bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            foreach (var pair in await bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                var rate = await bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true);
                var After = await bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default).ConfigureAwait(true);

                Assert.True(After.Amount == rate * Before.Amount);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConversionDirection001Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var CanadianDollar = new CurrencyInfo(new RegionInfo("CA"));
            var PoundSterling = new CurrencyInfo(new RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, CanadianDollar, DateTimeOffset.Now, default).ConfigureAwait(true);

            Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, decimal.One);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default).ConfigureAwait(true);

                Assert.True(After.Amount > decimal.Zero);
            }
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, decimal.One);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default).ConfigureAwait(true);

                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency003Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTimeOffset.Now, default).ConfigureAwait(true);

                var rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true);

                Assert.True(After.Currency == pair.CounterCurrency);
                Assert.True(After.Amount == rate * Before.Amount);
            }
        }

        [Fact]
        public async Task ConvertCurrency004Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var US = new RegionInfo("US");
            var CA = new RegionInfo("CA");

            var USD = new CurrencyInfo(US);
            var CAD = new CurrencyInfo(CA);

            var Before = new Money(USD, 100m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.ConvertCurrencyAsync(Before, CAD, DateTimeOffset.Now.AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ConvertCurrency006Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var AO = new RegionInfo("AO");
            var BW = new RegionInfo("BW");

            var AOA = new CurrencyInfo(AO);
            var BWP = new CurrencyInfo(BW);

            var Before = new Money(AOA, 100m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.ConvertCurrencyAsync(Before, BWP, DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task CurrencyPairs001Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

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
        public async Task CurrencyPairs002Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(CurrencyPairs, C => C == reversePair);
            }
        }

        [Fact]
        public async Task CurrencyPairs003Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var pairSet = new HashSet<CurrencyPair>();

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                _ = pairSet.Add(pair);
            }

            Assert.True(pairSet.Count == CurrencyPairs.Count());
        }

        [Fact]
        public async Task CurrencyPairs005Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now.AddDays(10), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task Fetch001Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            _ = await Bank.GetExchangeRatesAsync(DateTimeOffset.Now, default).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var rate = await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var US = new RegionInfo("US");
            var CA = new RegionInfo("CA");

            var USD = new CurrencyInfo(US);
            var CAD = new CurrencyInfo(CA);

            var pair = new CurrencyPair(CAD, USD);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now.AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var Bank = new BankOfCanada(this._currencyFactory, this._timeProvider);

            var AO = new RegionInfo("AO");
            var BW = new RegionInfo("BW");

            var AOA = new CurrencyInfo(AO);
            var BWP = new CurrencyInfo(BW);

            var pair = new CurrencyPair(BWP, AOA);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await Bank.GetExchangeRateAsync(pair, DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }
}
