using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Cumulative;
using TIKSN.Framework.IntegrationTests;
using TIKSN.Globalization;
using TIKSN.Time;
using Xunit;

namespace TIKSN.Finance.Tests.ForeignExchange
{
    [Collection("ServiceProviderCollection")]
    public class CurrencylayerDotComTests
    {
        private readonly string accessKey = "<put your access key here>";
        private readonly ICurrencyFactory currencyFactory;
        private readonly ITimeProvider timeProvider;
        private readonly ServiceProviderFixture serviceProviderFixture;

        public CurrencylayerDotComTests(ServiceProviderFixture serviceProviderFixture)
        {
            this.currencyFactory = serviceProviderFixture.Services.GetRequiredService<ICurrencyFactory>();
            this.timeProvider = serviceProviderFixture.Services.GetRequiredService<ITimeProvider>();
            this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));
        }

        //[Fact]
        public async Task GetCurrencyPairs001()
        {
            var exchange = new CurrencylayerDotCom(this.currencyFactory, this.timeProvider, this.accessKey);

            var pairs = await exchange.GetCurrencyPairsAsync(DateTimeOffset.Now, default);

            Assert.True(pairs.Count() > 0);
        }

        //[Fact]
        public async Task GetExchangeRateAsync001()
        {
            var exchange = new CurrencylayerDotCom(this.currencyFactory, this.timeProvider, this.accessKey);

            var pair = new CurrencyPair(new CurrencyInfo("USD"), new CurrencyInfo("UAH"));

            var rate = await exchange.GetExchangeRateAsync(pair, DateTimeOffset.Now, default);

            Assert.True(rate > decimal.Zero);
        }
    }
}
