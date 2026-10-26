using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenFeature;
using OpenFeature.Model;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Finance;
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

        _ = builder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            containerBuilder.RegisterModule<CoreModule>());

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
    [InlineData(typeof(ICurrencyPairFactory))]
    [InlineData(typeof(IDateTimeZoneLookup))]
    [InlineData(typeof(ITimeZoneInfoLookup))]
    [InlineData(typeof(IFeatureClient))]
    [InlineData(typeof(ITransactionContextPropagator))]
    public void GivenHost_WhenServiceResolved_ThenItShouldNotBeNull(Type serviceType)
    {
        var service = this.host.Services.GetRequiredService(serviceType);

        _ = service.ShouldNotBeNull();
    }

    [Fact]
    public void GivenHost_WhenTransactionContextSetViaPropagator_ThenPropagatorReturnsContext()
    {
        var propagator = this.host.Services.GetRequiredService<ITransactionContextPropagator>();
        var context = EvaluationContext.Builder().Set("test-key", "test-value").Build();

        propagator.SetTransactionContext(context);
        try
        {
            propagator.GetTransactionContext().GetValue("test-key").AsString.ShouldBe("test-value");
        }
        finally
        {
            propagator.SetTransactionContext(EvaluationContext.Empty);
        }
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
