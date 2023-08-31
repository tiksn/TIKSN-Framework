using System;
using System.Net.Mail;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Licensing;
using Xunit;
using static LanguageExt.Prelude;

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

        var licenseFactory = serviceProvider.GetRequiredService<ILicenseFactory<TestEntitlements, TestLicenseEntitlements>>();

        var serialNumber = Ulid.NewUlid();
        var licensor = new IndividualParty(
            "Tigran",
            "Torosyan",
            "Tigran TIKSN Torosyan",
            new MailAddress("me@me.me"),
            new Uri("https://tiksn.com/"));
        var licensee = new OrganizationParty(
            "Microsoft Corporation",
            "Microsoft",
            new MailAddress("info@microsoft.com"),
            new Uri("https://www.microsoft.com/"));
        var notBefore = new DateTimeOffset(2022, 8, 24, 0, 0, 0, TimeSpan.Zero);
        var notAfter = new DateTimeOffset(2023, 8, 24, 0, 0, 0, TimeSpan.Zero);

        var terms = new LicenseTerms(
            serialNumber,
            licensor,
            licensee,
            notBefore,
            notAfter);

        var entitlements = new TestEntitlements(
            "Test-Name",
            100,
            Seq<byte>(1, 2, 3, 4, 5, 6, 7, 8, 9, 0),
            1000,
            10002000);

        licenseFactory.Create(terms, entitlements);
        new TestLicenseEntitlements();
    }
}
