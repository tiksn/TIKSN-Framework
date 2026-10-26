using LanguageExt;
using OpenFeature.Model;
using TIKSN.Licensing;

namespace TIKSN.FeatureManagement;

public sealed class SystemLicenseEntitlementsFeatureProvider<TEntitlements>
    : LicenseEntitlementsFeatureProviderBase<TEntitlements>
{
    private const string ProviderName = "TIKSN.SystemLicenseEntitlementsFeatureProvider";

#pragma warning disable S2743 // Static fields should not be used in generic types
    private static readonly Metadata ProviderMetadata = new(ProviderName);
#pragma warning restore S2743 // Static fields should not be used in generic types

    private readonly ISystemLicenseProvider<TEntitlements> systemLicenseProvider;

    public SystemLicenseEntitlementsFeatureProvider(
        ISystemLicenseProvider<TEntitlements> systemLicenseProvider,
        ILicenseFeatureResolver<TEntitlements> licenseFeatureResolver,
        TimeProvider timeProvider)
        : base(licenseFeatureResolver, timeProvider) =>
        this.systemLicenseProvider =
            systemLicenseProvider ?? throw new ArgumentNullException(nameof(systemLicenseProvider));

    public override Metadata GetMetadata() => ProviderMetadata;

    protected override Task<Option<License<TEntitlements>>> ResolveLicenseAsync(
        EvaluationContext? context,
        CancellationToken cancellationToken) =>
        this.systemLicenseProvider.GetLicenseAsync(cancellationToken);
}
