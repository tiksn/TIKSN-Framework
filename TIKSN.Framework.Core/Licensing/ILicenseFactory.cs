using System.Security.Cryptography.X509Certificates;
using Google.Protobuf;
using LanguageExt;
using LanguageExt.Common;

namespace TIKSN.Licensing;

public interface ILicenseFactory<TEntitlements, TEntitlementsData> where TEntitlementsData : IMessage<TEntitlementsData>
{
    Validation<Error, License<TEntitlements>> Create(
        LicenseTerms terms,
        TEntitlements entitlements,
        X509Certificate2 privateCertificate);

    Validation<Error, License<TEntitlements>> Create(
        Seq<byte> data,
        X509Certificate2 publicCertificate);
}
