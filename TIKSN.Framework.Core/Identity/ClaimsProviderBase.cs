using System.Security.Claims;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Identity;

public abstract class ClaimsProviderBase
{
    protected static T GetFound<T>(Option<T> found, string claimType) => found.IfNone(() =>
        throw new ClaimNotFoundException($"Claim '{claimType}' not found.", claimType));

    protected Option<string> FindFirstClaimValue(string type) => this.GetClaimsPrincipal()
        .Bind(principal => Optional(principal.FindFirst(type)))
        .Map(claim => claim.Value);

    protected Option<T> FindFirstClaimValue<T>(
        string type,
        Func<string, T> parser)
    {
        ArgumentNullException.ThrowIfNull(parser);

        var rawValue = this.FindFirstClaimValue(type);

        return rawValue.Map(parser);
    }

    protected abstract Option<ClaimsPrincipal> GetClaimsPrincipal();
}
