using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using TIKSN.DependencyInjection;
using TIKSN.Licensing;
using TIKSN.Time;
using Xunit;
using Xunit.Abstractions;
using static LanguageExt.Prelude;

namespace TIKSN.Framework.Core.Tests.Licensing;

public class LicenseTests
{
    private readonly IReadOnlyDictionary<string, string> privateCertificatePasswords;
    private readonly IReadOnlyDictionary<string, byte[]> privateCertificates;
    private readonly IReadOnlyDictionary<string, byte[]> publicCertificates;
    private readonly ITestOutputHelper testOutputHelper;

    public LicenseTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));

        this.publicCertificates = new Dictionary<string, byte[]>()
        {
            { "RSA", LicensingResource.LicensingTest1Public }
        };

        this.privateCertificates = new Dictionary<string, byte[]>()
        {
            { "RSA", LicensingResource.LicensingTest1Private_pfx }
        };

        this.privateCertificatePasswords = new Dictionary<string, string>()
        {
            { "RSA", LicensingResource.LicensingTest1PrivatePassword }
        };
    }

    [Theory]
    [InlineData("RSA")]
    public void GivenPrivateCertificate_WhenLicenseCreated_ThenItShouldBeValid(
        string kind)
    {
        // Arrange

        ILicenseFactory<TestEntitlements, TestLicenseEntitlements> licenseFactory;
        IndividualParty licensor;
        OrganizationParty licensee;
        LicenseTerms terms;
        TestEntitlements entitlements;
        X509Certificate2 publicCertificate;
        X509Certificate2 privateCertificate;
        this.Arrange(
            kind,
            out licenseFactory,
            out licensor,
            out licensee,
            out terms,
            out entitlements,
            out publicCertificate,
            out privateCertificate);

        // Act

        var result = licenseFactory.Create(terms, entitlements, privateCertificate);

        // Assert

        this.AssertSuccess(licensor, licensee, terms, result);

        // Act

        result = licenseFactory.Create(result.SuccessToSeq().Single().Data, publicCertificate);

        // Assert

        this.AssertSuccess(licensor, licensee, terms, result);
    }

    [Theory]
    [InlineData("RSA")]
    public void GivenPrivateCertificate_WhenLicenseCreatedWithValidation_ThenItShouldBeInvalid(
        string kind)
    {
        // Arrange

        ILicenseFactory<TestEntitlements, TestLicenseEntitlements> licenseFactory;
        IndividualParty licensor;
        OrganizationParty licensee;
        LicenseTerms terms;
        TestEntitlements entitlements;
        X509Certificate2 publicCertificate;
        X509Certificate2 privateCertificate;
        this.Arrange(
            kind,
            out licenseFactory,
            out licensor,
            out licensee,
            out terms,
            out entitlements,
            out publicCertificate,
            out privateCertificate);

        // Act

        var result = licenseFactory.Create(terms, entitlements, privateCertificate)
            .Validate(license => license.Entitlements.CompanyId == 2000, 1638412088, "Wrong Company ID")
            .Validate(license => license.Entitlements.EmployeeId == 20004000, 102340273, "Wrong Employee ID");

        // Assert

        _ = result.IsFail.Should().BeTrue();
    }

    private void Arrange(
        string kind,
        out ILicenseFactory<TestEntitlements, TestLicenseEntitlements> licenseFactory,
        out IndividualParty licensor,
        out OrganizationParty licensee,
        out LicenseTerms terms,
        out TestEntitlements entitlements,
        out X509Certificate2 publicCertificate,
        out X509Certificate2 privateCertificate)
    {
        var services = new ServiceCollection();
        _ = services.AddSingleton<IEntitlementsConverter<TestEntitlements, TestLicenseEntitlements>, TestEntitlementsConverter>();
        _ = services.AddFrameworkPlatform();

        var fakeTimeProvider = Substitute.For<ITimeProvider>();
        _ = fakeTimeProvider.GetCurrentTime()
            .Returns(new DateTimeOffset(2022, 9, 24, 0, 0, 0, TimeSpan.Zero));

        _ = services.AddSingleton(fakeTimeProvider);
        ContainerBuilder containerBuilder = new();
        _ = containerBuilder.RegisterModule<CoreModule>();
        containerBuilder.Populate(services);
        var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());

        licenseFactory = serviceProvider.GetRequiredService<ILicenseFactory<TestEntitlements, TestLicenseEntitlements>>();
        var serialNumber = Ulid.NewUlid();
        licensor = new IndividualParty(
            "Tigran",
            "Torosyan",
            "Tigran TIKSN Torosyan",
            new MailAddress("me@me.me"),
            new Uri("https://tiksn.com/"));
        licensee = new OrganizationParty(
            "Microsoft Corporation",
            "Microsoft",
            new MailAddress("info@microsoft.com"),
            new Uri("https://www.microsoft.com/"));
        var notBefore = new DateTimeOffset(2022, 8, 24, 0, 0, 0, TimeSpan.Zero);
        var notAfter = new DateTimeOffset(2023, 8, 24, 0, 0, 0, TimeSpan.Zero);

        terms = new LicenseTerms(
            serialNumber,
            licensor,
            licensee,
            notBefore,
            notAfter);
        this.testOutputHelper.WriteLine("License Terms:");
        this.testOutputHelper.WriteLine(JsonConvert.SerializeObject(terms, Formatting.Indented));
        entitlements = new TestEntitlements(
            "Test-Name",
            100,
            Seq<byte>(1, 2, 3, 4, 5, 6, 7, 8, 9, 0),
            1000,
            10002000);
        this.testOutputHelper.WriteLine("License Entitlements:");
        this.testOutputHelper.WriteLine(JsonConvert.SerializeObject(entitlements, Formatting.Indented));
        publicCertificate = new X509Certificate2(this.publicCertificates[kind]);
        privateCertificate = new X509Certificate2(
            this.privateCertificates[kind],
            this.privateCertificatePasswords[kind]);
    }

    private void AssertSuccess(
        IndividualParty licensor,
        OrganizationParty licensee,
        LicenseTerms terms,
        Validation<Error, License<TestEntitlements>> result)
    {
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
        this.testOutputHelper.WriteLine($"License Data Length: {result.SuccessToSeq().Single().Data.Length}");
    }
}
