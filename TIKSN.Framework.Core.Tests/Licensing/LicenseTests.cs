using System;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.DependencyInjection;
using TIKSN.Licensing;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Framework.Core.Tests.Licensing;

public class LicenseTests
{
    [Fact]
    public void GivenRSAPrivateCertificate_WhenCertificateCreated_ThenItShouldBeValid()
    {
        GivenPrivateCertificate_WhenCertificateCreated_ThenItShouldBeValid(
            LicensingResource.LicensingTest1Public,
            LicensingResource.LicensingTest1Private_pfx,
            LicensingResource.LicensingTest1PrivatePassword);
    }

    private void GivenPrivateCertificate_WhenCertificateCreated_ThenItShouldBeValid(
        byte[] licensingTestPublic,
        byte[] licensingTestPrivate_pfx,
        string licensingTestPrivatePassword)
    {
        // Arrange

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

        var privateCertificate = new X509Certificate2(
            licensingTestPrivate_pfx,
            licensingTestPrivatePassword);

        // Act

        var result = licenseFactory.Create(terms, entitlements, privateCertificate);

        // Assert

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.SuccessToSeq().Single().Terms.Should().NotBeNull();
        _ = result.SuccessToSeq().Single().Terms.SerialNumber.Should().Be(terms.SerialNumber);
        _ = result.SuccessToSeq().Single().Terms.Licensor.Should().BeOfType<IndividualParty>();
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().FirstName.Should().Be(licensor.FirstName);
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().LastName.Should().Be(licensor.LastName);
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().FullName.Should().Be(licensor.FullName);
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().Email.Address.Should().Be(licensor.Email.Address);
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().Website.Should().Be(licensor.Website);
        _ = result.SuccessToSeq().Single().Terms.Licensee.Should().BeOfType<OrganizationParty>();
        _ = result.SuccessToSeq().Single().Terms.Licensee.As<OrganizationParty>().LongName.Should().Be(licensee.LongName);
        _ = result.SuccessToSeq().Single().Terms.Licensee.As<OrganizationParty>().ShortName.Should().Be(licensee.ShortName);
        _ = result.SuccessToSeq().Single().Terms.Licensee.As<OrganizationParty>().Email.Address.Should().Be(licensee.Email.Address);
        _ = result.SuccessToSeq().Single().Terms.Licensee.As<OrganizationParty>().Website.Should().Be(licensee.Website);
        _ = result.SuccessToSeq().Single().Terms.NotBefore.Should().Be(terms.NotBefore);
        _ = result.SuccessToSeq().Single().Terms.NotAfter.Should().Be(terms.NotAfter);
        _ = result.SuccessToSeq().Single().Data.Should().NotBeEmpty();

        // Arrange

        var publicCertificate = new X509Certificate2(licensingTestPublic);

        // Act

        result = licenseFactory.Create(result.SuccessToSeq().Single().Data, publicCertificate);

        // Assert

        _ = result.IsSuccess.Should().BeTrue();
        _ = result.SuccessToSeq().Single().Terms.Should().NotBeNull();
        _ = result.SuccessToSeq().Single().Terms.SerialNumber.Should().Be(terms.SerialNumber);
        _ = result.SuccessToSeq().Single().Terms.Licensor.Should().BeOfType<IndividualParty>();
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().FirstName.Should().Be(licensor.FirstName);
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().LastName.Should().Be(licensor.LastName);
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().FullName.Should().Be(licensor.FullName);
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().Email.Address.Should().Be(licensor.Email.Address);
        _ = result.SuccessToSeq().Single().Terms.Licensor.As<IndividualParty>().Website.Should().Be(licensor.Website);
        _ = result.SuccessToSeq().Single().Terms.Licensee.Should().BeOfType<OrganizationParty>();
        _ = result.SuccessToSeq().Single().Terms.Licensee.As<OrganizationParty>().LongName.Should().Be(licensee.LongName);
        _ = result.SuccessToSeq().Single().Terms.Licensee.As<OrganizationParty>().ShortName.Should().Be(licensee.ShortName);
        _ = result.SuccessToSeq().Single().Terms.Licensee.As<OrganizationParty>().Email.Address.Should().Be(licensee.Email.Address);
        _ = result.SuccessToSeq().Single().Terms.Licensee.As<OrganizationParty>().Website.Should().Be(licensee.Website);
        _ = result.SuccessToSeq().Single().Terms.NotBefore.Should().Be(terms.NotBefore);
        _ = result.SuccessToSeq().Single().Terms.NotAfter.Should().Be(terms.NotAfter);
        _ = result.SuccessToSeq().Single().Data.Should().NotBeEmpty();
    }
}
