using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.Tests
{
    public class NationalBankOfUkraineTests
    {
        private readonly ICurrencyFactory _currencyFactory;

        public NationalBankOfUkraineTests()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            services.AddSingleton<IRegionFactory, RegionFactory>();

            var serviceProvider = services.BuildServiceProvider();
            _currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        }

        [Fact]
        public async Task ConvertCurrencyAsync001()
        {
            var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
            var nbu = new NationalBankOfUkraine(_currencyFactory);
            var pairs = await nbu.GetCurrencyPairsAsync(date, default);

            foreach (var pair in pairs)
            {
                var baseMoney = new Money(pair.BaseCurrency, 100);
                var convertedMoney = await nbu.ConvertCurrencyAsync(baseMoney, pair.CounterCurrency, date, default);

                Assert.Equal(pair.CounterCurrency, convertedMoney.Currency);
                Assert.True(convertedMoney.Amount > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetCurrencyPairsAsync001()
        {
            var nbu = new NationalBankOfUkraine(_currencyFactory);

            var pairs = await nbu.GetCurrencyPairsAsync(new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero), default);

            Assert.True(pairs.Any());
        }

        [Fact]
        public async Task GetExchangeRateAsync001()
        {
            var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
            var nbu = new NationalBankOfUkraine(_currencyFactory);
            var pairs = await nbu.GetCurrencyPairsAsync(date, default);

            foreach (var pair in pairs)
            {
                var rate = await nbu.GetExchangeRateAsync(pair, date, default);

                Assert.True(rate > decimal.Zero);
            }
        }
    }
}