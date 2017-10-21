using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    public class EuropeanCentralBankTests
    {
        private readonly ICurrencyFactory _currencyFactory;

        public EuropeanCentralBankTests()
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
            var Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            foreach (var pair in pairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);
                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now);

                Assert.True(After.Amount == Before.Amount * rate);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task Calculation002()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory);

            var OneYearsAgo = DateTime.Now.AddYears(-1);
            var pairs = await Bank.GetCurrencyPairsAsync(OneYearsAgo);

            foreach (var pair in pairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, OneYearsAgo);
                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, OneYearsAgo);

                Assert.True(After.Amount == Before.Amount * rate);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConversionDirection001()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory);

            var Euro = new CurrencyInfo(new System.Globalization.RegionInfo("DE"));
            var PoundSterling = new CurrencyInfo(new System.Globalization.RegionInfo("GB"));

            var BeforeInEuro = new Money(Euro, 100m);

            var AfterInPound = await Bank.ConvertCurrencyAsync(BeforeInEuro, PoundSterling, DateTime.Now);

            Assert.True(BeforeInEuro.Amount > AfterInPound.Amount);
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            foreach (var pair in pairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);
                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now);

                Assert.True(After.Amount > 0m);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            foreach (var pair in pairs)
            {
                Money Before = new Money(pair.BaseCurrency, 10m);

                await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now.AddMinutes(10d)));
            }
        }

        [Fact]
        public async Task ConvertCurrency003()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory);

            var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
            var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

            Money Before = new Money(AMD, 10m);

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.ConvertCurrencyAsync(Before, ALL, DateTime.Now));
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            foreach (var pair in pairs)
            {
                var ReversedPair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.True(pairs.Any(P => P == ReversedPair));
            }
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);
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
            var Bank = new EuropeanCentralBank(_currencyFactory);

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetCurrencyPairsAsync(DateTime.Now.AddMinutes(10d)));
        }

        [Fact]
        public async Task GetCurrencyPairs004()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            Assert.True(pairs.Any(P => P.ToString() == "AUD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "BGN/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "BRL/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "CAD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "CHF/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "CNY/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "CZK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "DKK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "GBP/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "HKD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "HRK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "HUF/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "IDR/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "ILS/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "INR/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "JPY/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "KRW/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "MXN/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "MYR/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "NOK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "NZD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "PHP/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "PLN/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "RON/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "RUB/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "SEK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "SGD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "THB/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "TRY/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "USD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "ZAR/EUR"));

            Assert.True(pairs.Any(P => P.ToString() == "EUR/AUD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/BGN"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/BRL"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/CAD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/CHF"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/CNY"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/CZK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/DKK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/GBP"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/HKD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/HRK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/HUF"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/IDR"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/ILS"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/INR"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/JPY"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/KRW"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/MXN"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/MYR"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/NOK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/NZD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/PHP"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/PLN"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/RON"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/RUB"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/SEK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/SGD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/THB"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/TRY"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/USD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/ZAR"));

            //Assert.Equal(37 * 2, pairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs005()
        {
            var Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(new System.DateTime(2010, 1, 1));

            Assert.True(pairs.Any(P => P.ToString() == "AUD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "BGN/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "BRL/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "CAD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "CHF/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "CNY/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "CZK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "DKK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "GBP/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "HKD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "HRK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "HUF/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "IDR/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "INR/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "JPY/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "KRW/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "LTL/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "MXN/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "MYR/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "NOK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "NZD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "PHP/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "PLN/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "RON/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "RUB/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "SEK/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "SGD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "THB/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "TRY/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "USD/EUR"));
            Assert.True(pairs.Any(P => P.ToString() == "ZAR/EUR"));

            Assert.True(pairs.Any(P => P.ToString() == "EUR/AUD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/BGN"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/BRL"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/CAD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/CHF"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/CNY"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/CZK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/DKK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/GBP"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/HKD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/HRK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/HUF"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/IDR"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/INR"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/JPY"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/KRW"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/LTL"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/MXN"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/MYR"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/NOK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/NZD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/PHP"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/PLN"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/RON"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/RUB"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/SEK"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/SGD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/THB"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/TRY"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/USD"));
            Assert.True(pairs.Any(P => P.ToString() == "EUR/ZAR"));
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            foreach (var pair in pairs)
            {
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now);

                Assert.True(rate > 0m);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now);

            foreach (var pair in pairs)
            {
                await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(10d)));
            }
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now.AddYears(-1));

            foreach (var pair in pairs)
            {
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddYears(-1));

                Assert.True(rate > 0m);
            }
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            EuropeanCentralBank Bank = new EuropeanCentralBank(_currencyFactory);

            var AMD = new CurrencyInfo(new System.Globalization.RegionInfo("AM"));
            var ALL = new CurrencyInfo(new System.Globalization.RegionInfo("AL"));

            var pair = new CurrencyPair(AMD, ALL);

            await Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await Bank.GetExchangeRateAsync(pair, DateTime.Now));
        }
    }
}