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
    public class ReserveBankOfAustraliaTests
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ICurrencyFactory currencyFactory;
        private readonly ITimeProvider timeProvider;

        public ReserveBankOfAustraliaTests()
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
        public async Task ConversionDirection001Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var australianDollar = new CurrencyInfo(new RegionInfo("AU"));
            var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

            var beforeInPound = new Money(poundSterling, 100m);

            var afterInDollar = await bank.ConvertCurrencyAsync(beforeInPound, australianDollar, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            Assert.True(beforeInPound.Amount < afterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var before = new Money(pair.BaseCurrency, 10m);

                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                Assert.True(after.Amount > decimal.Zero);
                Assert.True(after.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var before = new Money(pair.BaseCurrency, 10m);

                var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                Assert.True(after.Amount == before.Amount * rate);
            }
        }

        [Fact]
        public async Task ConvertCurrency003Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var before = new Money(pair.BaseCurrency, 10m);

                _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime().AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task ConvertCurrency004Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var before = new Money(pair.BaseCurrency, 10m);

                _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime().AddDays(-20d), default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task ConvertCurrency005Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var armenia = new RegionInfo("AM");
            var belarus = new RegionInfo("BY");

            var armenianDram = new CurrencyInfo(armenia);
            var belarusianRuble = new CurrencyInfo(belarus);

            var before = new Money(armenianDram, 10m);

            _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await bank.ConvertCurrencyAsync(before, belarusianRuble, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task Fetch001Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            _ = await bank.GetExchangeRatesAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs001Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            Assert.Contains(currencyPairs, p => p.ToString() == "USD/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "CNY/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "JPY/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "EUR/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "KRW/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "GBP/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "SGD/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "INR/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "THB/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "NZD/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "TWD/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "MYR/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "IDR/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "VND/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "HKD/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "CHF/AUD");
            Assert.Contains(currencyPairs, p => p.ToString() == "XDR/AUD");

            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/USD");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/CNY");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/JPY");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/EUR");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/KRW");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/GBP");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/SGD");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/INR");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/THB");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/NZD");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/TWD");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/MYR");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/IDR");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/VND");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/HKD");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/CHF");
            Assert.Contains(currencyPairs, p => p.ToString() == "AUD/XDR");
        }

        [Fact]
        public async Task GetCurrencyPairs002Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(currencyPairs, p => p == reversedPair);
            }
        }

        [Fact]
        public async Task GetCurrencyPairs003Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var pairSet = new HashSet<CurrencyPair>();

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                _ = pairSet.Add(pair);
            }

            Assert.True(pairSet.Count == currencyPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs004Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime().AddMinutes(10), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs005Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime().AddDays(-20), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                Assert.True(await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true) > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime().AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task GetExchangeRate003Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime().AddDays(-20d), default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var bank = new ReserveBankOfAustralia(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var armenia = new RegionInfo("AM");
            var belarus = new RegionInfo("BY");

            var armenianDram = new CurrencyInfo(armenia);
            var belarusianRuble = new CurrencyInfo(belarus);

            var pair = new CurrencyPair(armenianDram, belarusianRuble);

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }
}
