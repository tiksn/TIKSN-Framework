using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;

namespace TIKSN.Licensing;

#pragma warning disable S101 // Types should be named in PascalCase

public class EdDSAX509Certificate2 : X509Certificate2
#pragma warning restore S101 // Types should be named in PascalCase
{
#pragma warning disable SYSLIB0057 // Loading certificate data through the constructor is obsolete
    public EdDSAX509Certificate2(
        byte[] rawData,
        string? password)
        : base(rawData, password) => this.EdDsaPrivateKey = LoadPrivateKey(rawData, password);
#pragma warning restore SYSLIB0057 // Loading certificate data through the constructor is obsolete

    internal AsymmetricKeyParameter EdDsaPrivateKey { get; }

    private static AsymmetricKeyParameter LoadPrivateKey(
        byte[] rawData,
        string? password)
    {
        ArgumentNullException.ThrowIfNull(rawData);

        using var stream = new MemoryStream(rawData);
        var store = new Pkcs12StoreBuilder().Build();
        store.Load(stream, password?.ToCharArray() ?? []);

        foreach (var alias in store.Aliases)
        {
            if (!store.IsKeyEntry(alias))
            {
                continue;
            }

            var key = store.GetKey(alias).Key;

            if (key is Ed25519PrivateKeyParameters)
            {
                return key;
            }
        }

        throw new InvalidOperationException("PKCS#12 data does not contain an Ed25519 private key.");
    }
}
