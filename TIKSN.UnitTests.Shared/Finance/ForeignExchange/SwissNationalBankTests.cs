using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    public class SwissNationalBankTests
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public SwissNationalBankTests()
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
        public async Task Calculation001()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var pair in await Bank.GetCurrencyPairsAsync(AtTheMoment, default))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, AtTheMoment, default);

                var rate = await Bank.GetExchangeRateAsync(pair, AtTheMoment, default);

                Assert.True(After.Amount == Before.Amount * rate);
                Assert.Equal<CurrencyInfo>(pair.CounterCurrency, After.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var Pair in await Bank.GetCurrencyPairsAsync(AtTheMoment, default))
            {
                var Before = new Money(Pair.BaseCurrency, 100m);
                var After = await Bank.ConvertCurrencyAsync(Before, Pair.CounterCurrency, AtTheMoment, default);

                Assert.True(After.Amount > decimal.Zero);
                Assert.Equal<CurrencyInfo>(Pair.CounterCurrency, After.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, moment, default));
            }
        }

        [Fact]
        public async Task ConvertCurrency004()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, moment, default));
            }
        }

        [Fact]
        public async Task CounterCurrency003()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var AO = new RegionInfo("AO");
            var BW = new RegionInfo("BW");

            var AOA = new CurrencyInfo(AO);
            var BWP = new CurrencyInfo(BW);

            var Before = new Money(AOA, 100m);

            await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, BWP, DateTimeOffset.Now, default));
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetCurrencyPairsAsync(moment, default));
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            var DistinctPairs = Pairs.Distinct();

            Assert.True(Pairs.Count() == DistinctPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs003()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetCurrencyPairsAsync(moment, default));
        }

        [Fact]
        public async Task GetCurrencyPairs004()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            foreach (var pair in Pairs)
            {
                var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.True(Pairs.Any(P => P == reversed));
            }
        }

        [Fact]
        public async Task GetCurrencyPairs005()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

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
        public async Task GetExchangeRate001()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var Pair in await Bank.GetCurrencyPairsAsync(AtTheMoment, default))
            {
                decimal rate = await Bank.GetExchangeRateAsync(Pair, AtTheMoment, default);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default))
            {
                await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, moment, default));
            }
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var AO = new RegionInfo("AO");
            var BW = new RegionInfo("BW");

            var AOA = new CurrencyInfo(AO);
            var BWP = new CurrencyInfo(BW);

            var Pair = new CurrencyPair(AOA, BWP);

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(Pair, DateTimeOffset.Now, default));
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            var Bank = new SwissNationalBank(_currencyFactory, _timeProvider);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now, default))
            {
                await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, moment, default));
            }
        }
    }
}