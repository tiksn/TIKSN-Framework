using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.IntegrationTests
{
    public class BankOfCanadaTests
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ICurrencyFactory currencyFactory;
        private readonly ITimeProvider timeProvider;

        public BankOfCanadaTests()
        {
            var services = new ServiceCollection();
            _ = services.AddMemoryCache();
            _ = services.AddHttpClient();
            _ = services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            _ = services.AddSingleton<IRegionFactory, RegionFactory>();
            _ = services.AddSingleton<ITimeProvider, TimeProvider>();

            var serviceProvider = services.BuildServiceProvider();
            this.currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
            this.timeProvider = serviceProvider.GetRequiredService<ITimeProvider>();
            this.httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        }

        [Fact]
        public async Task Calculate001Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true))
            {
                var before = new Money(pair.BaseCurrency, 10m);
                var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);
                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                Assert.True(after.Amount == rate * before.Amount);
                Assert.True(after.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConversionDirection001Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var canadianDollar = new CurrencyInfo(new RegionInfo("CA"));
            var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

            var beforeInPound = new Money(poundSterling, 100m);

            var afterInDollar = await bank.ConvertCurrencyAsync(
                beforeInPound,
                canadianDollar,
                this.timeProvider.GetCurrentTime(),
                default).ConfigureAwait(true);

            Assert.True(beforeInPound.Amount < afterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(
                this.timeProvider.GetCurrentTime(),
                default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var before = new Money(pair.BaseCurrency, decimal.One);

                var after = await bank.ConvertCurrencyAsync(
                    before,
                    pair.CounterCurrency,
                    this.timeProvider.GetCurrentTime(),
                    default).ConfigureAwait(true);

                Assert.True(after.Amount > decimal.Zero);
            }
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(
                this.timeProvider.GetCurrentTime(),
                default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var before = new Money(pair.BaseCurrency, decimal.One);

                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                Assert.True(after.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency003Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(
                this.timeProvider.GetCurrentTime(),
                default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var before = new Money(pair.BaseCurrency, 10m);

                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                Assert.True(after.Currency == pair.CounterCurrency);
                Assert.True(after.Amount == rate * before.Amount);
            }
        }

        [Fact]
        public async Task ConvertCurrency004Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var us = new RegionInfo("US");
            var ca = new RegionInfo("CA");

            var usd = new CurrencyInfo(us);
            var cad = new CurrencyInfo(ca);

            var before = new Money(usd, 100m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await bank.ConvertCurrencyAsync(before, cad, this.timeProvider.GetCurrentTime().AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ConvertCurrency006Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var ao = new RegionInfo("AO");
            var bw = new RegionInfo("BW");

            var aoa = new CurrencyInfo(ao);
            var bwp = new CurrencyInfo(bw);

            var before = new Money(aoa, 100m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task CurrencyPairs001Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(
                this.timeProvider.GetCurrentTime(),
                default).ConfigureAwait(true);

            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/USD");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/AUD");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/BRL");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/CNY");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/EUR");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/HKD");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/INR");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/IDR");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/JPY");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/MXN");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/NZD");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/NOK");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/PEN");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/RUB");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/SGD");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/ZAR");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/KRW");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/SEK");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/CHF");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/TWD");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/TRY");
            Assert.Contains(currencyPairs, c => c.ToString() == "CAD/GBP");

            Assert.Contains(currencyPairs, c => c.ToString() == "USD/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "AUD/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "BRL/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "CNY/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "EUR/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "HKD/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "INR/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "IDR/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "JPY/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "MXN/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "NZD/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "NOK/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "PEN/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "RUB/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "SGD/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "ZAR/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "KRW/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "SEK/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "CHF/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "TWD/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "TRY/CAD");
            Assert.Contains(currencyPairs, c => c.ToString() == "GBP/CAD");
        }

        [Fact]
        public async Task CurrencyPairs002Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(
                this.timeProvider.GetCurrentTime(),
                default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(currencyPairs, c => c == reversePair);
            }
        }

        [Fact]
        public async Task CurrencyPairs003Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pairSet = new HashSet<CurrencyPair>();

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                _ = pairSet.Add(pair);
            }

            Assert.True(pairSet.Count == currencyPairs.Count());
        }

        [Fact]
        public async Task CurrencyPairs005Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime().AddDays(10), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task Fetch001Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            _ = await bank.GetExchangeRatesAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(
                this.timeProvider.GetCurrentTime(),
                default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var us = new RegionInfo("US");
            var ca = new RegionInfo("CA");

            var usd = new CurrencyInfo(us);
            var cad = new CurrencyInfo(ca);

            var pair = new CurrencyPair(cad, usd);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime().AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var bank = new BankOfCanada(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var ao = new RegionInfo("AO");
            var bw = new RegionInfo("BW");

            var aoa = new CurrencyInfo(ao);
            var bwp = new CurrencyInfo(bw);

            var pair = new CurrencyPair(bwp, aoa);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                async () => await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }
}
