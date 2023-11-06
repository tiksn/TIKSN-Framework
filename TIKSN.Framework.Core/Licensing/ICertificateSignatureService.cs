using System.Security.Cryptography.X509Certificates;

namespace TIKSN.Licensing;

public interface ICertificateSignatureService
{
    byte[] Sign(byte[] data, X509Certificate2 privateCertificate);

    bool Verify(byte[] data, byte[] signature, X509Certificate2 publicCertificate);
}
