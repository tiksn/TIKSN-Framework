using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Google.Protobuf;
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

        this.publicCertificates = new Dictionary<string, byte[]>
        {
            { "RSA", LicensingResource.LicensingTest1Public },
            { "DSA", LicensingResource.LicensingTest2Public },
            { "EdDSA", LicensingResource.LicensingTest3Public },
        };

        this.privateCertificates = new Dictionary<string, byte[]>
        {
            { "RSA", LicensingResource.LicensingTest1Private_pfx },
            { "DSA", LicensingResource.LicensingTest2Private_pfx },
            { "EdDSA", LicensingResource.LicensingTest3Private_pfx },
        };

        this.privateCertificatePasswords = new Dictionary<string, string>
        {
            { "RSA", LicensingResource.LicensingTest1PrivatePassword },
            { "DSA", LicensingResource.LicensingTest2PrivatePassword },
            { "EdDSA", LicensingResource.LicensingTest3PrivatePassword },
        };
    }

    [Fact]
    public void GivenEmptyEnvelope_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out _,
            out _,
            out _,
            out _,
            out var publicCertificate,
            out _);

        // Act

        var result = licenseFactory.Create(ToSeq(new LicenseEnvelope()), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenExpiredLicense_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out var licensor,
            out var licensee,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var expiredTerms = new LicenseTerms(
            terms.SerialNumber,
            licensor,
            licensee,
            new DateTimeOffset(year: 2021, month: 8, day: 24, hour: 0, minute: 0, second: 0, TimeSpan.Zero),
            new DateTimeOffset(year: 2021, month: 9, day: 24, hour: 0, minute: 0, second: 0, TimeSpan.Zero));
        var license = licenseFactory.Create(expiredTerms, entitlements, privateCertificate).SuccessToSeq().Single();

        // Act

        var result = licenseFactory.Create(license.Data, publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenInvalidPartyEmail_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        const string kind = "RSA";
        this.Arrange(
            kind,
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var envelope = CreateEnvelope(licenseFactory, terms, entitlements, privateCertificate);
        envelope.Message.Licensor.Email = "not an email address";

        // Act

        var result = licenseFactory.Create(Resign(envelope, kind, privateCertificate), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenInvalidPartyWebsite_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        const string kind = "RSA";
        this.Arrange(
            kind,
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var envelope = CreateEnvelope(licenseFactory, terms, entitlements, privateCertificate);
        envelope.Message.Licensee.Website = "ftp://www.microsoft.com/";

        // Act

        var result = licenseFactory.Create(Resign(envelope, kind, privateCertificate), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenInvalidSerialNumber_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        const string kind = "RSA";
        this.Arrange(
            kind,
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var envelope = CreateEnvelope(licenseFactory, terms, entitlements, privateCertificate);
        envelope.Message.SerialNumber = ByteString.CopyFrom([1, 2, 3]);

        // Act

        var result = licenseFactory.Create(Resign(envelope, kind, privateCertificate), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenMalformedData_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out _,
            out _,
            out _,
            out _,
            out var publicCertificate,
            out _);

        // Act

        var result = licenseFactory.Create(new[]
        {
            (byte)0xff
        }.ToSeq(), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenMalformedEntitlements_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        const string kind = "RSA";
        this.Arrange(
            kind,
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var envelope = CreateEnvelope(licenseFactory, terms, entitlements, privateCertificate);
        envelope.Message.Entitlements = ByteString.CopyFrom([0xff]);

        // Act

        var result = licenseFactory.Create(Resign(envelope, kind, privateCertificate), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenMissingEntitlements_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var envelope = CreateEnvelope(licenseFactory, terms, entitlements, privateCertificate);
        envelope.Message.Entitlements = ByteString.Empty;

        // Act

        var result = licenseFactory.Create(ToSeq(envelope), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenMissingSignature_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var envelope = CreateEnvelope(licenseFactory, terms, entitlements, privateCertificate);
        envelope.Signature = ByteString.Empty;

        // Act

        var result = licenseFactory.Create(ToSeq(envelope), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenNotYetValidLicense_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out var licensor,
            out var licensee,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var futureTerms = new LicenseTerms(
            terms.SerialNumber,
            licensor,
            licensee,
            new DateTimeOffset(year: 2023, month: 8, day: 24, hour: 0, minute: 0, second: 0, TimeSpan.Zero),
            new DateTimeOffset(year: 2024, month: 8, day: 24, hour: 0, minute: 0, second: 0, TimeSpan.Zero));
        var license = licenseFactory.Create(futureTerms, entitlements, privateCertificate).SuccessToSeq().Single();

        // Act

        var result = licenseFactory.Create(license.Data, publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Theory]
    [InlineData("RSA")]
    [InlineData("DSA")]
    [InlineData("EdDSA")]
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
            .Validate(license => license.Entitlements.CompanyId == 2000, failCode: 1638412088, "Wrong Company ID")
            .Validate(license => license.Entitlements.EmployeeId == 20004000, failCode: 102340273, "Wrong Employee ID");

        // Assert

        result.IsFail.ShouldBeTrue();
    }

    [Theory]
    [InlineData("RSA")]
    [InlineData("DSA")]
    [InlineData("EdDSA")]
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

    [Fact]
    public void GivenTamperedDates_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var envelope = CreateEnvelope(licenseFactory, terms, entitlements, privateCertificate);
        envelope.Message.NotAfter += TimeSpan.FromDays(1).Ticks;

        // Act

        var result = licenseFactory.Create(ToSeq(envelope), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenTamperedEntitlements_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        var envelope = CreateEnvelope(licenseFactory, terms, entitlements, privateCertificate);
        envelope.Message.Entitlements = ByteString.CopyFromUtf8("tampered");

        // Act

        var result = licenseFactory.Create(ToSeq(envelope), publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenWrongCertificate_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out _,
            out var privateCertificate);
        this.Arrange(
            "DSA",
            out _,
            out _,
            out _,
            out _,
            out _,
            out var wrongPublicCertificate,
            out _);
        var license = licenseFactory.Create(terms, entitlements, privateCertificate).SuccessToSeq().Single();

        // Act

        var result = licenseFactory.Create(license.Data, wrongPublicCertificate);

        // Assert

        this.AssertFail(result);
    }

    [Fact]
    public void GivenWrongDescriptor_WhenLicenseCreated_ThenItShouldFail()
    {
        // Arrange

        this.Arrange(
            "RSA",
            out var licenseFactory,
            out _,
            out _,
            out var terms,
            out var entitlements,
            out var publicCertificate,
            out var privateCertificate);
        this.Arrange(
            "RSA",
            out var otherLicenseFactory,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            descriptor: Guid.Parse("7587ba79-b0b9-427e-8dc6-3d7528682386"));
        var license = licenseFactory.Create(terms, entitlements, privateCertificate).SuccessToSeq().Single();

        // Act

        var result = otherLicenseFactory.Create(license.Data, publicCertificate);

        // Assert

        this.AssertFail(result);
    }

    private static Seq<byte> ToSeq(
        IMessage message) => message.ToByteArray().ToSeq();

    private void Arrange(
        string kind,
        out ILicenseFactory<TestEntitlements, TestLicenseEntitlements> licenseFactory,
        out IndividualParty licensor,
        out OrganizationParty licensee,
        out LicenseTerms terms,
        out TestEntitlements entitlements,
        out X509Certificate2 publicCertificate,
        out X509Certificate2 privateCertificate,
        Guid? descriptor = null)
    {
        var services = new ServiceCollection();
        _ = services
            .AddSingleton<IEntitlementsConverter<TestEntitlements, TestLicenseEntitlements>,
                TestEntitlementsConverter>();
        _ = services.AddFrameworkCore();
        _ = services.AddSingleton<ILicenseDescriptor<TestEntitlements>>(
            new LicenseDescriptor<TestEntitlements>(
                "Test License",
                descriptor ?? Guid.Parse("20559a6a-2ea6-45b2-9dc7-928406bc1719")));

        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(year: 2022, month: 9, day: 24, hour: 0,
            minute: 0, second: 0, TimeSpan.Zero));
        _ = services.AddSingleton<TimeProvider>(fakeTimeProvider);

        _ = services.AddSingleton(fakeTimeProvider);
        ContainerBuilder containerBuilder = new();
        _ = containerBuilder.RegisterModule<CoreModule>();
        containerBuilder.Populate(services);
        var serviceProvider = new AutofacServiceProvider(containerBuilder.Build());

        licenseFactory =
            serviceProvider.GetRequiredService<ILicenseFactory<TestEntitlements, TestLicenseEntitlements>>();
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
        var notBefore = new DateTimeOffset(year: 2022, month: 8, day: 24, hour: 0, minute: 0, second: 0, TimeSpan.Zero);
        var notAfter = new DateTimeOffset(year: 2023, month: 8, day: 24, hour: 0, minute: 0, second: 0, TimeSpan.Zero);

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
            Quantity: 100,
            Seq<byte>(a: 1, b: 2, c: 3, d: 4, e: 5, f: 6, g: 7, h: 8, 9, 0),
            CompanyId: 1000,
            EmployeeId: 10002000);
        this.testOutputHelper.WriteLine("License Entitlements:");
        this.testOutputHelper.WriteLine(JsonConvert.SerializeObject(entitlements, Formatting.Indented));

        publicCertificate = new X509Certificate2(this.publicCertificates[kind]);
        this.testOutputHelper.WriteLine(
            $"Public Certificate Serial Number: {publicCertificate.GetSerialNumberString()}");

        privateCertificate = kind == "EdDSA"
            ? new EdDSAX509Certificate2(
                this.privateCertificates[kind],
                this.privateCertificatePasswords[kind])
            : new X509Certificate2(
                this.privateCertificates[kind],
                this.privateCertificatePasswords[kind]);

        this.testOutputHelper.WriteLine(
            $"Private Certificate Serial Number: {privateCertificate.GetSerialNumberString()}");
    }

    private void AssertFail(
        Validation<Error, License<TestEntitlements>> result)
    {
        foreach (var resultSuccess in result.SuccessToSeq())
        {
            this.testOutputHelper.WriteLine($"License Result Success: {resultSuccess}");
        }

        result.IsFail.ShouldBeTrue();
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
        ((OrganizationParty)result.SuccessToSeq().Single().Terms.Licensee).Email.Address.ShouldBe(
            licensee.Email.Address);
        ((OrganizationParty)result.SuccessToSeq().Single().Terms.Licensee).Website.ShouldBe(licensee.Website);
        result.SuccessToSeq().Single().Terms.NotBefore.ShouldBe(terms.NotBefore);
        result.SuccessToSeq().Single().Terms.NotAfter.ShouldBe(terms.NotAfter);
        result.SuccessToSeq().Single().Data.ShouldNotBeEmpty();
        this.testOutputHelper.WriteLine($"License Data Length: {result.SuccessToSeq().Single().Data.Length}");
    }

    private static LicenseEnvelope CreateEnvelope(
        ILicenseFactory<TestEntitlements, TestLicenseEntitlements> licenseFactory,
        LicenseTerms terms,
        TestEntitlements entitlements,
        X509Certificate2 privateCertificate)
    {
        var license = licenseFactory.Create(terms, entitlements, privateCertificate).SuccessToSeq().Single();
        return LicenseEnvelope.Parser.ParseFrom([.. license.Data]);
    }

    private static Seq<byte> Resign(
        LicenseEnvelope envelope,
        string kind,
        X509Certificate2 privateCertificate)
    {
        ICertificateSignatureService signatureService = kind switch
        {
            "RSA" => new RSACertificateSignatureService(),
            "DSA" => new DSACertificateSignatureService(),
            "EdDSA" => new EdDSACertificateSignatureService(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, message: null),
        };

        envelope.SignatureAlgorithm = privateCertificate.GetKeyAlgorithm();
        envelope.Signature =
            ByteString.CopyFrom(signatureService.Sign(envelope.Message.ToByteArray(), privateCertificate));
        return ToSeq(envelope);
    }
}
