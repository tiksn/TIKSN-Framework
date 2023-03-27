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
    public class SwissNationalBankTests
    {
        private readonly ICurrencyFactory currencyFactory;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ITimeProvider timeProvider;

        public SwissNationalBankTests()
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
        public async Task Calculation001Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var atTheMoment = this.timeProvider.GetCurrentTime();

            foreach (var pair in await bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true))
            {
                var before = new Money(pair.BaseCurrency, 100m);

                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, atTheMoment, default).ConfigureAwait(true);

                var rate = await bank.GetExchangeRateAsync(pair, atTheMoment, default).ConfigureAwait(true);

                Assert.True(after.Amount == before.Amount * rate);
                Assert.Equal(pair.CounterCurrency, after.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var atTheMoment = this.timeProvider.GetCurrentTime();

            foreach (var pair in await bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true))
            {
                var before = new Money(pair.BaseCurrency, 100m);
                var after = await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, atTheMoment, default).ConfigureAwait(true);

                Assert.True(after.Amount > decimal.Zero);
                Assert.Equal(pair.CounterCurrency, after.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var moment = this.timeProvider.GetCurrentTime().AddMinutes(10d);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true))
            {
                var before = new Money(pair.BaseCurrency, 100m);

                _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task ConvertCurrency004Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var moment = this.timeProvider.GetCurrentTime().AddDays(-10d);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true))
            {
                var before = new Money(pair.BaseCurrency, 100m);

                _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.ConvertCurrencyAsync(before, pair.CounterCurrency, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task CounterCurrency003Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var ao = new RegionInfo("AO");
            var bw = new RegionInfo("BW");

            var aoa = new CurrencyInfo(ao);
            var bwp = new CurrencyInfo(bw);

            var before = new Money(aoa, 100m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.ConvertCurrencyAsync(before, bwp, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs001Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var moment = this.timeProvider.GetCurrentTime().AddMinutes(10d);

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs002Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var pairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            var distinctPairs = pairs.Distinct();

            Assert.True(pairs.Count() == distinctPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs003Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var moment = this.timeProvider.GetCurrentTime().AddDays(-10d);

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs004Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var pairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in pairs)
            {
                var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(pairs, p => p == reversed);
            }
        }

        [Fact]
        public async Task GetCurrencyPairs005Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var pairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            Assert.Contains(pairs, p => p.ToString() == "EUR/CHF");
            Assert.Contains(pairs, p => p.ToString() == "USD/CHF");
            Assert.Contains(pairs, p => p.ToString() == "JPY/CHF");
            Assert.Contains(pairs, p => p.ToString() == "GBP/CHF");

            Assert.Contains(pairs, p => p.ToString() == "CHF/EUR");
            Assert.Contains(pairs, p => p.ToString() == "CHF/USD");
            Assert.Contains(pairs, p => p.ToString() == "CHF/JPY");
            Assert.Contains(pairs, p => p.ToString() == "CHF/GBP");

            Assert.Equal(8, pairs.Count());
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var atTheMoment = this.timeProvider.GetCurrentTime();

            foreach (var pair in await bank.GetCurrencyPairsAsync(atTheMoment, default).ConfigureAwait(true))
            {
                var rate = await bank.GetExchangeRateAsync(pair, atTheMoment, default).ConfigureAwait(true);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var moment = this.timeProvider.GetCurrentTime().AddMinutes(10d);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true))
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task GetExchangeRate003Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var ao = new RegionInfo("AO");
            var bw = new RegionInfo("BW");

            var aoa = new CurrencyInfo(ao);
            var bwp = new CurrencyInfo(bw);

            var pair = new CurrencyPair(aoa, bwp);

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var bank = new SwissNationalBank(this.httpClientFactory, this.currencyFactory, this.timeProvider);

            var moment = this.timeProvider.GetCurrentTime().AddDays(-10d);

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true))
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(async () => await bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }
    }
}
