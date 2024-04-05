using System.Security.Cryptography.X509Certificates;
using LanguageExt;
using LanguageExt.Common;

namespace TIKSN.Licensing;

#pragma warning disable S2326 // Unused type parameters should be removed

public interface ILicenseFactory<TEntitlements, TEntitlementsData>
#pragma warning restore S2326 // Unused type parameters should be removed
{
    Validation<Error, License<TEntitlements>> Create(
        LicenseTerms terms,
        TEntitlements entitlements,
        X509Certificate2 privateCertificate);

    Validation<Error, License<TEntitlements>> Create(
        Seq<byte> data,
        X509Certificate2 publicCertificate);
}
