using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    public class ReserveBankOfAustraliaTests
    {
        private readonly ICurrencyFactory _currencyFactory;

        public ReserveBankOfAustraliaTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            services.AddSingleton<IRegionFactory, RegionFactory>();

            var serviceProvider = services.BuildServiceProvider();
            _currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        }

        [Fact]
        public async Task ConversionDirection001()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var AustralianDollar = new CurrencyInfo(new System.Globalization.RegionInfo("AU"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, AustralianDollar, DateTime.Now, default);

            Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default);

                Assert.True(After.Amount > decimal.Zero);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default);

                Assert.True(After.Amount == Before.Amount * rate);
            }
        }

        [Fact]
        public async Task ConvertCurrency003()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddMinutes(1d), default));
            }
        }

        [Fact]
        public async Task ConvertCurrency004()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddDays(-20d), default));
            }
        }

        [Fact]
        public async Task ConvertCurrency005()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var Armenia = new System.Globalization.RegionInfo("AM");
            var Belarus = new System.Globalization.RegionInfo("BY");

            var ArmenianDram = new CurrencyInfo(Armenia);
            var BelarusianRuble = new CurrencyInfo(Belarus);

            var Before = new Money(ArmenianDram, 10m);

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, BelarusianRuble, DateTime.Now, default));
        }

        [Fact]
        public async Task Fetch001()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            await Bank.GetExchangeRatesAsync(DateTimeOffset.Now, default);
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            Assert.Contains(CurrencyPairs, P => P.ToString() == "USD/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "CNY/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "JPY/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "EUR/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "KRW/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "GBP/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "SGD/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "INR/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "THB/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "NZD/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "TWD/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "MYR/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "IDR/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "VND/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AED/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "HKD/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "CAD/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "CHF/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "PGK/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "XDR/AUD");

            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/USD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/CNY");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/JPY");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/EUR");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/KRW");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/GBP");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/SGD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/INR");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/THB");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/NZD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/TWD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/MYR");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/IDR");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/VND");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/AED");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/HKD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/CAD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/CHF");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/PGK");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/XDR");
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                var reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(CurrencyPairs, P => P == reversedPair);
            }
        }

        [Fact]
        public async Task GetCurrencyPairs003()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var PairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                PairSet.Add(pair);
            }

            Assert.True(PairSet.Count == CurrencyPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs004()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetCurrencyPairsAsync(DateTime.Now.AddMinutes(10), default));
        }

        [Fact]
        public async Task GetCurrencyPairs005()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetCurrencyPairsAsync(DateTime.Now.AddDays(-20), default));
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                Assert.True(await Bank.GetExchangeRateAsync(pair, DateTime.Now, default) > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(1d), default));
            }
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddDays(-20d), default));
            }
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            var Bank = new ReserveBankOfAustralia(_currencyFactory);

            var Armenia = new System.Globalization.RegionInfo("AM");
            var Belarus = new System.Globalization.RegionInfo("BY");

            var ArmenianDram = new CurrencyInfo(Armenia);
            var BelarusianRuble = new CurrencyInfo(Belarus);

            var pair = new CurrencyPair(ArmenianDram, BelarusianRuble);

            await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, DateTime.Now, default));
        }
    }
}