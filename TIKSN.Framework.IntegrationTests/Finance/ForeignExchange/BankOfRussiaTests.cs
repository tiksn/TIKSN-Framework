using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    public class BankOfRussiaTests
    {
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public BankOfRussiaTests()
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
        public async Task Calculate001()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now, default))
            {
                Money Before = new Money(pair.BaseCurrency, 10m);
                decimal rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);
                Money After = await Bank.ConvertCurrencyAsync(Before, pair.CounterCurrency, DateTime.Now, default);

                Assert.True(After.Amount == rate * Before.Amount);
                Assert.True(After.Currency == pair.CounterCurrency);
            }
        }

        [Fact]
        public async Task ConvertCurrency001()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            var Moment = DateTime.Now;

            foreach (var pair in await Bank.GetCurrencyPairsAsync(Moment, default))
            {
                Money BeforeConversion = new Money(pair.BaseCurrency, 100m);

                Money AfterComversion = await Bank.ConvertCurrencyAsync(BeforeConversion, pair.CounterCurrency, Moment, default);

                Assert.True(AfterComversion.Amount > 0m);
                Assert.Equal<Finance.CurrencyInfo>(pair.CounterCurrency, AfterComversion.Currency);
            }
        }

        [Fact]
        public async Task ConvertCurrency002()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
            System.Globalization.RegionInfo RU = new System.Globalization.RegionInfo("RU");

            CurrencyInfo USD = new CurrencyInfo(US);
            CurrencyInfo RUB = new CurrencyInfo(RU);

            Money Before = new Money(USD, 100m);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, RUB, DateTimeOffset.Now.AddMinutes(1d), default));
        }

        [Fact]
        public async Task ConvertCurrency003()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            System.Globalization.RegionInfo AO = new System.Globalization.RegionInfo("AO");
            System.Globalization.RegionInfo BW = new System.Globalization.RegionInfo("BW");

            CurrencyInfo AOA = new CurrencyInfo(AO);
            CurrencyInfo BWP = new CurrencyInfo(BW);

            Money Before = new Money(AOA, 100m);

            await
                Assert.ThrowsAsync<ArgumentException>(
                    async () =>
                        await Bank.ConvertCurrencyAsync(Before, BWP, DateTime.Now, default));
        }

        [Fact]
        public async Task GetCurrencyPairs001()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                CurrencyPair reversePair = new CurrencyPair(pair.CounterCurrency, pair.BaseCurrency);

                Assert.Contains(CurrencyPairs, C => C == reversePair);
            }
        }

        [Fact]
        public async Task GetCurrencyPairs002()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            System.Collections.Generic.HashSet<CurrencyPair> pairSet = new System.Collections.Generic.HashSet<CurrencyPair>();

            var CurrencyPairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            foreach (CurrencyPair pair in CurrencyPairs)
            {
                pairSet.Add(pair);
            }

            Assert.True(pairSet.Count == CurrencyPairs.Count());
        }

        [Fact]
        public async Task GetCurrencyPairs003()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.GetCurrencyPairsAsync(DateTime.Now.AddDays(10), default));
        }

        [Fact]
        public async Task GetCurrencyPairs004()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(DateTime.Now, default);

            Assert.Contains(pairs, C => C.ToString() == "AUD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "AZN/RUB");
            Assert.Contains(pairs, C => C.ToString() == "AMD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "BYN/RUB");
            Assert.Contains(pairs, C => C.ToString() == "BGN/RUB");
            Assert.Contains(pairs, C => C.ToString() == "BRL/RUB");
            Assert.Contains(pairs, C => C.ToString() == "HUF/RUB");
            Assert.Contains(pairs, C => C.ToString() == "KRW/RUB");
            Assert.Contains(pairs, C => C.ToString() == "DKK/RUB");
            Assert.Contains(pairs, C => C.ToString() == "USD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "EUR/RUB");
            Assert.Contains(pairs, C => C.ToString() == "INR/RUB");
            Assert.Contains(pairs, C => C.ToString() == "KZT/RUB");
            Assert.Contains(pairs, C => C.ToString() == "CAD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "KGS/RUB");
            Assert.Contains(pairs, C => C.ToString() == "CNY/RUB");
            Assert.Contains(pairs, C => C.ToString() == "MDL/RUB");
            Assert.Contains(pairs, C => C.ToString() == "RON/RUB");
            Assert.Contains(pairs, C => C.ToString() == "TMT/RUB");
            Assert.Contains(pairs, C => C.ToString() == "NOK/RUB");
            Assert.Contains(pairs, C => C.ToString() == "PLN/RUB");
            Assert.Contains(pairs, C => C.ToString() == "SGD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "TJS/RUB");
            Assert.Contains(pairs, C => C.ToString() == "TRY/RUB");
            Assert.Contains(pairs, C => C.ToString() == "UZS/RUB");
            Assert.Contains(pairs, C => C.ToString() == "UAH/RUB");
            Assert.Contains(pairs, C => C.ToString() == "GBP/RUB");
            Assert.Contains(pairs, C => C.ToString() == "CZK/RUB");
            Assert.Contains(pairs, C => C.ToString() == "SEK/RUB");
            Assert.Contains(pairs, C => C.ToString() == "CHF/RUB");
            Assert.Contains(pairs, C => C.ToString() == "ZAR/RUB");
            Assert.Contains(pairs, C => C.ToString() == "JPY/RUB");

            Assert.Contains(pairs, C => C.ToString() == "RUB/AUD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/AZN");
            Assert.Contains(pairs, C => C.ToString() == "RUB/AMD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/BYN");
            Assert.Contains(pairs, C => C.ToString() == "RUB/BGN");
            Assert.Contains(pairs, C => C.ToString() == "RUB/BRL");
            Assert.Contains(pairs, C => C.ToString() == "RUB/HUF");
            Assert.Contains(pairs, C => C.ToString() == "RUB/KRW");
            Assert.Contains(pairs, C => C.ToString() == "RUB/DKK");
            Assert.Contains(pairs, C => C.ToString() == "RUB/USD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/EUR");
            Assert.Contains(pairs, C => C.ToString() == "RUB/INR");
            Assert.Contains(pairs, C => C.ToString() == "RUB/KZT");
            Assert.Contains(pairs, C => C.ToString() == "RUB/CAD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/KGS");
            Assert.Contains(pairs, C => C.ToString() == "RUB/CNY");
            Assert.Contains(pairs, C => C.ToString() == "RUB/MDL");
            Assert.Contains(pairs, C => C.ToString() == "RUB/RON");
            Assert.Contains(pairs, C => C.ToString() == "RUB/TMT");
            Assert.Contains(pairs, C => C.ToString() == "RUB/NOK");
            Assert.Contains(pairs, C => C.ToString() == "RUB/PLN");
            Assert.Contains(pairs, C => C.ToString() == "RUB/SGD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/TJS");
            Assert.Contains(pairs, C => C.ToString() == "RUB/TRY");
            Assert.Contains(pairs, C => C.ToString() == "RUB/UZS");
            Assert.Contains(pairs, C => C.ToString() == "RUB/UAH");
            Assert.Contains(pairs, C => C.ToString() == "RUB/GBP");
            Assert.Contains(pairs, C => C.ToString() == "RUB/CZK");
            Assert.Contains(pairs, C => C.ToString() == "RUB/SEK");
            Assert.Contains(pairs, C => C.ToString() == "RUB/CHF");
            Assert.Contains(pairs, C => C.ToString() == "RUB/ZAR");
            Assert.Contains(pairs, C => C.ToString() == "RUB/JPY");
        }

        [Fact]
        public async Task GetCurrencyPairs005()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            var pairs = await Bank.GetCurrencyPairsAsync(new System.DateTime(2010, 01, 01), default);

            Assert.Contains(pairs, C => C.ToString() == "AUD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "BYR/RUB");
            Assert.Contains(pairs, C => C.ToString() == "DKK/RUB");
            Assert.Contains(pairs, C => C.ToString() == "USD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "EUR/RUB");
            Assert.Contains(pairs, C => C.ToString() == "ISK/RUB");
            Assert.Contains(pairs, C => C.ToString() == "KZT/RUB");
            Assert.Contains(pairs, C => C.ToString() == "CAD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "CNY/RUB");
            Assert.Contains(pairs, C => C.ToString() == "NOK/RUB");
            Assert.Contains(pairs, C => C.ToString() == "SGD/RUB");
            Assert.Contains(pairs, C => C.ToString() == "TRY/RUB");
            Assert.Contains(pairs, C => C.ToString() == "UAH/RUB");
            Assert.Contains(pairs, C => C.ToString() == "GBP/RUB");
            Assert.Contains(pairs, C => C.ToString() == "SEK/RUB");
            Assert.Contains(pairs, C => C.ToString() == "CHF/RUB");
            Assert.Contains(pairs, C => C.ToString() == "JPY/RUB");

            Assert.Contains(pairs, C => C.ToString() == "RUB/AUD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/BYR");
            Assert.Contains(pairs, C => C.ToString() == "RUB/DKK");
            Assert.Contains(pairs, C => C.ToString() == "RUB/USD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/EUR");
            Assert.Contains(pairs, C => C.ToString() == "RUB/ISK");
            Assert.Contains(pairs, C => C.ToString() == "RUB/KZT");
            Assert.Contains(pairs, C => C.ToString() == "RUB/CAD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/CNY");
            Assert.Contains(pairs, C => C.ToString() == "RUB/NOK");
            Assert.Contains(pairs, C => C.ToString() == "RUB/SGD");
            Assert.Contains(pairs, C => C.ToString() == "RUB/TRY");
            Assert.Contains(pairs, C => C.ToString() == "RUB/UAH");
            Assert.Contains(pairs, C => C.ToString() == "RUB/GBP");
            Assert.Contains(pairs, C => C.ToString() == "RUB/SEK");
            Assert.Contains(pairs, C => C.ToString() == "RUB/CHF");
            Assert.Contains(pairs, C => C.ToString() == "RUB/JPY");
        }

        [Fact]
        public async Task GetCurrencyPairs006()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            var AtTheMoment = DateTime.Now;

            var pairs = await Bank.GetCurrencyPairsAsync(AtTheMoment, default);

            var WebUrl = string.Format("https://www.cbr.ru/scripts/XML_daily.asp?date_req={2:00}.{1:00}.{0}", AtTheMoment.Year, AtTheMoment.Month, AtTheMoment.Day);

            using (var httpClient = new HttpClient())
            {
                var responseStream = await httpClient.GetStreamAsync(WebUrl);

                var stream​Reader = new Stream​Reader(responseStream, Encoding.UTF7);

                var Xdoc = XDocument.Load(stream​Reader);

                var WebPairs = new System.Collections.Generic.List<string>();

                foreach (var CodeElement in Xdoc.Element("ValCurs").Elements("Valute"))
                {
                    string code = CodeElement.Element("CharCode").Value.Trim().ToUpper();

                    WebPairs.Add(string.Format("{0}/RUB", code));
                    WebPairs.Add(string.Format("RUB/{0}", code));
                }

                foreach (var Pair in pairs)
                {
                    Assert.Contains(WebPairs, WP => WP == Pair.ToString());
                }

                foreach (var WebPair in WebPairs)
                {
                    Assert.True(pairs.Any(P => P.ToString() == WebPair));
                }

                Assert.True(pairs.Count() == WebPairs.Count);
            }
        }

        [Fact]
        public async Task GetCurrencyPairs007()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            for (int year = 1994; year <= DateTime.Now.Year; year++)
            {
                for (int month = 1; (year < DateTime.Now.Year && month <= 12) || month <= DateTime.Now.Month; month++)
                {
                    var date = new System.DateTime(year, month, 1);

                    await Bank.GetCurrencyPairsAsync(date, default);
                }
            }
        }

        [Fact]
        public async Task GetExchangeRate001()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(DateTime.Now, default))
            {
                var rate = await Bank.GetExchangeRateAsync(pair, DateTime.Now, default);

                Assert.True(rate > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetExchangeRate002()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
            System.Globalization.RegionInfo RU = new System.Globalization.RegionInfo("RU");

            CurrencyInfo USD = new CurrencyInfo(US);
            CurrencyInfo RUB = new CurrencyInfo(RU);

            CurrencyPair pair = new CurrencyPair(RUB, USD);

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.GetExchangeRateAsync(pair, DateTime.Now.AddMinutes(1d), default));
        }

        [Fact]
        public async Task GetExchangeRate003()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            System.Globalization.RegionInfo AO = new System.Globalization.RegionInfo("AO");
            System.Globalization.RegionInfo BW = new System.Globalization.RegionInfo("BW");

            CurrencyInfo AOA = new CurrencyInfo(AO);
            CurrencyInfo BWP = new CurrencyInfo(BW);

            CurrencyPair pair = new CurrencyPair(BWP, AOA);

            await
                    Assert.ThrowsAsync<ArgumentException>(
                        async () =>
                            await Bank.GetExchangeRateAsync(pair, DateTime.Now, default));
        }

        [Fact]
        public async Task GetExchangeRate004()
        {
            var Bank = new BankOfRussia(_currencyFactory, _timeProvider);

            var Moment = DateTime.Now.AddYears(-1);

            foreach (var pair in await Bank.GetCurrencyPairsAsync(Moment, default))
            {
                var rate = await Bank.GetExchangeRateAsync(pair, Moment, default);

                Assert.True(rate > decimal.Zero);
            }
        }
    }
}
