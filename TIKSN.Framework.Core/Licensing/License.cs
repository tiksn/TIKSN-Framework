using LanguageExt;

namespace TIKSN.Licensing;

public class License<TEntitlements>
{
    internal License(
        LicenseTerms terms,
        TEntitlements entitlements,
        Seq<byte> data)
    {
        this.Terms = terms ?? throw new ArgumentNullException(nameof(terms));
        this.Entitlements = entitlements;
        this.Data = data;
    }

    public Seq<byte> Data { get; }
    public TEntitlements Entitlements { get; }
    public LicenseTerms Terms { get; }
}
