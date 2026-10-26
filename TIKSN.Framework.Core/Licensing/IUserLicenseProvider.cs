using LanguageExt;

namespace TIKSN.Licensing;

public interface IUserLicenseProvider<in TUserIdentity, TEntitlements>
{
    public Task<Option<License<TEntitlements>>> GetLicenseAsync(
        TUserIdentity userId,
        CancellationToken cancellationToken);
}
