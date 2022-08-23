using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Globalization;
using TIKSN.IntegrationTests;
using Xunit;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    [Collection("ServiceProviderCollection")]
    public class ExchangeRateServiceBaseTests
    {
        private readonly ServiceProviderFixture serviceProviderFixture;

        public ExchangeRateServiceBaseTests(
            ServiceProviderFixture serviceProviderFixture)
            => this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));

        [Theory]
        [InlineData("LiteDB")]
        [InlineData("MongoDB")]
        public async Task Given_10USD_When_ExchangedForEuro_Then_ResultShouldBeEuro(string database)
        {
            // Arrange
            var exchangeRateService = this.serviceProviderFixture.GetServiceProvider(database).GetRequiredService<IExchangeRateService>();
            var currencyFactory = this.serviceProviderFixture.GetServiceProvider(database).GetRequiredService<ICurrencyFactory>();
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
