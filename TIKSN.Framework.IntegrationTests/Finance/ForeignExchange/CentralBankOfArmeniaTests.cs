using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.IntegrationTests
{
    public class CentralBankOfArmeniaTests
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ICurrencyFactory currencyFactory;
        private readonly ITimeProvider timeProvider;

        public CentralBankOfArmeniaTests()
        {
            var services = new ServiceCollection();
            _ = services.AddMemoryCache();
            _ = services.AddHttpClient();
            _ = services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            _ = services.AddSingleton<IRegionFactory, RegionFactory>();
            _ = services.AddSingleton<ITimeProvider, TimeProvider>();

            var serviceProvider = services.BuildServiceProvider();
            this.currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
            this.timeProvider = serviceProvider.GetRequiredService<ITimeProvider>();
            this.httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        }

        [Fact]
        public async Task ConversionDirection001Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var armenianDram = new CurrencyInfo(new RegionInfo("AM"));
            var poundSterling = new CurrencyInfo(new RegionInfo("GB"));

            var beforeInPound = new Money(poundSterling, 100m);

            var afterInDram = await bank.ConvertCurrencyAsync(beforeInPound, armenianDram, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            Assert.True(beforeInPound.Amount < afterInDram.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var initial = new Money(pair.BaseCurrency, 10m);
                var rate = await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);
                var result = await bank.ConvertCurrencyAsync(initial, pair.CounterCurrency, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                Assert.True(result.Currency == pair.CounterCurrency);
                Assert.True(result.Amount > 0m);
                Assert.True(result.Amount == (rate * initial.Amount));
            }
        }

        [Fact]
        public async Task ConvertCurrency002Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var unitedStates = new RegionInfo("US");
            var armenia = new RegionInfo("AM");

            var dollar = new CurrencyInfo(unitedStates);
            var dram = new CurrencyInfo(armenia);

            var before = new Money(dollar, 100m);

            _ = await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetCurrentTime().AddDays(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ConvertCurrency003Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var unitedStates = new RegionInfo("US");
            var armenia = new RegionInfo("AM");

            var dollar = new CurrencyInfo(unitedStates);
            var dram = new CurrencyInfo(armenia);

            var before = new Money(dollar, 100m);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetCurrentTime().AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ConvertCurrency004Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var unitedStates = new RegionInfo("US");
            var armenia = new RegionInfo("AM");

            var dollar = new CurrencyInfo(unitedStates);
            var dram = new CurrencyInfo(armenia);

            var before = new Money(dollar, 100m);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.ConvertCurrencyAsync(before, dram, this.timeProvider.GetCurrentTime().AddDays(-20d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task Fetch001Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            _ = await bank.GetExchangeRatesAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);
        }

        [Fact]
        public async Task Fetch002Async()
        {
            var passed = false;

            await Task.Run(async () =>
            {
                var ci = new CultureInfo("ru-RU");
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;

                var bank = new CentralBankOfArmenia(
                    this.httpClientFactory,
                    this.currencyFactory,
                    this.timeProvider);

                _ = await bank.GetExchangeRatesAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

                passed = true;
            }).ConfigureAwait(true);

            Assert.True(passed);
        }

        [Fact]
        public async Task GetCurrencyPairs001Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var pairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true))
            {
                _ = pairs.Add(pair);
            }

            Assert.True(pairs.Count == (await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).Count());
        }

        [Fact]
        public async Task GetCurrencyPairs002Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

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
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var reverse = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(currencyPairs, C => C == reverse);
            }
        }

        [Fact]
        public async Task GetExchangeRate001Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                Assert.True(await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true) > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var currencyPairs = await bank.GetCurrencyPairsAsync(this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true);

            foreach (var pair in currencyPairs)
            {
                var reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Equal(decimal.One, Math.Round(await bank.GetExchangeRateAsync(pair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true) * await bank.GetExchangeRateAsync(reversePair, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true), 5));
            }
        }

        [Fact]
        public async Task GetExchangeRate003Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var unitedStates = new RegionInfo("US");
            var armenia = new RegionInfo("AM");

            var dollar = new CurrencyInfo(unitedStates);
            var dram = new CurrencyInfo(armenia);

            var dollarPerDram = new CurrencyPair(dollar, dram);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetCurrentTime().AddDays(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate004Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var unitedStates = new RegionInfo("US");
            var armenia = new RegionInfo("AM");

            var dollar = new CurrencyInfo(unitedStates);
            var dram = new CurrencyInfo(armenia);

            var dollarPerDram = new CurrencyPair(dollar, dram);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetCurrentTime().AddDays(-20d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate005Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var unitedStates = new RegionInfo("US");
            var armenia = new RegionInfo("AM");

            var dollar = new CurrencyInfo(unitedStates);
            var dram = new CurrencyInfo(armenia);

            var dollarPerDram = new CurrencyPair(dollar, dram);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(dollarPerDram, this.timeProvider.GetCurrentTime().AddMinutes(1d), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate006Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var albania = new RegionInfo("AL");
            var armenia = new RegionInfo("AM");

            var lek = new CurrencyInfo(albania);
            var dram = new CurrencyInfo(armenia);

            var lekPerDram = new CurrencyPair(lek, dram);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(lekPerDram, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).ConfigureAwait(true);
        }

        [Fact]
        public async Task GetExchangeRate007Async()
        {
            var bank = new CentralBankOfArmenia(
                this.httpClientFactory,
                this.currencyFactory,
                this.timeProvider);

            var albania = new RegionInfo("AL");
            var armenia = new RegionInfo("AM");

            var lek = new CurrencyInfo(albania);
            var dram = new CurrencyInfo(armenia);

            var dramPerLek = new CurrencyPair(dram, lek);

            _ = await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await bank.GetExchangeRateAsync(dramPerLek, this.timeProvider.GetCurrentTime(), default).ConfigureAwait(true)).ConfigureAwait(true);
        }
    }
}
