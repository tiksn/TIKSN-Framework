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
    public class ReserveBankOfAustraliaTests
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public ReserveBankOfAustraliaTests()
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
        public async Task ConversionDirection001Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var AustralianDollar = new CurrencyInfo(new System.Globalization.RegionInfo("AU"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDollar = await Bank.ConvertCurrencyAsync(BeforeInPound, AustralianDollar, DateTime.Now, default).ConfigureAwait(true);

            Assert.True(BeforeInPound.Amount < AfterInDollar.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default).ConfigureAwait(true);

                Assert.True(After.Amount > decimal.Zero);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default).ConfigureAwait(true);

                var After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default).ConfigureAwait(true);

                Assert.True(After.Amount == Before.Amount * rate);
            }
        }

        [Fact]
        public async Task ConvertCurrency003Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task ConvertCurrency004Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var Before = new Money(pair.BaseCurrency, 10m);

                _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddDays(-20d), default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task ConvertCurrency005Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var Armenia = new System.Globalization.RegionInfo("AM");
            var Belarus = new System.Globalization.RegionInfo("BY");

            var ArmenianDram = new CurrencyInfo(Armenia);
            var BelarusianRuble = new CurrencyInfo(Belarus);

            var Before = new Money(ArmenianDram, 10m);

            _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, BelarusianRuble, DateTime.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task Fetch001Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            _ = await Bank.GetExchangeRatesAsync(DateTimeOffset.Now, default).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs001Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

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
            Assert.Contains(CurrencyPairs, P => P.ToString() == "HKD/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "CAD/AUD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "CHF/AUD");
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
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/HKD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/CAD");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/CHF");
            Assert.Contains(CurrencyPairs, P => P.ToString() == "AUD/XDR");
        }

        [Fact]
        public async Task GetCurrencyPairs002Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var reversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(CurrencyPairs, P => P == reversedPair);
            }
        }

        [Fact]
        public async Task GetCurrencyPairs003Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var PairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                _ = PairSet.Add(pair);
            }

            Assert.True(PairSet.Count == CurrencyPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs004Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetCurrencyPairsAsync(DateTime.Now.AddMinutes(10), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetCurrencyPairs005Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetCurrencyPairsAsync(DateTime.Now.AddDays(-20), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                Assert.True(await Bank.GetExchangeRateAsync(pair, DateTime.Now, default).ConfigureAwait(true) > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task GetExchangeRate003Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                _ = await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddDays(-20d), default).ConfigureAwait(true)).ConfigureAwait(true);
            }
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var Bank = new ReserveBankOfAustralia(this._currencyFactory, this._timeProvider);

            var Armenia = new System.Globalization.RegionInfo("AM");
            var Belarus = new System.Globalization.RegionInfo("BY");

            var ArmenianDram = new CurrencyInfo(Armenia);
            var BelarusianRuble = new CurrencyInfo(Belarus);

            var pair = new CurrencyPair(ArmenianDram, BelarusianRuble);

            _ = await Assert.ThrowsAsync<ArgumentException>(async () => await Bank.GetExchangeRateAsync(pair, DateTime.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }
}
