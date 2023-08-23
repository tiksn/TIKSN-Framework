using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Autofac.Features.Indexed;
using LanguageExt;
using LanguageExt.Common;
using TIKSN.Serialization.Bond;

namespace TIKSN.Licensing;

public class LicenseFactory<TEntitlements, TEntitlementsData> : ILicenseFactory<TEntitlements, TEntitlementsData>
{
    private readonly IIndex<string, ICertificateSignatureService> certificateSignatureService;
    private readonly CompactBinaryBondDeserializer deserializer;
    private readonly IEntitlementsConverter<TEntitlements, TEntitlementsData> entitlementsConverter;
    private readonly CompactBinaryBondSerializer serializer;

    public LicenseFactory(
        IIndex<string, ICertificateSignatureService> certificateSignatureService,
        CompactBinaryBondSerializer serializer,
        CompactBinaryBondDeserializer deserializer,
        IEntitlementsConverter<TEntitlements, TEntitlementsData> entitlementsConverter)
    {
        this.certificateSignatureService = certificateSignatureService ?? throw new ArgumentNullException(nameof(certificateSignatureService));
        this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        this.entitlementsConverter = entitlementsConverter ?? throw new ArgumentNullException(nameof(entitlementsConverter));
    }

    public Validation<Error, License<TEntitlements>> Create(
        LicenseTerms terms,
        TEntitlements entitlements,
        X509Certificate2 privateCertificate)
    {
        if (terms is null)
        {
            throw new ArgumentNullException(nameof(terms));
        }

        if (entitlements is null)
        {
            throw new ArgumentNullException(nameof(entitlements));
        }

        if (privateCertificate is null)
        {
            throw new ArgumentNullException(nameof(privateCertificate));
        }

        var errors = new List<Error>();
        var envelope = new LicenseEnvelope();

        _ = Populate(envelope, terms)
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

        if (errors.Any())
        {
            return errors.ToSeq();
        }

        var messageData = this.serializer.Serialize(envelope.Message);

        var keyAlgorithm = privateCertificate.GetKeyAlgorithm();
        envelope.SignatureAlgorithm = keyAlgorithm;
        var signatureService = this.certificateSignatureService[keyAlgorithm];
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
        if (publicCertificate is null)
        {
            throw new ArgumentNullException(nameof(publicCertificate));
        }

        var errors = new List<Error>();

        if (data.IsEmpty)
        {
            errors.Add(Error.New(1393113914, "License data is empty"));
            return errors.ToSeq();
        }

        var envelope = this.deserializer.Deserialize<LicenseEnvelope>(data.ToArray());

        LicenseTerms terms = null;
        TEntitlements entitlements = default;

        _ = GetTerms(envelope)
            .Match(succ => terms = succ, fail => errors.AddRange(fail));

        _ = this.entitlementsConverter.Convert(this.deserializer.Deserialize<TEntitlementsData>(envelope.Message.Entitlements.ToArray()))
            .Match(succ => entitlements = succ, fail => errors.AddRange(fail));

        var keyAlgorithm = publicCertificate.GetKeyAlgorithm();

        if (!string.Equals(envelope.SignatureAlgorithm, keyAlgorithm, StringComparison.Ordinal))
        {
            errors.Add(Error.New(429095162, "License Signature Algorithm is not matching with Certificate Signature Algorithm"));
        }

        var signatureService = this.certificateSignatureService[keyAlgorithm];

        var messageData = this.serializer.Serialize(envelope.Message);

        if (!signatureService.Verify(messageData, envelope.Signature.ToArray(), publicCertificate))
        {
            errors.Add(Error.New(1311896038, "License signature is invalid"));
        }

        if (errors.Any())
        {
            return errors.ToSeq();
        }

        return new License<TEntitlements>(
            terms,
            entitlements,
            data);
    }

    private static Validation<Error, LicenseTerms> GetTerms(
        LicenseEnvelope envelope)
    {
        if (envelope is null)
        {
            throw new ArgumentNullException(nameof(envelope));
        }

        var errors = new List<Error>();

        if (envelope.Message.VersionNumber != 1)
        {
            errors.Add(Error.New(2129266854, "License version is invalid"));
        }

        var serialNumber = Ulid.Empty;
        var notBefore = DateTimeOffset.MinValue;
        var notAfter = DateTimeOffset.MinValue;
        Party licensor = null;
        Party licensee = null;

        _ = LicenseFactoryTermsValidation.ConvertToUlid(envelope.Message.SerialNumber)
            .Match(succ => serialNumber = succ, fail => errors.AddRange(fail));
        _ = LicenseFactoryTermsValidation.ConvertToDate(envelope.Message.NotBefore)
            .Match(succ => notBefore = succ, fail => errors.AddRange(fail));
        _ = LicenseFactoryTermsValidation.ConvertToDate(envelope.Message.NotAfter)
            .Match(succ => notAfter = succ, fail => errors.AddRange(fail));
        _ = LicenseFactoryTermsValidation.ConvertToParty(envelope.Message.Licensor)
            .Match(succ => licensor = succ, fail => errors.AddRange(fail));
        _ = LicenseFactoryTermsValidation.ConvertToParty(envelope.Message.Licensee)
            .Match(succ => licensee = succ, fail => errors.AddRange(fail));

        if (errors.Any())
        {
            return errors.ToSeq();
        }

        return new LicenseTerms(
            serialNumber,
            licensor,
            licensee,
            notBefore,
            notAfter);
    }

    private static Validation<Error, LicenseEnvelope> Populate(
        LicenseEnvelope envelope,
        LicenseTerms terms)
    {
        if (envelope is null)
        {
            throw new ArgumentNullException(nameof(envelope));
        }

        if (terms is null)
        {
            throw new ArgumentNullException(nameof(terms));
        }

        var errors = new List<Error>();

        envelope.Message.VersionNumber = 1;

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

        if (errors.Any())
        {
            return errors.ToSeq();
        }

        return envelope;
    }
}
