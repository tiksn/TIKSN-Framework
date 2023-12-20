namespace TIKSN.Session;

public interface IUserSessionScopeStorage<TIdentity> where TIdentity : IEquatable<TIdentity>
{
    IServiceProvider GetOrAddServiceProvider(TIdentity id);

    ValueTask<bool> TryRemoveServiceProviderAsync(TIdentity id);
}
