using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Globalization;
using Xunit;

namespace TIKSN.Tests.DependencyInjection;

public class RegistrationsTests : IDisposable
{
    private readonly IHost host;
    private bool disposedValue;

    public RegistrationsTests()
    {
        var builder = Host.CreateDefaultBuilder();

        _ = builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        _ = builder.ConfigureServices((_, services) => services.AddFrameworkCore());

        _ = builder.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule<CoreModule>());

        this.host = builder.Build();
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData(typeof(TimeProvider))]
    [InlineData(typeof(ICultureFactory))]
    [InlineData(typeof(IRegionFactory))]
    [InlineData(typeof(ICurrencyFactory))]
    public void GivenHost_WhenServiceResolved_ThenItShouldNotBeNull(Type serviceType)
    {
        var service = this.host.Services.GetRequiredService(serviceType);

        _ = service.ShouldNotBeNull();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.host.Dispose();
            }

            this.disposedValue = true;
        }
    }
}
