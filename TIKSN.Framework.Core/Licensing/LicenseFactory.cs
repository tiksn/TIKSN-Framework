using System.Security.Cryptography.X509Certificates;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Serialization.Bond;
using static LanguageExt.Prelude;

namespace TIKSN.Licensing;

public class LicenseFactory<TEntitlements, TEntitlementsData> : ILicenseFactory<TEntitlements, TEntitlementsData>
{
    private readonly CompactBinaryBondDeserializer deserializer;
    private readonly IEntitlementsConverter<TEntitlements, TEntitlementsData> entitlementsConverter;
    private readonly ILicenseDescriptor<TEntitlements> licenseDescriptor;
    private readonly CompactBinaryBondSerializer serializer;
    private readonly IServiceProvider serviceProvider;
    private readonly TimeProvider timeProvider;

    public LicenseFactory(
        CompactBinaryBondSerializer serializer,
        CompactBinaryBondDeserializer deserializer,
        ILicenseDescriptor<TEntitlements> licenseDescriptor,
        IEntitlementsConverter<TEntitlements, TEntitlementsData> entitlementsConverter,
        TimeProvider timeProvider,
        IServiceProvider serviceProvider)
    {
        this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        this.licenseDescriptor = licenseDescriptor ?? throw new ArgumentNullException(nameof(licenseDescriptor));
        this.entitlementsConverter = entitlementsConverter ?? throw new ArgumentNullException(nameof(entitlementsConverter));
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Validation<Error, License<TEntitlements>> Create(
        LicenseTerms terms,
        TEntitlements entitlements,
        X509Certificate2 privateCertificate)
    {
        ArgumentNullException.ThrowIfNull(terms);
        ArgumentNullException.ThrowIfNull(entitlements);
        ArgumentNullException.ThrowIfNull(privateCertificate);

        var errors = new List<Error>();
        var envelope = new LicenseEnvelope();

        _ = this.Populate(envelope, terms)
            .Match(succ => envelope = succ, fail => errors.AddRange(fail));

        _ = this.entitlementsConverter.Convert(entitlements)
            .Match(
                succ => envelope.Message.Entitlements =
                    new ArraySegment<byte>(this.serializer.Serialize(succ)),
                fail => errors.AddRange(fail));

        if (!privateCertificate.HasPrivateKey)
        {
            errors.Add(Error.New(124921383, "Certificate Private Key is missing"));
        }

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        var messageData = this.serializer.Serialize(envelope.Message);

        var keyAlgorithm = privateCertificate.GetKeyAlgorithm();
        envelope.SignatureAlgorithm = keyAlgorithm;
        var signatureService = this.serviceProvider.GetRequiredKeyedService<ICertificateSignatureService>(keyAlgorithm);
        var signature = signatureService.Sign(messageData, privateCertificate);
        envelope.Signature = new ArraySegment<byte>(signature);

        var envelopeData = this.serializer.Serialize(envelope);

        return new License<TEntitlements>(
            terms,
            entitlements,
            envelopeData.ToSeq());
    }

    public Validation<Error, License<TEntitlements>> Create(
        Seq<byte> data,
        X509Certificate2 publicCertificate)
    {
        ArgumentNullException.ThrowIfNull(publicCertificate);

        var errors = new List<Error>();

        if (data.IsEmpty)
        {
            errors.Add(Error.New(1393113914, "License data is empty"));
            return errors.ToSeq();
        }

        var envelope = this.deserializer.Deserialize<LicenseEnvelope>([.. data]);

        var licenseTermsValidation = this.GetTerms(envelope, this.timeProvider);
        var entitlementsValidation = this.entitlementsConverter.Convert(this.deserializer.Deserialize<TEntitlementsData>([.. envelope.Message.Entitlements]));

        var keyAlgorithm = publicCertificate.GetKeyAlgorithm();

        if (!string.Equals(envelope.SignatureAlgorithm, keyAlgorithm, StringComparison.Ordinal))
        {
            errors.Add(Error.New(429095162, "License Signature Algorithm is not matching with Certificate Signature Algorithm"));
        }

        var signatureService = this.serviceProvider.GetRequiredKeyedService<ICertificateSignatureService>(keyAlgorithm);

        var messageData = this.serializer.Serialize(envelope.Message);

        if (!signatureService.Verify(messageData, [.. envelope.Signature], publicCertificate))
        {
            errors.Add(Error.New(1311896038, "License signature is invalid"));
        }

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return licenseTermsValidation
            .Bind(terms => entitlementsValidation.Map(entitlements => (terms, entitlements)))
            .Map(x => new License<TEntitlements>(
                x.terms,
                x.entitlements,
                data));
    }

    private Validation<Error, LicenseTerms> GetTerms(
        LicenseEnvelope envelope,
        TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        if (envelope.Message.VersionNumber != 1)
        {
            return Error.New(2129266854, "License version is invalid");
        }

        if (envelope.Message.Discriminator.Count != Guid.Empty.ToByteArray().Length ||
            envelope.Message.Discriminator.All(x => x == byte.MinValue) ||
            !envelope.Message.Discriminator.SequenceEqual(this.licenseDescriptor.Discriminator.ToByteArray()))
        {
            return Error.New(13127362, "License Descriptor is invalid");
        }

        var serialNumberValidation = LicenseFactoryTermsValidation.ConvertToUlid(envelope.Message.SerialNumber);
        var notBeforeValidation = LicenseFactoryTermsValidation.ConvertToDate(envelope.Message.NotBefore);
        var notAfterValidation = LicenseFactoryTermsValidation.ConvertToDate(envelope.Message.NotAfter);
        var licensorValidation = LicenseFactoryTermsValidation.ConvertToParty(envelope.Message.Licensor);
        var licenseeValidation = LicenseFactoryTermsValidation.ConvertToParty(envelope.Message.Licensee);

        notBeforeValidation = notBeforeValidation.Bind(notBeforeValue =>
        {
            if (timeProvider.GetUtcNow() < notBeforeValue)
            {
                return Error.New(504364885, "License is not valid yet");
            }

            return Success<Error, DateTimeOffset>(notBeforeValue);
        });

        notAfterValidation = notAfterValidation.Bind(notAfterValue =>
        {
            if (notAfterValue < timeProvider.GetUtcNow())
            {
                return Error.New(428925195, "License is not valid anymore");
            }

            return Success<Error, DateTimeOffset>(notAfterValue);
        });

        return serialNumberValidation
            .Bind(serialNumber => licensorValidation.Map(licensor => (serialNumber, licensor)))
            .Bind(x => licenseeValidation.Map(licensee => (x.serialNumber, x.licensor, licensee)))
            .Bind(x => notBeforeValidation.Map(notBefore => (x.serialNumber, x.licensor, x.licensee, notBefore)))
            .Bind(x => notAfterValidation.Map(notAfter => (x.serialNumber, x.licensor, x.licensee, x.notBefore, notAfter)))
            .Map(x => new LicenseTerms(
                x.serialNumber,
                x.licensor,
                x.licensee,
                x.notBefore,
                x.notAfter));
    }

    private Validation<Error, LicenseEnvelope> Populate(
        LicenseEnvelope envelope,
        LicenseTerms terms)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ArgumentNullException.ThrowIfNull(terms);

        var errors = new List<Error>();

        envelope.Message.VersionNumber = 1;

        if (this.licenseDescriptor.Discriminator == Guid.Empty)
        {
            errors.Add(Error.New(13127674, "'Discriminator' should not be empty GUID"));
        }

        envelope.Message.Discriminator = new ArraySegment<byte>(this.licenseDescriptor.Discriminator.ToByteArray());

        if (terms.NotAfter < terms.NotBefore)
        {
            errors.Add(Error.New(318567863, "'NotBefore' should be not later than 'NotAfter'"));
        }

        _ = LicenseFactoryTermsValidation.ConvertFromUlid(terms.SerialNumber)
            .Match(succ => envelope.Message.SerialNumber = succ, fail => errors.AddRange(fail));
        _ = LicenseFactoryTermsValidation.ConvertFromDate(terms.NotBefore)
            .Match(succ => envelope.Message.NotBefore = succ, fail => errors.AddRange(fail));
        _ = LicenseFactoryTermsValidation.ConvertFromDate(terms.NotAfter)
            .Match(succ => envelope.Message.NotAfter = succ, fail => errors.AddRange(fail));
        _ = LicenseFactoryTermsValidation.ConvertFromParty(terms.Licensor)
            .Match(succ => envelope.Message.Licensor = succ, fail => errors.AddRange(fail));
        _ = LicenseFactoryTermsValidation.ConvertFromParty(terms.Licensee)
            .Match(succ => envelope.Message.Licensee = succ, fail => errors.AddRange(fail));

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return envelope;
    }
}
