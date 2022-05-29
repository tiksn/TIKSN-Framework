using System;
using System.Linq;
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
        const string skip = "API changed, code needs to be adopted";

        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public FederalReserveSystemTests()
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

        [Fact(Skip = skip)]
        public async Task Calculation001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (var pair in pairs)
            {
                var Before = new Money(pair.BaseCurrency);
                var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default);

                Assert.True(After.Amount == rate * Before.Amount);
            }
        }

        [Fact(Skip = skip)]
        public async Task ConversionDirection001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var USDollar = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, USDollar, DateTime.Now, default);

            Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Fact(Skip = skip)]
        public async Task ConvertCurrency001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now, default))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default);

                Assert.True(After.Amount > decimal.Zero);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact(Skip = skip)]
        public async Task ConvertCurrency002()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var LastYear = DateTime.Now.AddYears(-1);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(LastYear, default))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, LastYear, default);

                Assert.True(After.Amount > decimal.Zero);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact(Skip = skip)]
        public async Task ConvertCurrency003()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now, default))
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddMinutes(1d), default));
            }
        }

        [Fact(Skip = skip)]
        public async Task ConvertCurrency004()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var Before = new Money(new CurrencyInfo(new System.Globalization.RegionInfo("AL")), 10m);

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, new CurrencyInfo(new System.Globalization.RegionInfo("AM")), DateTime.Now.AddMinutes(1d), default));
        }

        [Fact(Skip = skip)]
        public async Task GetCurrencyPairs001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (var pair in pairs)
            {
                var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(pairs, C => C == reversed);
            }
        }

        [Fact(Skip = skip)]
        public async Task GetCurrencyPairs002()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in pairs)
            {
                Assert.True(uniquePairs.Add(pair));
            }

            Assert.True(uniquePairs.Count == pairs.Count());
        }

        [Fact(Skip = skip)]
        public async Task GetCurrencyPairs003()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetCurrencyPairsAsync(DateTime.Now.AddMinutes(1d), default));
        }

        [Fact(Skip = skip)]
        public async Task GetCurrencyPairs004()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            Assert.Contains(pairs, C => C.ToString() == "AUD/USD");
            Assert.Contains(pairs, C => C.ToString() == "BRL/USD");
            Assert.Contains(pairs, C => C.ToString() == "CAD/USD");
            Assert.Contains(pairs, C => C.ToString() == "CNY/USD");
            Assert.Contains(pairs, C => C.ToString() == "DKK/USD");
            Assert.Contains(pairs, C => C.ToString() == "EUR/USD");
            Assert.Contains(pairs, C => C.ToString() == "HKD/USD");
            Assert.Contains(pairs, C => C.ToString() == "INR/USD");
            Assert.Contains(pairs, C => C.ToString() == "JPY/USD");
            Assert.Contains(pairs, C => C.ToString() == "MYR/USD");
            Assert.Contains(pairs, C => C.ToString() == "MXN/USD");
            Assert.Contains(pairs, C => C.ToString() == "NZD/USD");
            Assert.Contains(pairs, C => C.ToString() == "NOK/USD");
            Assert.Contains(pairs, C => C.ToString() == "SGD/USD");
            Assert.Contains(pairs, C => C.ToString() == "ZAR/USD");
            Assert.Contains(pairs, C => C.ToString() == "KRW/USD");
            Assert.Contains(pairs, C => C.ToString() == "LKR/USD");
            Assert.Contains(pairs, C => C.ToString() == "SEK/USD");
            Assert.Contains(pairs, C => C.ToString() == "CHF/USD");
            Assert.Contains(pairs, C => C.ToString() == "TWD/USD");
            Assert.Contains(pairs, C => C.ToString() == "THB/USD");
            Assert.Contains(pairs, C => C.ToString() == "GBP/USD");

            Assert.Contains(pairs, C => C.ToString() == "USD/AUD");
            Assert.Contains(pairs, C => C.ToString() == "USD/BRL");
            Assert.Contains(pairs, C => C.ToString() == "USD/CAD");
            Assert.Contains(pairs, C => C.ToString() == "USD/CNY");
            Assert.Contains(pairs, C => C.ToString() == "USD/DKK");
            Assert.Contains(pairs, C => C.ToString() == "USD/EUR");
            Assert.Contains(pairs, C => C.ToString() == "USD/HKD");
            Assert.Contains(pairs, C => C.ToString() == "USD/INR");
            Assert.Contains(pairs, C => C.ToString() == "USD/JPY");
            Assert.Contains(pairs, C => C.ToString() == "USD/MYR");
            Assert.Contains(pairs, C => C.ToString() == "USD/MXN");
            Assert.Contains(pairs, C => C.ToString() == "USD/NZD");
            Assert.Contains(pairs, C => C.ToString() == "USD/NOK");
            Assert.Contains(pairs, C => C.ToString() == "USD/SGD");
            Assert.Contains(pairs, C => C.ToString() == "USD/ZAR");
            Assert.Contains(pairs, C => C.ToString() == "USD/KRW");
            Assert.Contains(pairs, C => C.ToString() == "USD/LKR");
            Assert.Contains(pairs, C => C.ToString() == "USD/SEK");
            Assert.Contains(pairs, C => C.ToString() == "USD/CHF");
            Assert.Contains(pairs, C => C.ToString() == "USD/TWD");
            Assert.Contains(pairs, C => C.ToString() == "USD/THB");
            Assert.Contains(pairs, C => C.ToString() == "USD/GBP");
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now, default))
            {
                var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate002()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var LastYear = DateTime.Now.AddYears(-1);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(LastYear, default))
            {
                var rate = await Bank.GetExchangeRateAsync(pair, LastYear, default);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate003()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now, default))
            {
                await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(1d), default));
            }
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate004()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("AL")), new CurrencyInfo(new System.Globalization.RegionInfo("AM")));

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(1d), default));
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate005()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("CN")));

            var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

            Assert.True(rate > decimal.One);
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate006()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("SG")));

            var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

            Assert.True(rate > decimal.One);
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate007()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("DE")));

            var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

            Assert.True(rate < decimal.One);
        }

        [Fact(Skip = skip)]
        public async Task GetExchangeRate008()
        {
            var Bank = new FederalReserveSystem(_currencyFactory, _timeProvider);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("GB")));

            var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

            Assert.True(rate < decimal.One);
        }
    }
}
