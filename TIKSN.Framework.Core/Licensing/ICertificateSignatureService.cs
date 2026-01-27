using System.Security.Cryptography.X509Certificates;

namespace TIKSN.Licensing;

public interface ICertificateSignatureService
{
    public byte[] Sign(byte[] data, X509Certificate2 privateCertificate);

    public bool Verify(byte[] data, byte[] signature, X509Certificate2 publicCertificate);
}
