using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Newtonsoft.Json;
using Shouldly;
using TIKSN.DependencyInjection;
using TIKSN.Licensing;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Licensing;

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
            { "RSA", LicensingResource.LicensingTest1Public },
            { "DSA", LicensingResource.LicensingTest2Public },
        };

        this.privateCertificates = new Dictionary<string, byte[]>()
        {
            { "RSA", LicensingResource.LicensingTest1Private_pfx },
            { "DSA", LicensingResource.LicensingTest2Private_pfx },
        };

        this.privateCertificatePasswords = new Dictionary<string, string>()
        {
            { "RSA", LicensingResource.LicensingTest1PrivatePassword },
            { "DSA", LicensingResource.LicensingTest2PrivatePassword },
        };
    }

    [Theory]
    [InlineData("RSA")]
    [InlineData("DSA")]
    public void GivenPrivateCertificate_WhenLicenseCreated_ThenItShouldBeValid(
        string kind)
    {
        // Arrange

        this.Arrange(
            kind,
            out var licenseFactory,
            out var licensor,
            out var licensee,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);

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
    [InlineData("DSA")]
    public void GivenPrivateCertificate_WhenLicenseCreatedWithValidation_ThenItShouldBeInvalid(
        string kind)
    {
        // Arrange

        this.Arrange(
            kind,
            out var licenseFactory,
            out var licensor,
            out var licensee,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);

        // Act

        var result = licenseFactory.Create(terms, entitlements, privateCertificate)
            .Validate(license => license.Entitlements.CompanyId == 2000, 1638412088, "Wrong Company ID")
            .Validate(license => license.Entitlements.EmployeeId == 20004000, 102340273, "Wrong Employee ID");

        // Assert

        result.IsFail.ShouldBeTrue();
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
        _ = services.AddFrameworkCore();
        _ = services.AddSingleton<ILicenseDescriptor<TestEntitlements>>(
            new LicenseDescriptor<TestEntitlements>(
                "Test License",
                Guid.Parse("20559a6a-2ea6-45b2-9dc7-928406bc1719")));

        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2022, 9, 24, 0, 0, 0, TimeSpan.Zero));
        _ = services.AddSingleton<TimeProvider>(fakeTimeProvider);

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
        this.testOutputHelper.WriteLine($"Public Certificate Serial Number: {publicCertificate.GetSerialNumberString()}");

        privateCertificate = new X509Certificate2(
            this.privateCertificates[kind],
            this.privateCertificatePasswords[kind]);

        this.testOutputHelper.WriteLine($"Private Certificate Serial Number: {privateCertificate.GetSerialNumberString()}");
    }

    private void AssertSuccess(
        IndividualParty licensor,
        OrganizationParty licensee,
        LicenseTerms terms,
        Validation<Error, License<TestEntitlements>> result)
    {
        foreach (var resultFail in result.FailToSeq())
        {
            this.testOutputHelper.WriteLine($"License Result Fail: {resultFail}");
        }
        result.IsSuccess.ShouldBeTrue();
        _ = result.SuccessToSeq().Single().Terms.ShouldNotBeNull();
        result.SuccessToSeq().Single().Terms.SerialNumber.ShouldBe(terms.SerialNumber);
        _ = result.SuccessToSeq().Single().Terms.Licensor.ShouldBeOfType<IndividualParty>();
        ((IndividualParty)result.SuccessToSeq().Single().Terms.Licensor).FirstName.ShouldBe(licensor.FirstName);
        ((IndividualParty)result.SuccessToSeq().Single().Terms.Licensor).LastName.ShouldBe(licensor.LastName);
        ((IndividualParty)result.SuccessToSeq().Single().Terms.Licensor).FullName.ShouldBe(licensor.FullName);
        ((IndividualParty)result.SuccessToSeq().Single().Terms.Licensor).Email.Address.ShouldBe(licensor.Email.Address);
        ((IndividualParty)result.SuccessToSeq().Single().Terms.Licensor).Website.ShouldBe(licensor.Website);
        _ = result.SuccessToSeq().Single().Terms.Licensee.ShouldBeOfType<OrganizationParty>();
        ((OrganizationParty)result.SuccessToSeq().Single().Terms.Licensee).LongName.ShouldBe(licensee.LongName);
        ((OrganizationParty)result.SuccessToSeq().Single().Terms.Licensee).ShortName.ShouldBe(licensee.ShortName);
        ((OrganizationParty)result.SuccessToSeq().Single().Terms.Licensee).Email.Address.ShouldBe(licensee.Email.Address);
        ((OrganizationParty)result.SuccessToSeq().Single().Terms.Licensee).Website.ShouldBe(licensee.Website);
        result.SuccessToSeq().Single().Terms.NotBefore.ShouldBe(terms.NotBefore);
        result.SuccessToSeq().Single().Terms.NotAfter.ShouldBe(terms.NotAfter);
        result.SuccessToSeq().Single().Data.ShouldNotBeEmpty();
        this.testOutputHelper.WriteLine($"License Data Length: {result.SuccessToSeq().Single().Data.Length}");
    }
}
