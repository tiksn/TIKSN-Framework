using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    public class CentralBankOfArmeniaTests
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public CentralBankOfArmeniaTests()
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
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var ArmenianDram = new CurrencyInfo(new RegionInfo("AM"));
            var PoundSterling = new CurrencyInfo(new RegionInfo("GB"));

            var BeforeInPound = new Money(PoundSterling, 100m);

            var AfterInDram = await Bank.ConvertCurrencyAsync(BeforeInPound, ArmenianDram, DateTime.Now, default).ConfigureAwait(true);

            Assert.True(BeforeInPound.Amount < AfterInDram.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var Initial = new Money(pair.BaseCurrency, 10m);
                var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default).ConfigureAwait(true);
                var Result = await Bank.ConvertCurrencyAsync(Initial, pair.CounterCurrency, DateTime.Now, default).ConfigureAwait(true);

                Assert.True(Result.Currency == pair.CounterCurrency);
                Assert.True(Result.Amount > 0m);
                Assert.True(Result.Amount == (rate * Initial.Amount));
            }
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var UnitedStates = new RegionInfo("US");
            var Armenia = new RegionInfo("AM");

            var Dollar = new CurrencyInfo(UnitedStates);
            var Dram = new CurrencyInfo(Armenia);

            var Before = new Money(Dollar, 100m);

            _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, Dram, DateTime.Now.AddDays(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ConvertCurrency003Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var UnitedStates = new RegionInfo("US");
            var Armenia = new RegionInfo("AM");

            var Dollar = new CurrencyInfo(UnitedStates);
            var Dram = new CurrencyInfo(Armenia);

            var Before = new Money(Dollar, 100m);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, Dram, DateTime.Now.AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ConvertCurrency004Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var UnitedStates = new RegionInfo("US");
            var Armenia = new RegionInfo("AM");

            var Dollar = new CurrencyInfo(UnitedStates);
            var Dram = new CurrencyInfo(Armenia);

            var Before = new Money(Dollar, 100m);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, Dram, DateTime.Now.AddDays(-20d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task Fetch001Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            _ = await Bank.GetExchangeRatesAsync(DateTimeOffset.Now, default).ConfigureAwait(true);
        }

        [Fact]
        public async Task Fetch002Async()
        {
            var passed = false;

            var stringValue = string.Empty;

            await Task.Run(async () =>
            {
                var ci = new CultureInfo("ru-RU");
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;

                var bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

                _ = await bank.GetExchangeRatesAsync(this._timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                passed = true;
            }).ConfigureAwait(true);

            Assert.True(passed);
        }

        [Fact]
        public async Task GetCurrencyPairs001Async()
        {
            var bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var pairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in await bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true))
            {
                _ = pairs.Add(pair);
            }

            Assert.True(pairs.Count == (await bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true)).Count());
        }

        [Fact]
        public async Task GetCurrencyPairs002Async()
        {
            var bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "USD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "USD");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "GBP" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "GBP");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AUD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "AUD");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "EUR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "EUR");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "XDR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "XDR");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "IRR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "IRR");


            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "PLN" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "PLN");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "CAD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CAD");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "INR" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "INR");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "JPY" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "JPY");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "NOK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "NOK");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "SEK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "SEK");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "CHF" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CHF");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "CZK" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CZK");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "CNY" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "CNY");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "SGD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "SGD");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AED" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "AED");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "KGS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "KGS");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "KZT" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "KZT");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "RUB" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "RUB");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "UAH" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "UAH");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "UZS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "UZS");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "BYN" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "BYN");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "TJS" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "TJS");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "GEL" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "GEL");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "HKD" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "HKD");

            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "BRL" && c.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(currencyPairs, c => c.BaseCurrency.ISOCurrencySymbol == "AMD" && c.CounterCurrency.ISOCurrencySymbol == "BRL");
        }

        [Fact]
        public async Task GetCurrencyPairs003Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var reverse = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(CurrencyPairs, C => C == reverse);
            }
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                Assert.True(await Bank.GetExchangeRateAsync(pair, DateTime.Now, default).ConfigureAwait(true) > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            foreach (var pair in CurrencyPairs)
            {
                var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Equal(decimal.One, Math.Round(await Bank.GetExchangeRateAsync(pair, DateTime.Now, default).ConfigureAwait(true) * await Bank.GetExchangeRateAsync(reversePair, DateTime.Now, default).ConfigureAwait(true), 5));
            }
        }

        [Fact]
        public async Task GetExchangeRate003Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var UnitedStates = new RegionInfo("US");
            var Armenia = new RegionInfo("AM");

            var Dollar = new CurrencyInfo(UnitedStates);
            var Dram = new CurrencyInfo(Armenia);

            var DollarPerDram = new CurrencyPair(Dollar, Dram);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetExchangeRateAsync(DollarPerDram, DateTime.Now.AddDays(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var UnitedStates = new RegionInfo("US");
            var Armenia = new RegionInfo("AM");

            var Dollar = new CurrencyInfo(UnitedStates);
            var Dram = new CurrencyInfo(Armenia);

            var DollarPerDram = new CurrencyPair(Dollar, Dram);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetExchangeRateAsync(DollarPerDram, DateTime.Now.AddDays(-20d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate005Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var UnitedStates = new RegionInfo("US");
            var Armenia = new RegionInfo("AM");

            var Dollar = new CurrencyInfo(UnitedStates);
            var Dram = new CurrencyInfo(Armenia);

            var DollarPerDram = new CurrencyPair(Dollar, Dram);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetExchangeRateAsync(DollarPerDram, DateTime.Now.AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate006Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var Albania = new RegionInfo("AL");
            var Armenia = new RegionInfo("AM");

            var Lek = new CurrencyInfo(Albania);
            var Dram = new CurrencyInfo(Armenia);

            var LekPerDram = new CurrencyPair(Lek, Dram);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetExchangeRateAsync(LekPerDram, DateTime.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate007Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var Albania = new RegionInfo("AL");
            var Armenia = new RegionInfo("AM");

            var Lek = new CurrencyInfo(Albania);
            var Dram = new CurrencyInfo(Armenia);

            var DramPerLek = new CurrencyPair(Dram, Lek);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.GetExchangeRateAsync(DramPerLek, DateTime.Now, default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }
}
