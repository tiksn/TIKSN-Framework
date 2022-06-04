using System;
using System.Collections.Generic;
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
        private readonly IReadOnlyDictionary<string, IServiceProvider> serviceProviders;

        public ExchangeRateServiceBaseTests(LiteDbServiceProviderFixture liteDbServiceProviderFixture)
            => this.serviceProviders = new Dictionary<string, IServiceProvider>()
            {
                { "LiteDB", liteDbServiceProviderFixture.Services}
            };

        [Theory]
        [InlineData("LiteDB")]
        public async Task Given_10USD_When_ExchangedForEuro_Then_ResultShouldBeEuro(string database)
        {
            // Arrange
            var exchangeRateService = this.serviceProviders[database].GetRequiredService<IExchangeRateService>();
            var currencyFactory = this.serviceProviders[database].GetRequiredService<ICurrencyFactory>();
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
