using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TIKSN.Licensing;

public class EdDSACertificateSignatureService : ICertificateSignatureService
{
    public byte[] Sign(
        byte[] data,
        X509Certificate2 privateCertificate)
    {
        using var privateKey = privateCertificate.GetECDsaPrivateKey();
        return privateKey.SignData(
            data, HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
    }

    public bool Verify(
        byte[] data,
        byte[] signature,
        X509Certificate2 publicCertificate)
    {
        using var publicKey = publicCertificate.GetECDsaPublicKey();
        return publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
    }
}
