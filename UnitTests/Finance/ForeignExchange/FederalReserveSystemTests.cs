using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    public class FederalReserveSystemTests
    {
        private readonly ICurrencyFactory _currencyFactory;

        public FederalReserveSystemTests()
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
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            foreach (var pair in pairs)
            {
                var Before = new Money(pair.BaseCurrency);
                var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now);

                Assert.True(After.Amount == rate * Before.Amount);
            }
        }

        [Fact]
        public async Task ConversionDirection001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var USDollar = new CurrencyInfo(new System.Globalization.RegionInfo("US"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, USDollar, DateTime.Now);

            Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now);

                Assert.True(After.Amount > decimal.Zero);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var LastYear = DateTime.Now.AddYears(-1);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(LastYear))
            {
                var Before = new Money(pair.BaseCurrency, 10m);
                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, LastYear);

                Assert.True(After.Amount > decimal.Zero);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency003()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now))
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddMinutes(1d)));
            }
        }

        [Fact]
        public async Task ConvertCurrency004()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var Before = new Money(new CurrencyInfo(new System.Globalization.RegionInfo("AL")), 10m);

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, new CurrencyInfo(new System.Globalization.RegionInfo("AM")), DateTime.Now.AddMinutes(1d)));
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            foreach (var pair in pairs)
            {
                var reversed = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.True(pairs.Any(C => C == reversed));
            }
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

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
            var Bank = new FederalReserveSystem(_currencyFactory);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetCurrencyPairsAsync(DateTime.Now.AddMinutes(1d)));
        }

        [Fact]
        public async Task GetCurrencyPairs004()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            Assert.True(pairs.Any(C => C.ToString() == "AUD/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "BRL/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "CAD/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "CNY/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "DKK/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "EUR/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "HKD/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "INR/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "JPY/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "MYR/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "MXN/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "NZD/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "NOK/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "SGD/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "ZAR/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "KRW/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "LKR/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "SEK/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "CHF/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "TWD/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "THB/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "GBP/USD"));
            Assert.True(pairs.Any(C => C.ToString() == "VEF/USD"));

            Assert.True(pairs.Any(C => C.ToString() == "USD/AUD"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/BRL"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/CAD"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/CNY"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/DKK"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/EUR"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/HKD"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/INR"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/JPY"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/MYR"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/MXN"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/NZD"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/NOK"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/SGD"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/ZAR"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/KRW"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/LKR"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/SEK"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/CHF"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/TWD"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/THB"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/GBP"));
            Assert.True(pairs.Any(C => C.ToString() == "USD/VEF"));

            Assert.Equal(23 * 2, pairs.Count());
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now))
            {
                var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var LastYear = DateTime.Now.AddYears(-1);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(LastYear))
            {
                var rate = await Bank.GetExchangeRateAsync(pair, LastYear);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now))
            {
                await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(1d)));
            }
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("AL")), new CurrencyInfo(new System.Globalization.RegionInfo("AM")));

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(1d)));
        }

        [Fact]
        public async Task GetExchangeRate005()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("CN")));

            var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);

            Assert.True(rate > decimal.One);
        }

        [Fact]
        public async Task GetExchangeRate006()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("SG")));

            var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);

            Assert.True(rate > decimal.One);
        }

        [Fact]
        public async Task GetExchangeRate007()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("DE")));

            var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);

            Assert.True(rate < decimal.One);
        }

        [Fact]
        public async Task GetExchangeRate008()
        {
            var Bank = new FederalReserveSystem(_currencyFactory);

            var pair = new CurrencyPair(new CurrencyInfo(new System.Globalization.RegionInfo("US")), new CurrencyInfo(new System.Globalization.RegionInfo("GB")));

            var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);

            Assert.True(rate < decimal.One);
        }
    }
}