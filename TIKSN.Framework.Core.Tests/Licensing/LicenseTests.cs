using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Licensing;
using Xunit;

namespace TIKSN.Framework.Core.Tests.Licensing;

public class LicenseTests
{
    [Fact]
    public void XXX()
    {
        var services = new ServiceCollection();
        _ = services.AddSingleton<IEntitlementsConverter<TestEntitlements, TestLicenseEntitlements>, TestEntitlementsConverter>();
        _ = services.AddFrameworkPlatform();

        ContainerBuilder containerBuilder = new();
        _ = containerBuilder.RegisterModule<CoreModule>();
        _ = containerBuilder.RegisterModule<PlatformModule>();
        containerBuilder.Populate(services);
        var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());

        new TestLicenseEntitlements();
    }
}
