using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.DependencyInjection.Tests;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;
using Xunit.Abstractions;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    public class CurrencylayerDotComTests
    {
        private string accessKey = "<put your access key here>";
        private readonly IServiceProvider _serviceProvider;
        private readonly ICurrencyFactory _currencyFactory;
        private readonly ITimeProvider _timeProvider;

        public CurrencylayerDotComTests(ITestOutputHelper testOutputHelper)
        {
            _serviceProvider = new TestCompositionRootSetup(testOutputHelper).CreateServiceProvider();
            _currencyFactory = _serviceProvider.GetRequiredService<ICurrencyFactory>();
            _timeProvider = _serviceProvider.GetRequiredService<ITimeProvider>();
        }

        //[Fact]
        public async Task GetCurrencyPairs001()
        {
            var exchange = new CurrencylayerDotCom(_currencyFactory, _timeProvider, accessKey);

            var pairs = await exchange.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            Assert.True(pairs.Count() > 0);
        }

        //[Fact]
        public async Task GetExchangeRateAsync001()
        {
            var exchange = new CurrencylayerDotCom(_currencyFactory, _timeProvider, accessKey);

            var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("UAH"));

            var rate = await exchange.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

            Assert.True(rate > decimal.Zero);
        }
    }
}