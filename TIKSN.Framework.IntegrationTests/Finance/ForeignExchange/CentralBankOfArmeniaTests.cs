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
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var pairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true))
            {
                _ = pairs.Add(pair);
            }

            Assert.True(pairs.Count == (await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true)).Count());
        }

        [Fact]
        public async Task GetCurrencyPairs002Async()
        {
            var Bank = new CentralBankOfArmenia(this._currencyFactory, this._timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default).ConfigureAwait(true);

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "USD" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "USD");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "GBP" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "GBP");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AUD" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "AUD");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "ARS" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ARS");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "DKK" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "DKK");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "EGP" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "EGP");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "EUR" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "EUR");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "TRY" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TRY");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "IRR" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "IRR");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "ILS" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ILS");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "PLN" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "PLN");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "LBP" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "LBP");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "CAD" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CAD");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "INR" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "INR");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "HUF" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "HUF");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "JPY" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "JPY");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "NOK" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "NOK");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "SEK" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SEK");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "CHF" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CHF");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "CZK" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CZK");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "CNY" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "CNY");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "SGD" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SGD");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "KRW" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KRW");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "MXN" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "MXN");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "SAR" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SAR");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "SYP" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "SYP");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AED" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "AED");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "KWD" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KWD");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "BGN" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BGN");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "RON" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "RON");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "ISK" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "ISK");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "KGS" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KGS");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "KZT" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "KZT");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "MDL" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "MDL");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "RUB" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "RUB");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "UAH" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "UAH");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "UZS" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "UZS");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "BYN" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BYN");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "TJS" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TJS");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "TMT" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "TMT");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "GEL" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "GEL");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "HKD" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "HKD");

            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "BRL" && C.CounterCurrency.ISOCurrencySymbol == "AMD");
            Assert.Contains(CurrencyPairs, C => C.BaseCurrency.ISOCurrencySymbol == "AMD" && C.CounterCurrency.ISOCurrencySymbol == "BRL");

            //Assert.Equal(88, CurrencyPairs.Count());
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
