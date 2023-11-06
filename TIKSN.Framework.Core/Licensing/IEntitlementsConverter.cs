using LanguageExt;
using LanguageExt.Common;

namespace TIKSN.Licensing;

public interface IEntitlementsConverter<TEntitlements, TEntitlementsData>
{
    Validation<Error, TEntitlementsData> Convert(TEntitlements entitlements);

    Validation<Error, TEntitlements> Convert(TEntitlementsData entitlementsData);
}
