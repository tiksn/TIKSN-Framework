using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
            _ = services.AddMemoryCache();
            _ = services.AddSingleton<ICurrencyFactory, CurrencyFactory>();
            _ = services.AddSingleton<IRegionFactory, RegionFactory>();

            var serviceProvider = services.BuildServiceProvider();
            this._currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        }

        [Fact]
        public async Task ConvertCurrencyAsync001Async()
        {
            var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
            var nbu = new NationalBankOfUkraine(this._currencyFactory);
            var pairs = await nbu.GetCurrencyPairsAsync(date, default).ConfigureAwait(true);

            foreach (var pair in pairs)
            {
                var baseMoney = new Money(pair.BaseCurrency, 100);
                var convertedMoney = await nbu.ConvertCurrencyAsync(baseMoney, pair.CounterCurrency, date, default).ConfigureAwait(true);

                Assert.Equal(pair.CounterCurrency, convertedMoney.Currency);
                Assert.True(convertedMoney.Amount > decimal.Zero);
            }
        }

        [Fact]
        public async Task GetCurrencyPairsAsync001Async()
        {
            var nbu = new NationalBankOfUkraine(this._currencyFactory);

            var pairs = await nbu.GetCurrencyPairsAsync(new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero), default).ConfigureAwait(true);

            Assert.True(pairs.Any());
        }

        [Fact]
        public async Task GetExchangeRateAsync001Async()
        {
            var date = new DateTimeOffset(2016, 05, 06, 0, 0, 0, TimeSpan.Zero);
            var nbu = new NationalBankOfUkraine(this._currencyFactory);
            var pairs = await nbu.GetCurrencyPairsAsync(date, default).ConfigureAwait(true);

            foreach (var pair in pairs)
            {
                var rate = await nbu.GetExchangeRateAsync(pair, date, default).ConfigureAwait(true);

                Assert.True(rate > decimal.Zero);
            }
        }
    }
}
