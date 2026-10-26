using LanguageExt;

namespace TIKSN.Licensing;

public interface ISystemLicenseProvider<TEntitlements>
{
    public Task<Option<License<TEntitlements>>> GetLicenseAsync(CancellationToken cancellationToken);
}
