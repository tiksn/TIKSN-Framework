using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TIKSN.Finance.Tests
{
    public class FixedRateCurrencyConverterTests
    {
        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);

            var Initial = new Money(USDollar, 100);

            var converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

            var Final = await converter.ConvertCurrencyAsync(Initial, PoundSterling, DateTime.Now, default).ConfigureAwait(true);

            Assert.Equal(PoundSterling, Final.Currency);
            Assert.Equal(200m, Final.Amount);
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");
            var Italy = new RegionInfo("IT");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);
            var Euro = new CurrencyInfo(Italy);

            var Initial = new Money(USDollar, 100);

            var converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () => await converter.ConvertCurrencyAsync(Initial, Euro, DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ConvertCurrency003Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");
            var Italy = new RegionInfo("IT");
            var Armenia = new RegionInfo("AM");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);
            var Euro = new CurrencyInfo(Italy);
            var ArmenianDram = new CurrencyInfo(Armenia);

            var Initial = new Money(ArmenianDram, 100);

            var converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () => await converter.ConvertCurrencyAsync(Initial, Euro, DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public Task CurrencyPair001Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");
            var Italy = new RegionInfo("IT");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);

            _ = new CurrencyInfo(Italy);

            var converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

            Assert.True(ReferenceEquals(converter.CurrencyPair.BaseCurrency, USDollar));
            Assert.True(ReferenceEquals(converter.CurrencyPair.CounterCurrency, PoundSterling));
            return Task.CompletedTask;
        }

        [Fact]
        public Task FixedRateCurrencyConverter001Async()
        {
            _ = Assert.Throws<ArgumentNullException>(
                    () => new FixedRateCurrencyConverter(null, 0.5m));
            return Task.CompletedTask;
        }

        [Fact]
        public Task FixedRateCurrencyConverter002Async()
        {
            var US = new RegionInfo("US");
            var AM = new RegionInfo("AM");

            var USD = new CurrencyInfo(US);
            var AMD = new CurrencyInfo(AM);

            var pair = new CurrencyPair(USD, AMD);

            _ = Assert.Throws<ArgumentException>(
                    () => new FixedRateCurrencyConverter(pair, -0.5m));
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetCurrencyPairs001Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);

            var pair = new CurrencyPair(USDollar, PoundSterling);

            var converter = new FixedRateCurrencyConverter(pair, 2m);

            Assert.Equal(USDollar, converter.CurrencyPair.BaseCurrency);
            Assert.Equal(PoundSterling, converter.CurrencyPair.CounterCurrency);

            Assert.True(ReferenceEquals(pair, (await converter.GetCurrencyPairsAsync(DateTimeOffset.Now, default).ConfigureAwait(true)).Single()));
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);

            var converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

            Assert.Equal(2m, await converter.GetExchangeRateAsync(new CurrencyPair(USDollar, PoundSterling), DateTimeOffset.Now, default).ConfigureAwait(true));
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);

            var converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () => await converter.GetExchangeRateAsync(new CurrencyPair(PoundSterling, USDollar), DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate003Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");
            var Italy = new RegionInfo("IT");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);
            var Euro = new CurrencyInfo(Italy);

            var converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () => await converter.GetExchangeRateAsync(new CurrencyPair(Euro, USDollar), DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");
            var Italy = new RegionInfo("IT");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);
            var Euro = new CurrencyInfo(Italy);

            var converter = new FixedRateCurrencyConverter(new CurrencyPair(USDollar, PoundSterling), 2m);

            _ = await Assert.ThrowsAsync<ArgumentException>(
                    async () => await converter.GetExchangeRateAsync(new CurrencyPair(USDollar, Euro), DateTimeOffset.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate005Async()
        {
            var UnitedStates = new RegionInfo("US");
            var UnitedKingdom = new RegionInfo("GB");

            var USDollar = new CurrencyInfo(UnitedStates);
            var PoundSterling = new CurrencyInfo(UnitedKingdom);

            var pair = new CurrencyPair(PoundSterling, USDollar);

            var converter = new FixedRateCurrencyConverter(pair, 1.6m);

            var LastMonth = DateTimeOffset.Now.AddMonths(-1);
            var NextMonth = DateTimeOffset.Now.AddMonths(1);

            var RateInLastMonth = await converter.GetExchangeRateAsync(pair, LastMonth, default).ConfigureAwait(true);
            var RateInNextMonth = await converter.GetExchangeRateAsync(pair, NextMonth, default).ConfigureAwait(true);

            Assert.True(RateInLastMonth == RateInNextMonth);
        }
    }
}
