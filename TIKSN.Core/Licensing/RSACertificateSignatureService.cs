using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TIKSN.Licensing;

public class RSACertificateSignatureService : ICertificateSignatureService
{
    public byte[] Sign(
        byte[] data,
        X509Certificate2 privateCertificate)
    {
        using var privateKey = privateCertificate.GetRSAPrivateKey();
        return privateKey.SignData(
            data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public bool Verify(
        byte[] data,
        byte[] signature,
        X509Certificate2 publicCertificate)
    {
        using var publicKey = publicCertificate.GetRSAPublicKey();
        return publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}