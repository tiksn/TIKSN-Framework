using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
using TIKSN.Finance.ForeignExchange.Bank;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.Finance.ForeignExchange.Bank;

public class FederalReserveSystemTests
{
    [Fact]
    public async Task GivenCurrentFederalReserveUnitLabel_WhenExchangeRateRequested_ThenSeriesNameDeterminesDirection()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        var serviceProvider = services.BuildServiceProvider();
        var currencyFactory = serviceProvider.GetRequiredService<ICurrencyFactory>();
        using var httpClient = new HttpClient(new StringContentHandler(GetRatesXml()));
        var bank = new FederalReserveSystem(httpClient, currencyFactory, TimeProvider.System);

        var usd = new CurrencyInfo(new RegionInfo("US"));
        var cny = new CurrencyInfo(new RegionInfo("CN"));
        var eur = new CurrencyInfo(new RegionInfo("DE"));

        var cnyRate = await bank.GetExchangeRateAsync(new CurrencyPair(usd, cny), DateTimeOffset.UtcNow,
            TestContext.Current.CancellationToken);
        var eurRate = await bank.GetExchangeRateAsync(new CurrencyPair(usd, eur), DateTimeOffset.UtcNow,
            TestContext.Current.CancellationToken);

        cnyRate.ShouldBe(6.8m);
        eurRate.ShouldBe(0.8m);
    }

    private static string GetRatesXml()
        => """
           <?xml version="1.0" encoding="UTF-8"?>
           <message:MessageGroup xmlns:message="http://www.SDMX.org/resources/SDMXML/schemas/v1_0/message"
                                 xmlns:h10="http://www.federalreserve.gov/structure/compact/H10_H10"
                                 xmlns:common="http://www.federalreserve.gov/structure/compact/common">
             <common:DataSet>
               <h10:Series CURRENCY="CNY" FX="CNY" SERIES_NAME="RXI_N.B.CH" UNIT="Currency">
                 <common:Obs TIME_PERIOD="2026-05-22" OBS_VALUE="6.8" />
               </h10:Series>
               <h10:Series CURRENCY="EUR" FX="EUR" SERIES_NAME="RXI$US_N.B.EU" UNIT="Currency">
                 <common:Obs TIME_PERIOD="2026-05-22" OBS_VALUE="1.25" />
               </h10:Series>
             </common:DataSet>
           </message:MessageGroup>
           """;

    private sealed class StringContentHandler(string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/xml"),
            });
    }
}
