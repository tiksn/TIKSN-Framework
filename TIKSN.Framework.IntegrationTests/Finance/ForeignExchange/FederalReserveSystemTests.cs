using System;
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
    public class FederalReserveSystemTests
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ICurrencyFactory currencyFactory;
        private readonly ITimeProvider timeProvider;

        public FederalReserveSystemTests()
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
        public async Task Calculation001()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default);

            foreach (var pair in pairs)
            {
                var before = new Money(pair.BaseCurrency);
                var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default);

                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime(), default);

                Assert.True(after.Amount == rate * before.Amount);
            }
        }

        [Fact]
        public async Task ConversionDirection001()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var usDollar = new CurrencyInfo(new RegionInfo("US"));
            var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

            var beforeInPound = new Money(poundSterling, 100m);

            var afterInDollar = await bank.ConvertCurrencyAsync(beforeInPound, usDollar, this.timeProvider.GetCurrentTime(), default);

            Assert.True(beforeInPound.Amount < afterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default))
            {
                var before = new Money(pair.BaseCurrency, 10m);
                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime(), default);

                Assert.True(after.Amount > decimal.Zero);
                Assert.True(after.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var lastYear = this.timeProvider.GetCurrentTime().AddYears(-1);

            foreach (var pair in await bank.GetCurrencyPairsAsync(lastYear, default))
            {
                var before = new Money(pair.BaseCurrency, 10m);
                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, lastYear, default);

                Assert.True(after.Amount > decimal.Zero);
                Assert.True(after.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency003()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default))
            {
                var before = new Money(pair.BaseCurrency, 10m);

                _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, this.timeProvider.GetCurrentTime().AddMinutes(1d), default));
            }
        }

        [Fact]
        public async Task ConvertCurrency004()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var before = new Money(new CurrencyInfo(new RegionInfo("AL")), 10m);

            _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await bank.ConvertCurrencyAsync(before, new CurrencyInfo(new RegionInfo("AM")), this.timeProvider.GetCurrentTime().AddMinutes(1d), default));
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default);

            foreach (var pair in pairs)
            {
                var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(pairs, c => c == reversed);
            }
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default);

            var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in pairs)
            {
                Assert.True(uniquePairs.Add(pair));
            }

            Assert.True(uniquePairs.Count == pairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs003()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime().AddMinutes(1d), default));
        }

        [Fact]
        public async Task GetCurrencyPairs004()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default);

            Assert.Contains(pairs, c => c.ToString() == "AUD/USD");
            Assert.Contains(pairs, c => c.ToString() == "BRL/USD");
            Assert.Contains(pairs, c => c.ToString() == "CAD/USD");
            Assert.Contains(pairs, c => c.ToString() == "CNY/USD");
            Assert.Contains(pairs, c => c.ToString() == "DKK/USD");
            Assert.Contains(pairs, c => c.ToString() == "EUR/USD");
            Assert.Contains(pairs, c => c.ToString() == "HKD/USD");
            Assert.Contains(pairs, c => c.ToString() == "INR/USD");
            Assert.Contains(pairs, c => c.ToString() == "JPY/USD");
            Assert.Contains(pairs, c => c.ToString() == "MYR/USD");
            Assert.Contains(pairs, c => c.ToString() == "MXN/USD");
            Assert.Contains(pairs, c => c.ToString() == "NZD/USD");
            Assert.Contains(pairs, c => c.ToString() == "NOK/USD");
            Assert.Contains(pairs, c => c.ToString() == "SGD/USD");
            Assert.Contains(pairs, c => c.ToString() == "ZAR/USD");
            Assert.Contains(pairs, c => c.ToString() == "KRW/USD");
            Assert.Contains(pairs, c => c.ToString() == "LKR/USD");
            Assert.Contains(pairs, c => c.ToString() == "SEK/USD");
            Assert.Contains(pairs, c => c.ToString() == "CHF/USD");
            Assert.Contains(pairs, c => c.ToString() == "TWD/USD");
            Assert.Contains(pairs, c => c.ToString() == "THB/USD");
            Assert.Contains(pairs, c => c.ToString() == "GBP/USD");

            Assert.Contains(pairs, c => c.ToString() == "USD/AUD");
            Assert.Contains(pairs, c => c.ToString() == "USD/BRL");
            Assert.Contains(pairs, c => c.ToString() == "USD/CAD");
            Assert.Contains(pairs, c => c.ToString() == "USD/CNY");
            Assert.Contains(pairs, c => c.ToString() == "USD/DKK");
            Assert.Contains(pairs, c => c.ToString() == "USD/EUR");
            Assert.Contains(pairs, c => c.ToString() == "USD/HKD");
            Assert.Contains(pairs, c => c.ToString() == "USD/INR");
            Assert.Contains(pairs, c => c.ToString() == "USD/JPY");
            Assert.Contains(pairs, c => c.ToString() == "USD/MYR");
            Assert.Contains(pairs, c => c.ToString() == "USD/MXN");
            Assert.Contains(pairs, c => c.ToString() == "USD/NZD");
            Assert.Contains(pairs, c => c.ToString() == "USD/NOK");
            Assert.Contains(pairs, c => c.ToString() == "USD/SGD");
            Assert.Contains(pairs, c => c.ToString() == "USD/ZAR");
            Assert.Contains(pairs, c => c.ToString() == "USD/KRW");
            Assert.Contains(pairs, c => c.ToString() == "USD/LKR");
            Assert.Contains(pairs, c => c.ToString() == "USD/SEK");
            Assert.Contains(pairs, c => c.ToString() == "USD/CHF");
            Assert.Contains(pairs, c => c.ToString() == "USD/TWD");
            Assert.Contains(pairs, c => c.ToString() == "USD/THB");
            Assert.Contains(pairs, c => c.ToString() == "USD/GBP");
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default))
            {
                var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var lastYear = this.timeProvider.GetCurrentTime().AddYears(-1);

            foreach (var pair in await bank.GetCurrencyPairsAsync(lastYear, default))
            {
                var rate = await bank.GetExchangeRateAsync(pair, lastYear, default);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default))
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime().AddMinutes(1d), default));
            }
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("AL")), new CurrencyInfo(new RegionInfo("AM")));

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime().AddMinutes(1d), default));
        }

        [Fact]
        public async Task GetExchangeRate005()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("CN")));

            var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default);

            Assert.True(rate > decimal.One);
        }

        [Fact]
        public async Task GetExchangeRate006()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("SG")));

            var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default);

            Assert.True(rate > decimal.One);
        }

        [Fact]
        public async Task GetExchangeRate007()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("DE")));

            var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default);

            Assert.True(rate < decimal.One);
        }

        [Fact]
        public async Task GetExchangeRate008()
        {
            var bank = new FederalReserveSystem(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new RegionInfo("US")), new CurrencyInfo(new RegionInfo("GB")));

            var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default);

            Assert.True(rate < decimal.One);
        }
    }
}
