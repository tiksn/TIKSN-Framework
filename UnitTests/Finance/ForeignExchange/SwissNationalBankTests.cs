using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    public class SwissNationalBankTests
    {
        private readonly ICurrencyFactory _currencyFactory;

        public SwissNationalBankTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            services.AddSingleton<IRegionFactory, RegionFactory>();

            var serviceProvider = services.BuildServiceProvider();
            _currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        }

        [Fact]
        public async Task Calculation001()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var pair in await Bank.GetCurrencyPairsAsync(AtTheMoment))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, AtTheMoment);

                var rate = await Bank.GetExchangeRateAsync(pair, AtTheMoment);

                Assert.True(After.Amount == Before.Amount * rate);
                Assert.Equal<CurrencyInfo>(pair.CounterCurrency, After.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var Pair in await Bank.GetCurrencyPairsAsync(AtTheMoment))
            {
                var Before = new Money(Pair.BaseCurrency, 100m);
                var After = await Bank.ConvertCurrencyAsync(Before, Pair.CounterCurrency, AtTheMoment);

                Assert.True(After.Amount > decimal.Zero);
                Assert.Equal<CurrencyInfo>(Pair.CounterCurrency, After.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, moment));
            }
        }

        [Fact]
        public async Task ConvertCurrency004()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
            {
                var Before = new Money(pair.BaseCurrency, 100m);

                await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, moment));
            }
        }

        [Fact]
        public async Task CounterCurrency003()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            RegionInfo AO = new RegionInfo("AO");
            RegionInfo BW = new RegionInfo("BW");

            CurrencyInfo AOA = new CurrencyInfo(AO);
            CurrencyInfo BWP = new CurrencyInfo(BW);

            var Before = new Money(AOA, 100m);

            await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, BWP, DateTimeOffset.Now));
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetCurrencyPairsAsync(moment));
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

            var DistinctPairs = Pairs.Distinct();

            Assert.True(Pairs.Count() == DistinctPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs003()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetCurrencyPairsAsync(moment));
        }

        [Fact]
        public async Task GetCurrencyPairs004()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

            foreach (var pair in Pairs)
            {
                var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.True(Pairs.Any(P => P == reversed));
            }
        }

        [Fact]
        public async Task GetCurrencyPairs005()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var Pairs = await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now);

            Assert.True(Pairs.Any(P => P.ToString() == "EUR/CHF"));
            Assert.True(Pairs.Any(P => P.ToString() == "USD/CHF"));
            Assert.True(Pairs.Any(P => P.ToString() == "JPY/CHF"));
            Assert.True(Pairs.Any(P => P.ToString() == "GBP/CHF"));

            Assert.True(Pairs.Any(P => P.ToString() == "CHF/EUR"));
            Assert.True(Pairs.Any(P => P.ToString() == "CHF/USD"));
            Assert.True(Pairs.Any(P => P.ToString() == "CHF/JPY"));
            Assert.True(Pairs.Any(P => P.ToString() == "CHF/GBP"));

            Assert.Equal(8, Pairs.Count());
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var AtTheMoment = DateTimeOffset.Now;

            foreach (var Pair in await Bank.GetCurrencyPairsAsync(AtTheMoment))
            {
                decimal rate = await Bank.GetExchangeRateAsync(Pair, AtTheMoment);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var moment = DateTimeOffset.Now.AddMinutes(10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
            {
                await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, moment));
            }
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            RegionInfo AO = new RegionInfo("AO");
            RegionInfo BW = new RegionInfo("BW");

            CurrencyInfo AOA = new CurrencyInfo(AO);
            CurrencyInfo BWP = new CurrencyInfo(BW);

            CurrencyPair Pair = new CurrencyPair(AOA, BWP);

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(Pair, DateTimeOffset.Now));
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            var Bank = new SwissNationalBank(_currencyFactory);

            var moment = DateTimeOffset.Now.AddDays(-10d);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTimeOffset.Now))
            {
                await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, moment));
            }
        }
    }
}