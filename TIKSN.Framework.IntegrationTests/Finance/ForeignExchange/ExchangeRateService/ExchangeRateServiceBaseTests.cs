using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    [Collection("LiteDbServiceProviderCollection")]
    public class ExchangeRateServiceBaseTests
    {
        private readonly LiteDbServiceProviderFixture liteDbServiceProviderFixture;

        public ExchangeRateServiceBaseTests(LiteDbServiceProviderFixture liteDbServiceProviderFixture)
            => this.liteDbServiceProviderFixture = liteDbServiceProviderFixture ?? throw new ArgumentNullException(nameof(liteDbServiceProviderFixture));

        [Fact]
        public async Task Given_10USD_When_ExchangedForEuro_Then_ResultShouldBeEuro()
        {
            // Arrange
            var exchangeRateService = this.liteDbServiceProviderFixture.Services.GetRequiredService<IExchangeRateService>();
            var currencyFactory = this.liteDbServiceProviderFixture.Services.GetRequiredService<ICurrencyFactory>();
            var usd = currencyFactory.Create("USD");
            var eur = currencyFactory.Create("EUR");
            var usd10 = new Money(usd, 10m);

            await exchangeRateService.InitializeAsync(default);

            // Act
            var result = await exchangeRateService.ConvertCurrencyAsync(
                usd10,
                eur,
                DateTimeOffset.Now,
                default);

            // Assert
            _ = result.Currency.Should().Be(eur);
        }
    }
}
