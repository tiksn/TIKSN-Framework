using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TIKSN.Licensing;

#pragma warning disable S101 // Types should be named in PascalCase

public class DSACertificateSignatureService : ICertificateSignatureService
#pragma warning restore S101 // Types should be named in PascalCase
{
    public byte[] Sign(
        byte[] data,
        X509Certificate2 privateCertificate)
    {
        using var privateKey =
            privateCertificate.GetDSAPrivateKey()
            ?? throw new InvalidOperationException("Public Certificate is missing");

        return privateKey.SignData(
            data, HashAlgorithmName.SHA1, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
    }

    public bool Verify(
        byte[] data,
        byte[] signature,
        X509Certificate2 publicCertificate)
    {
        using var publicKey =
            publicCertificate.GetDSAPublicKey()
            ?? throw new InvalidOperationException("Public Certificate is missing");

        return publicKey.VerifyData(data, signature, HashAlgorithmName.SHA1, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
    }
}
