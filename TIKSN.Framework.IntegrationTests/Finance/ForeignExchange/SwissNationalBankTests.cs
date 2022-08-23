using System;
using System.Globalization;
using System.Linq;
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
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public SwissNationalBankTests()
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
        public async Task Calculation001Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var pair in await Bank.GetCurrencyPairsAsync(AtTheMoment, default).ConfigureAwait(true))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, AtTheMoment, default).ConfigureAwait(true);

                var rate = await Bank.GetExchangeRateAsync(pair, AtTheMoment, default).ConfigureAwait(true);

                Assert.True(After.Amount == Before.Amount * rate);
                Assert.Equal(pair.CounterCurrency, After.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var Pair in await Bank.GetCurrencyPairsAsync(AtTheMoment, default).ConfigureAwait(true))
            {
                var Before = new Money(Pair.BaseCurrency, 100m);
                var After = await Bank.ConvertCurrencyAsync(Before, Pair.CounterCurrency, AtTheMoment, default).ConfigureAwait(true);

                Assert.True(After.Amount > decimal.Zero);
                Assert.Equal(Pair.CounterCurrency, After.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task ConvertCurrency004Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task CounterCurrency003Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var AO = new RegionInfo("AO");
            var BW = new RegionInfo("BW");

            var AOA = new CurrencyInfo(AO);
            var BWP = new CurrencyInfo(BW);

            var Before = new Money(AOA, 100m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, BWP, DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs001Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs002Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            var DistinctPairs = Pairs.Distinct();

            Assert.True(Pairs.Count() == DistinctPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs003Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetCurrencyPairsAsync(moment, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs004Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            foreach (var pair in Pairs)
            {
                var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(Pairs, P => P == reversed);
            }
        }

        [Fact]
        public async Task GetCurrencyPairs005Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true);

            Assert.Contains(Pairs, P => P.ToString() == "EUR/CHF");
            Assert.Contains(Pairs, P => P.ToString() == "USD/CHF");
            Assert.Contains(Pairs, P => P.ToString() == "JPY/CHF");
            Assert.Contains(Pairs, P => P.ToString() == "GBP/CHF");

            Assert.Contains(Pairs, P => P.ToString() == "CHF/EUR");
            Assert.Contains(Pairs, P => P.ToString() == "CHF/USD");
            Assert.Contains(Pairs, P => P.ToString() == "CHF/JPY");
            Assert.Contains(Pairs, P => P.ToString() == "CHF/GBP");

            Assert.Equal(8, Pairs.Count());
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var Pair in await Bank.GetCurrencyPairsAsync(AtTheMoment, default).ConfigureAwait(true))
            {
                var rate = await Bank.GetExchangeRateAsync(Pair, AtTheMoment, default).ConfigureAwait(true);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true))
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task GetExchangeRate003Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var AO = new RegionInfo("AO");
            var BW = new RegionInfo("BW");

            var AOA = new CurrencyInfo(AO);
            var BWP = new CurrencyInfo(BW);

            var Pair = new CurrencyPair(AOA, BWP);

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(Pair, DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var Bank = new SwissNationalBank(this._currencyFactory, this._timeProvider);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true))
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, moment, default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }
    }
}
