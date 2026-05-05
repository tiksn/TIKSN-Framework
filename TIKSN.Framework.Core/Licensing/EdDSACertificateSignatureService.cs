using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace TIKSN.Licensing;

public class EdDSACertificateSignatureService : ICertificateSignatureService
{
    public byte[] Sign(
        byte[] data,
        X509Certificate2 privateCertificate)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(privateCertificate);

        if (privateCertificate is not EdDSAX509Certificate2 edDsaCertificate)
        {
            throw new InvalidOperationException(
                "EdDSA private key is not accessible from this X509Certificate2 instance. Use EdDSAX509Certificate2.");
        }

        if (edDsaCertificate.EdDsaPrivateKey is not Ed25519PrivateKeyParameters privateKey)
        {
            throw new InvalidOperationException("Certificate private key is not an Ed25519 private key.");
        }

        var signer = SignerUtilities.GetSigner("Ed25519");
        signer.Init(forSigning: true, privateKey);
        signer.BlockUpdate(data, inOff: 0, data.Length);
        return signer.GenerateSignature();
    }

    public bool Verify(
        byte[] data,
        byte[] signature,
        X509Certificate2 publicCertificate)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(signature);
        ArgumentNullException.ThrowIfNull(publicCertificate);

        var certificate = new X509CertificateParser().ReadCertificate(publicCertificate.RawData);

        if (certificate.GetPublicKey() is not Ed25519PublicKeyParameters publicKey)
        {
            throw new InvalidOperationException("Certificate public key is not an Ed25519 public key.");
        }

        var verifier = SignerUtilities.GetSigner("Ed25519");
        verifier.Init(forSigning: false, publicKey);
        verifier.BlockUpdate(data, inOff: 0, data.Length);
        return verifier.VerifySignature(signature);
    }
}
