using System;
using System.Security.Claims;
using LanguageExt;
using Shouldly;
using TIKSN.Identity;
using Xunit;

namespace TIKSN.Tests.Identity;

public class ClaimsProviderBaseTests
{
    [Fact]
    public void GivenMissingTenantIdClaim_WhenFindTenantIdGuid_ThenReturnsNone()
    {
        var principal = SetupClaimsPrincipal();
        ITenantIdProvider<Guid> provider = new TestClaimsProvider(principal);

        var result = provider.FindTenantId();

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenMissingTenantIdClaim_WhenGetTenantIdGuid_ThenThrowsClaimNotFoundException()
    {
        var principal = SetupClaimsPrincipal();
        ITenantIdProvider<Guid> provider = new TestClaimsProvider(principal);

        Action action = () => provider.GetTenantId();

        var exception = action.ShouldThrow<ClaimNotFoundException>();
        exception.ClaimType.ShouldBe("http://schemas.microsoft.com/identity/claims/tenantid");
        exception.Message.ShouldBe("Claim 'http://schemas.microsoft.com/identity/claims/tenantid' not found.");
    }

    [Fact]
    public void GivenNameIdentifierClaim_WhenFindUserIdInt_ThenReturnsOptionWithInt()
    {
        var userId = 12345;
        var principal = SetupClaimsPrincipal(ClaimTypes.NameIdentifier,
            userId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        IUserIdProvider<int> provider = new TestClaimsProvider(principal);

        var result = provider.FindUserId();

        result.IsSome.ShouldBeTrue();
        result.IfNone(0).ShouldBe(userId);
    }

    [Fact]
    public void GivenTenantIdClaim_WhenFindTenantIdGuid_ThenReturnsOptionWithGuid()
    {
        var tenantId = Guid.NewGuid();
        var principal =
            SetupClaimsPrincipal("http://schemas.microsoft.com/identity/claims/tenantid", tenantId.ToString());
        ITenantIdProvider<Guid> provider = new TestClaimsProvider(principal);

        var result = provider.FindTenantId();

        result.IsSome.ShouldBeTrue();
        result.IfNone(Guid.Empty).ShouldBe(tenantId);
    }

    private static ClaimsPrincipal SetupClaimsPrincipal(string? claimType = null, string? claimValue = null)
    {
        var user = new ClaimsPrincipal();

        if (claimType != null && claimValue != null)
        {
            var identity = new ClaimsIdentity(
            [
                new Claim(claimType, claimValue)
            ]);
            user.AddIdentity(identity);
        }

        return user;
    }

    private sealed class TestClaimsProvider : ClaimsProviderBase, ITenantIdProvider<Guid>, IUserIdProvider<int>
    {
        private readonly Option<ClaimsPrincipal> _principal;

        public TestClaimsProvider(Option<ClaimsPrincipal> principal) => this._principal = principal;

        public Option<Guid> FindTenantId() => this.FindFirstClaimValue("http://schemas.microsoft.com/identity/claims/tenantid", Guid.Parse);

        public Option<int> FindUserId() => this.FindFirstClaimValue(ClaimTypes.NameIdentifier, int.Parse);

        public Guid GetTenantId() => GetFound(this.FindTenantId(), "http://schemas.microsoft.com/identity/claims/tenantid");

        public int GetUserId() => GetFound(this.FindUserId(), ClaimTypes.NameIdentifier);

        protected override Option<ClaimsPrincipal> GetClaimsPrincipal() => this._principal;
    }
}
