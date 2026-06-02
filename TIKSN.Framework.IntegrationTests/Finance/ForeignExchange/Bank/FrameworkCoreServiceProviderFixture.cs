using System;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;

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
        this.serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose() => this.serviceProvider.Dispose();

    public T GetRequiredService<T>()
        where T : notnull
        => this.serviceProvider.GetRequiredService<T>();
}
