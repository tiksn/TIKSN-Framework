using LanguageExt;
using OpenFeature.Model;
using TIKSN.Identity;
using TIKSN.Licensing;

namespace TIKSN.FeatureManagement;

public sealed class TenantLicenseEntitlementsFeatureProvider<TTenantIdentity, TEntitlements>
    : LicenseEntitlementsFeatureProviderBase<TEntitlements>
{
    private const string ProviderName = "TIKSN.TenantLicenseEntitlementsFeatureProvider";

#pragma warning disable S2743 // Static fields should not be used in generic types
    private static readonly Metadata ProviderMetadata = new(ProviderName);
#pragma warning restore S2743 // Static fields should not be used in generic types

    private readonly ITenantIdProvider<TTenantIdentity> tenantIdProvider;
    private readonly ITenantLicenseProvider<TTenantIdentity, TEntitlements> tenantLicenseProvider;

    public TenantLicenseEntitlementsFeatureProvider(
        ITenantLicenseProvider<TTenantIdentity, TEntitlements> tenantLicenseProvider,
        ITenantIdProvider<TTenantIdentity> tenantIdProvider,
        ILicenseFeatureResolver<TEntitlements> licenseFeatureResolver,
        TimeProvider timeProvider)
        : base(licenseFeatureResolver, timeProvider)
    {
        this.tenantLicenseProvider =
            tenantLicenseProvider ?? throw new ArgumentNullException(nameof(tenantLicenseProvider));
        this.tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
    }

    public override Metadata GetMetadata() => ProviderMetadata;

    protected override Task<Option<License<TEntitlements>>> ResolveLicenseAsync(
        EvaluationContext? context,
        CancellationToken cancellationToken)
    {
        var tenantIdOption = this.tenantIdProvider.FindTenantId() ||
            TryExtractIdentityFromContext<TTenantIdentity>(context, FeatureEvaluationContextKeys.TenantId);

        return tenantIdOption.Match(
            tenantId => this.tenantLicenseProvider.GetLicenseAsync(tenantId, cancellationToken),
            () => Task.FromResult(Option<License<TEntitlements>>.None));
    }
}
