using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Google.Protobuf;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using static LanguageExt.Prelude;

namespace TIKSN.Licensing;

public class LicenseFactory<TEntitlements, TEntitlementsData> : ILicenseFactory<TEntitlements, TEntitlementsData>
    where TEntitlementsData : IMessage<TEntitlementsData>
{
    private readonly IEntitlementsConverter<TEntitlements, TEntitlementsData> entitlementsConverter;
    private readonly ILicenseDescriptor<TEntitlements> licenseDescriptor;
    private readonly IServiceProvider serviceProvider;
    private readonly TimeProvider timeProvider;

    public LicenseFactory(
        ILicenseDescriptor<TEntitlements> licenseDescriptor,
        IEntitlementsConverter<TEntitlements, TEntitlementsData> entitlementsConverter,
        TimeProvider timeProvider,
        IServiceProvider serviceProvider)
    {
        this.licenseDescriptor = licenseDescriptor ?? throw new ArgumentNullException(nameof(licenseDescriptor));
        this.entitlementsConverter =
            entitlementsConverter ?? throw new ArgumentNullException(nameof(entitlementsConverter));
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
                succ => envelope.Message.Entitlements = succ.ToByteString(),
                fail => errors.AddRange(fail));

        if (!privateCertificate.HasPrivateKey)
        {
            errors.Add(Error.New(124921383, "Certificate Private Key is missing"));
        }

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        var messageData = envelope.Message.ToByteString();

        var keyAlgorithm = privateCertificate.GetKeyAlgorithm();
        var signatureServiceValidation = this.GetSignatureService(keyAlgorithm);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        envelope.SignatureAlgorithm = keyAlgorithm;
        if (signatureServiceValidation.IsFail)
        {
            return signatureServiceValidation.FailToSeq();
        }

        try
        {
            var signatureService = signatureServiceValidation.SuccessToSeq().Single();
            var signature = signatureService.Sign(messageData.ToByteArray(), privateCertificate);
            envelope.Signature = ByteString.CopyFrom(signature);
        }
        catch (CryptographicException)
        {
            return Error.New(75510623, "License signing failed");
        }
        catch (InvalidOperationException)
        {
            return Error.New(75510930, "License signing failed");
        }
        catch (NotSupportedException)
        {
            return Error.New(75510931, "License signing failed");
        }

        var envelopeData = envelope.ToByteString();

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

        var envelopeValidation = ParseEnvelope(data);

        if (envelopeValidation.IsFail)
        {
            return envelopeValidation.FailToSeq();
        }

        var envelope = envelopeValidation.SuccessToSeq().Single();
        var envelopeShapeValidation = ValidateEnvelopeShape(envelope);

        if (envelopeShapeValidation.IsFail)
        {
            return envelopeShapeValidation.FailToSeq();
        }

        var keyAlgorithm = publicCertificate.GetKeyAlgorithm();
        var signatureSuiteValidation = ValidateVerificationSignatureAlgorithm(envelope, keyAlgorithm);

        if (signatureSuiteValidation.IsFail)
        {
            return signatureSuiteValidation.FailToSeq();
        }

        var signatureServiceValidation = this.GetSignatureService(keyAlgorithm);

        if (signatureServiceValidation.IsFail)
        {
            return signatureServiceValidation.FailToSeq();
        }

        var signatureService = signatureServiceValidation.SuccessToSeq().Single();

        var messageData = envelope.Message.ToByteArray();

        bool isSignatureValid;

        try
        {
            isSignatureValid =
                signatureService.Verify(messageData, envelope.Signature.ToByteArray(), publicCertificate);
        }
        catch (CryptographicException)
        {
            isSignatureValid = false;
        }
        catch (InvalidOperationException)
        {
            isSignatureValid = false;
        }
        catch (NotSupportedException)
        {
            isSignatureValid = false;
        }

        if (!isSignatureValid)
        {
            errors.Add(Error.New(75511191, "License signature is invalid"));
        }

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        var now = this.timeProvider.GetUtcNow();
        var licenseTermsValidation = this.GetTerms(envelope, now);
        var entitlementsValidation = this.GetEntitlements(envelope);

        return licenseTermsValidation
            .Bind(terms => entitlementsValidation.Map(entitlements => (terms, entitlements)))
            .Map(x => new License<TEntitlements>(
                x.terms,
                x.entitlements,
                data));
    }

    private static Validation<Error, LicenseEnvelope> ParseEnvelope(
        Seq<byte> data)
    {
        try
        {
            return LicenseEnvelope.Parser.ParseFrom([.. data]);
        }
        catch (InvalidProtocolBufferException)
        {
            return Error.New(75510624, "License data is malformed");
        }
    }

    private static Validation<Error, Unit> ValidateEnvelopeShape(
        LicenseEnvelope envelope)
    {
        var errors = new List<Error>();

        if (envelope.Message is null)
        {
            errors.Add(Error.New(75510625, "License message is missing"));
            return errors.ToSeq();
        }

        if (string.IsNullOrWhiteSpace(envelope.SignatureAlgorithm))
        {
            errors.Add(Error.New(75510626, "License signature algorithm is missing"));
        }

        if (envelope.Signature is null || envelope.Signature.Length == 0)
        {
            errors.Add(Error.New(75510627, "License signature is missing"));
        }

        if (envelope.Message.Discriminator is null || envelope.Message.Discriminator.Length == 0)
        {
            errors.Add(Error.New(75510628, "License descriptor is missing"));
        }

        if (envelope.Message.SerialNumber is null || envelope.Message.SerialNumber.Length == 0)
        {
            errors.Add(Error.New(75510629, "License serial number is missing"));
        }

        if (envelope.Message.Licensor is null)
        {
            errors.Add(Error.New(75510630, "License licensor is missing"));
        }

        if (envelope.Message.Licensee is null)
        {
            errors.Add(Error.New(75510631, "License licensee is missing"));
        }

        if (!envelope.Message.HasEntitlements || envelope.Message.Entitlements.Length == 0)
        {
            errors.Add(Error.New(75510632, "License entitlements are missing"));
        }

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return unit;
    }

    private static Validation<Error, Unit> ValidateVerificationSignatureAlgorithm(
        LicenseEnvelope envelope,
        string keyAlgorithm)
    {
        if (!string.Equals(envelope.SignatureAlgorithm, keyAlgorithm, StringComparison.Ordinal))
        {
            return Error.New(75511259,
                "License Signature Algorithm is not matching with Certificate Signature Algorithm");
        }

        return unit;
    }

    private Validation<Error, TEntitlements> GetEntitlements(
        LicenseEnvelope envelope)
    {
        try
        {
            var entitlementsData = Activator.CreateInstance<TEntitlementsData>();
            entitlementsData.MergeFrom(envelope.Message.Entitlements.CreateCodedInput());
            return this.entitlementsConverter.Convert(entitlementsData);
        }
        catch (InvalidProtocolBufferException)
        {
            return Error.New(75510633, "License entitlements are malformed");
        }
        catch (InvalidOperationException)
        {
            return Error.New(75510654, "License entitlements are invalid");
        }
    }

    private Validation<Error, ICertificateSignatureService> GetSignatureService(
        string keyAlgorithm)
    {
        try
        {
            return Success<Error, ICertificateSignatureService>(
                this.serviceProvider.GetRequiredKeyedService<ICertificateSignatureService>(keyAlgorithm));
        }
        catch (InvalidOperationException)
        {
            return Error.New(75510655, "Certificate signature algorithm is not registered");
        }
    }

    private Validation<Error, LicenseTerms> GetTerms(
        LicenseEnvelope envelope,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        if (envelope.Message.Discriminator.Length != Guid.Empty.ToByteArray().Length ||
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
            if (now < notBeforeValue)
            {
                return Error.New(75511224, "License is not valid yet");
            }

            return Success<Error, DateTimeOffset>(notBeforeValue);
        });

        notAfterValidation = notAfterValidation.Bind(notAfterValue =>
        {
            if (notAfterValue < now)
            {
                return Error.New(75511242, "License is not valid anymore");
            }

            return Success<Error, DateTimeOffset>(notAfterValue);
        });

        return serialNumberValidation
            .Bind(serialNumber => licensorValidation.Map(licensor => (serialNumber, licensor)))
            .Bind(x => licenseeValidation.Map(licensee => (x.serialNumber, x.licensor, licensee)))
            .Bind(x => notBeforeValidation.Map(notBefore => (x.serialNumber, x.licensor, x.licensee, notBefore)))
            .Bind(x => notAfterValidation.Map(notAfter =>
                (x.serialNumber, x.licensor, x.licensee, x.notBefore, notAfter)))
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

        if (this.licenseDescriptor.Discriminator == Guid.Empty)
        {
            errors.Add(Error.New(13127674, "'Discriminator' should not be empty GUID"));
        }

        envelope.Message = new LicenseMessage
        {
            Discriminator = ByteString.CopyFrom(this.licenseDescriptor.Discriminator.ToByteArray()),
        };

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
