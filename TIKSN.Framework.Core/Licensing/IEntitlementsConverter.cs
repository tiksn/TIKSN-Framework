using LanguageExt;
using LanguageExt.Common;

namespace TIKSN.Licensing;

public interface IEntitlementsConverter<TEntitlements, TEntitlementsData>
{
    public Validation<Error, TEntitlementsData> Convert(TEntitlements entitlements);

    public Validation<Error, TEntitlements> Convert(TEntitlementsData entitlementsData);
}
