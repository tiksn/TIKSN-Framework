using LanguageExt;

namespace TIKSN.Licensing;

public interface ITenantLicenseProvider<in TTenantIdentity, TEntitlements>
{
    public Task<Option<License<TEntitlements>>> GetLicenseAsync(
        TTenantIdentity tenantId,
        CancellationToken cancellationToken);
}
