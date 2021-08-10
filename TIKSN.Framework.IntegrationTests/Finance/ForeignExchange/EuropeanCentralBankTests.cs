using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    public class EuropeanCentralBankTests
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public EuropeanCentralBankTests()
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
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (var pair in pairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);
                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default);

                Assert.True(After.Amount == Before.Amount * rate);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task Calculation002()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var OneYearsAgo = DateTime.Now.AddYears(-1);
            var pairs = await Bank.GetCurrencyPairsAsync(OneYearsAgo, default);

            foreach (var pair in pairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, OneYearsAgo, default);
                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, OneYearsAgo, default);

                Assert.True(After.Amount == Before.Amount * rate);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConversionDirection001()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var Euro = new CurrencyInfo(new System.Globalization.RegionInfo("DE"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInEuro = new Money(Euro, 100m);

            var AfterInPound = await Bank.ConvertCurrencyAsync(BeforeInEuro, PoundSterling, DateTime.Now, default);

            Assert.True(BeforeInEuro.Amount > AfterInPound.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (var pair in pairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);
                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default);

                Assert.True(After.Amount > 0m);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (var pair in pairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddMinutes(10d), default));
            }
        }

        [Fact]
        public async Task ConvertCurrency003()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
            var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

            Money Before = new Money(AMD, 10m);

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.ConvertCurrencyAsync(Before, ALL, DateTime.Now, default));
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (var pair in pairs)
            {
                var ReversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.True(pairs.Any(P => P == ReversedPair));
            }
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);
            var uniquePairs = new System.Collections.Generic.HashSet<CurrencyPair>();

            foreach (var pair in pairs)
            {
                bool WasUnique = uniquePairs.Add(pair);

                if (!WasUnique)
                {
                    System.Diagnostics.Debug.WriteLine(pair);
                }
            }

            Assert.True(uniquePairs.Count == pairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs003()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetCurrencyPairsAsync(DateTime.Now.AddMinutes(10d), default));
        }

        [Fact]
        public async Task GetCurrencyPairs004()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            Assert.Contains(pairs, P => P.ToString() == "AUD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "BGN/EUR");
            Assert.Contains(pairs, P => P.ToString() == "BRL/EUR");
            Assert.Contains(pairs, P => P.ToString() == "CAD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "CHF/EUR");
            Assert.Contains(pairs, P => P.ToString() == "CNY/EUR");
            Assert.Contains(pairs, P => P.ToString() == "CZK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "DKK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "GBP/EUR");
            Assert.Contains(pairs, P => P.ToString() == "HKD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "HRK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "HUF/EUR");
            Assert.Contains(pairs, P => P.ToString() == "IDR/EUR");
            Assert.Contains(pairs, P => P.ToString() == "ILS/EUR");
            Assert.Contains(pairs, P => P.ToString() == "INR/EUR");
            Assert.Contains(pairs, P => P.ToString() == "JPY/EUR");
            Assert.Contains(pairs, P => P.ToString() == "KRW/EUR");
            Assert.Contains(pairs, P => P.ToString() == "MXN/EUR");
            Assert.Contains(pairs, P => P.ToString() == "MYR/EUR");
            Assert.Contains(pairs, P => P.ToString() == "NOK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "NZD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "PHP/EUR");
            Assert.Contains(pairs, P => P.ToString() == "PLN/EUR");
            Assert.Contains(pairs, P => P.ToString() == "RON/EUR");
            Assert.Contains(pairs, P => P.ToString() == "RUB/EUR");
            Assert.Contains(pairs, P => P.ToString() == "SEK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "SGD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "THB/EUR");
            Assert.Contains(pairs, P => P.ToString() == "TRY/EUR");
            Assert.Contains(pairs, P => P.ToString() == "USD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "ZAR/EUR");

            Assert.Contains(pairs, P => P.ToString() == "EUR/AUD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/BGN");
            Assert.Contains(pairs, P => P.ToString() == "EUR/BRL");
            Assert.Contains(pairs, P => P.ToString() == "EUR/CAD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/CHF");
            Assert.Contains(pairs, P => P.ToString() == "EUR/CNY");
            Assert.Contains(pairs, P => P.ToString() == "EUR/CZK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/DKK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/GBP");
            Assert.Contains(pairs, P => P.ToString() == "EUR/HKD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/HRK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/HUF");
            Assert.Contains(pairs, P => P.ToString() == "EUR/IDR");
            Assert.Contains(pairs, P => P.ToString() == "EUR/ILS");
            Assert.Contains(pairs, P => P.ToString() == "EUR/INR");
            Assert.Contains(pairs, P => P.ToString() == "EUR/JPY");
            Assert.Contains(pairs, P => P.ToString() == "EUR/KRW");
            Assert.Contains(pairs, P => P.ToString() == "EUR/MXN");
            Assert.Contains(pairs, P => P.ToString() == "EUR/MYR");
            Assert.Contains(pairs, P => P.ToString() == "EUR/NOK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/NZD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/PHP");
            Assert.Contains(pairs, P => P.ToString() == "EUR/PLN");
            Assert.Contains(pairs, P => P.ToString() == "EUR/RON");
            Assert.Contains(pairs, P => P.ToString() == "EUR/RUB");
            Assert.Contains(pairs, P => P.ToString() == "EUR/SEK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/SGD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/THB");
            Assert.Contains(pairs, P => P.ToString() == "EUR/TRY");
            Assert.Contains(pairs, P => P.ToString() == "EUR/USD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/ZAR");
        }

        [Fact]
        public async Task GetCurrencyPairs005()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(new System.DateTime(2010, 1, 1), default);

            Assert.Contains(pairs, P => P.ToString() == "AUD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "BGN/EUR");
            Assert.Contains(pairs, P => P.ToString() == "BRL/EUR");
            Assert.Contains(pairs, P => P.ToString() == "CAD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "CHF/EUR");
            Assert.Contains(pairs, P => P.ToString() == "CNY/EUR");
            Assert.Contains(pairs, P => P.ToString() == "CZK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "DKK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "GBP/EUR");
            Assert.Contains(pairs, P => P.ToString() == "HKD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "HRK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "HUF/EUR");
            Assert.Contains(pairs, P => P.ToString() == "IDR/EUR");
            Assert.Contains(pairs, P => P.ToString() == "INR/EUR");
            Assert.Contains(pairs, P => P.ToString() == "JPY/EUR");
            Assert.Contains(pairs, P => P.ToString() == "KRW/EUR");
            Assert.Contains(pairs, P => P.ToString() == "LTL/EUR");
            Assert.Contains(pairs, P => P.ToString() == "MXN/EUR");
            Assert.Contains(pairs, P => P.ToString() == "MYR/EUR");
            Assert.Contains(pairs, P => P.ToString() == "NOK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "NZD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "PHP/EUR");
            Assert.Contains(pairs, P => P.ToString() == "PLN/EUR");
            Assert.Contains(pairs, P => P.ToString() == "RON/EUR");
            Assert.Contains(pairs, P => P.ToString() == "RUB/EUR");
            Assert.Contains(pairs, P => P.ToString() == "SEK/EUR");
            Assert.Contains(pairs, P => P.ToString() == "SGD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "THB/EUR");
            Assert.Contains(pairs, P => P.ToString() == "TRY/EUR");
            Assert.Contains(pairs, P => P.ToString() == "USD/EUR");
            Assert.Contains(pairs, P => P.ToString() == "ZAR/EUR");

            Assert.Contains(pairs, P => P.ToString() == "EUR/AUD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/BGN");
            Assert.Contains(pairs, P => P.ToString() == "EUR/BRL");
            Assert.Contains(pairs, P => P.ToString() == "EUR/CAD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/CHF");
            Assert.Contains(pairs, P => P.ToString() == "EUR/CNY");
            Assert.Contains(pairs, P => P.ToString() == "EUR/CZK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/DKK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/GBP");
            Assert.Contains(pairs, P => P.ToString() == "EUR/HKD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/HRK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/HUF");
            Assert.Contains(pairs, P => P.ToString() == "EUR/IDR");
            Assert.Contains(pairs, P => P.ToString() == "EUR/INR");
            Assert.Contains(pairs, P => P.ToString() == "EUR/JPY");
            Assert.Contains(pairs, P => P.ToString() == "EUR/KRW");
            Assert.Contains(pairs, P => P.ToString() == "EUR/LTL");
            Assert.Contains(pairs, P => P.ToString() == "EUR/MXN");
            Assert.Contains(pairs, P => P.ToString() == "EUR/MYR");
            Assert.Contains(pairs, P => P.ToString() == "EUR/NOK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/NZD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/PHP");
            Assert.Contains(pairs, P => P.ToString() == "EUR/PLN");
            Assert.Contains(pairs, P => P.ToString() == "EUR/RON");
            Assert.Contains(pairs, P => P.ToString() == "EUR/RUB");
            Assert.Contains(pairs, P => P.ToString() == "EUR/SEK");
            Assert.Contains(pairs, P => P.ToString() == "EUR/SGD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/THB");
            Assert.Contains(pairs, P => P.ToString() == "EUR/TRY");
            Assert.Contains(pairs, P => P.ToString() == "EUR/USD");
            Assert.Contains(pairs, P => P.ToString() == "EUR/ZAR");
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (var pair in pairs)
            {
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

                Assert.True(rate > 0m);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (var pair in pairs)
            {
                await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(10d), default));
            }
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now.AddYears(-1), default);

            foreach (var pair in pairs)
            {
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddYears(-1), default);

                Assert.True(rate > 0m);
            }
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory, _timeProvider);

            var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
            var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

            var pair = new CurrencyPair(AMD, ALL);

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetExchangeRateAsync(pair, DateTime.Now, default));
        }
    }
}
