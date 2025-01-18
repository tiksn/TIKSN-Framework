using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.Data;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.IntegrationTests.Data;

[Collection("ServiceProviderCollection")]
public class QueryRepositoryPaginationTests
{
    private readonly ServiceProviderFixture serviceProviderFixture;

    public QueryRepositoryPaginationTests(
        ServiceProviderFixture serviceProviderFixture)
        => this.serviceProviderFixture = serviceProviderFixture ?? throw new ArgumentNullException(nameof(serviceProviderFixture));

    [Theory]
    [InlineData("LiteDB")]
    [InlineData("SQLite")]
    [InlineData("MongoDB")]
    public async Task GivenExchangeRates_WhenFirstPageRequested_ThenResultShouldMatch(string database)
    {
        // Arrange

        var serviceProvider = this.serviceProviderFixture.GetServiceProvider(database);
        var exchangeRateService = serviceProvider.GetRequiredService<IExchangeRateService>();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        var exchangeRateRepository = serviceProvider.GetRequiredService<IExchangeRateRepository>();
        var firstPage = new Page(1, 10);
        var firstPageQuery = new PageQuery(firstPage, estimateTotalItems: true);
        var usd = currencyFactory.Create("USD");
        var eur = currencyFactory.Create("EUR");
        var usd10 = new Money(usd, 10m);

        await exchangeRateService.InitializeAsync(default);

        // Act

        _ = await exchangeRateService.ConvertCurrencyAsync(
            usd10, eur, DateTimeOffset.Now, default);
        var firstPageResult = await exchangeRateRepository.PageAsync(firstPageQuery, default);
        var items = await exchangeRateRepository.StreamAllAsync(default).Take(10).ToListAsync(default);
        var totalItems = await exchangeRateRepository.StreamAllAsync(default).LongCountAsync(default);

        // Assert

        _ = firstPageResult.ShouldNotBeNull();
        firstPageResult.Page.Number.ShouldBe(1);
        firstPageResult.Page.Size.ShouldBe(10);
        _ = firstPageResult.Items.ShouldNotBeNull();
        firstPageResult.Items.ShouldBeEquivalentTo(items);
        firstPageResult.TotalItems.ShouldBe(totalItems);
        firstPageResult.TotalPages.ShouldBe((long)Math.Ceiling(totalItems / 10m));
    }
}
