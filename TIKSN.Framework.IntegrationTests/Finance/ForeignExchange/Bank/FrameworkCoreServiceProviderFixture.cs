using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Chameleon.Net.Http;
using Chameleon.Net.Profiles;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Finance.ForeignExchange.Bank;

[assembly: Xunit.AssemblyFixture(
    typeof(TIKSN.IntegrationTests.Finance.ForeignExchange.Bank.FrameworkCoreServiceProviderFixture))]

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.Bank;

public sealed class FrameworkCoreServiceProviderFixture : IDisposable
{
    private readonly ServiceProvider serviceProvider;

    public FrameworkCoreServiceProviderFixture()
    {
        var services = new ServiceCollection();
        _ = services.AddFrameworkCore();
        _ = AddReserveBankOfNewZealandBrowserClient(services);
        this.serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose() => this.serviceProvider.Dispose();

    public T GetRequiredService<T>()
        where T : notnull
        => this.serviceProvider.GetRequiredService<T>();

    internal static IServiceCollection AddReserveBankOfNewZealandBrowserClient(IServiceCollection services)
    {
        var builder =
            services.AddHttpClient<IReserveBankOfNewZealand, ReserveBankOfNewZealand>(ConfigureBrowserHttpClient);
        _ = builder.ConfigurePrimaryHttpMessageHandler(static () =>
            new ChameleonHttpMessageHandler(BuiltInProfiles.Chromium152Windows));

        return services;
    }

    private static void ConfigureBrowserHttpClient(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/148.0.0.0 Safari/537.36 Edg/148.0.0.0");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/avif"));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/apng"));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/signed-exchange", 0.7)
            {
                Parameters =
                {
                    new NameValueHeaderValue("v", "b3"),
                },
            });
        httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
        httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en", 0.9));
        httpClient.DefaultRequestHeaders.Add("Sec-CH-UA",
            "\"Chromium\";v=\"148\", \"Microsoft Edge\";v=\"148\", \"Not/A)Brand\";v=\"99\"");
        httpClient.DefaultRequestHeaders.Add("Sec-CH-UA-Mobile", "?0");
        httpClient.DefaultRequestHeaders.Add("Sec-CH-UA-Arch", "\"x86\"");
        httpClient.DefaultRequestHeaders.Add("Sec-CH-UA-Platform", "\"Windows\"");
        httpClient.DefaultRequestHeaders.Add("Sec-CH-UA-Platform-Version", "\"19.0.0\"");
        httpClient.DefaultRequestHeaders.Add("Sec-CH-UA-Model", "\"\"");
        httpClient.DefaultRequestHeaders.Add("Sec-CH-UA-Bitness", "\"64\"");
        httpClient.DefaultRequestHeaders.Add("Sec-CH-UA-Full-Version-List",
            "\"Chromium\";v=\"148.0.7778.217\", \"Microsoft Edge\";v=\"148.0.3967.96\", " +
            "\"Not/A)Brand\";v=\"99.0.0.0\"");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Referrer = new Uri(
            "https://www.rbnz.govt.nz/statistics/series/exchange-and-interest-rates/exchange-rates-and-the-trade-weighted-index");
    }
}
