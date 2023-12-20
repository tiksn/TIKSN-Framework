using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
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

        await exchangeRateService.InitializeAsync(default).ConfigureAwait(false);

        // Act

        _ = await exchangeRateService.ConvertCurrencyAsync(
            usd10, eur, DateTimeOffset.Now, default).ConfigureAwait(false);
        var firstPageResult = await exchangeRateRepository.PageAsync(firstPageQuery, default).ConfigureAwait(false);
        var items = await exchangeRateRepository.StreamAllAsync(default).Take(10).ToListAsync(default).ConfigureAwait(false);
        var totalItems = await exchangeRateRepository.StreamAllAsync(default).LongCountAsync(default).ConfigureAwait(false);

        // Assert

        _ = firstPageResult.Should().NotBeNull();
        _ = firstPageResult.Page.Number.Should().Be(1);
        _ = firstPageResult.Page.Size.Should().Be(10);
        _ = firstPageResult.Items.Should().NotBeNull();
        _ = firstPageResult.Items.Should().BeEquivalentTo(items);
        _ = firstPageResult.TotalItems.Should<Option<long>>().Be(totalItems);
        _ = firstPageResult.TotalPages.Should<Option<long>>().Be((long)Math.Ceiling(totalItems / 10m));
    }
}
