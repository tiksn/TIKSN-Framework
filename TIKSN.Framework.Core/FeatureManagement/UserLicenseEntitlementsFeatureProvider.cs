using LanguageExt;
using OpenFeature.Model;
using TIKSN.Identity;
using TIKSN.Licensing;

namespace TIKSN.FeatureManagement;

public sealed class UserLicenseEntitlementsFeatureProvider<TUserIdentity, TEntitlements>
    : LicenseEntitlementsFeatureProviderBase<TEntitlements>
{
    private const string ProviderName = "TIKSN.UserLicenseEntitlementsFeatureProvider";

#pragma warning disable S2743 // Static fields should not be used in generic types
    private static readonly Metadata ProviderMetadata = new(ProviderName);
#pragma warning restore S2743 // Static fields should not be used in generic types

    private readonly IUserIdProvider<TUserIdentity> userIdProvider;
    private readonly IUserLicenseProvider<TUserIdentity, TEntitlements> userLicenseProvider;

    public UserLicenseEntitlementsFeatureProvider(
        IUserLicenseProvider<TUserIdentity, TEntitlements> userLicenseProvider,
        IUserIdProvider<TUserIdentity> userIdProvider,
        ILicenseFeatureResolver<TEntitlements> licenseFeatureResolver,
        TimeProvider timeProvider)
        : base(licenseFeatureResolver, timeProvider)
    {
        this.userLicenseProvider =
            userLicenseProvider ?? throw new ArgumentNullException(nameof(userLicenseProvider));
        this.userIdProvider = userIdProvider ?? throw new ArgumentNullException(nameof(userIdProvider));
    }

    public override Metadata GetMetadata() => ProviderMetadata;

    protected override Task<Option<License<TEntitlements>>> ResolveLicenseAsync(
        EvaluationContext? context,
        CancellationToken cancellationToken)
    {
        var userIdOption = this.userIdProvider.FindUserId() ||
            TryExtractIdentityFromContext<TUserIdentity>(context, FeatureEvaluationContextKeys.UserId);

        return userIdOption.Match(
            userId => this.userLicenseProvider.GetLicenseAsync(userId, cancellationToken),
            () => Task.FromResult(Option<License<TEntitlements>>.None));
    }
}
