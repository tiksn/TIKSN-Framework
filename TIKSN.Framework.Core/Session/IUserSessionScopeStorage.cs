namespace TIKSN.Session;

public interface IUserSessionScopeStorage<TIdentity> where TIdentity : IEquatable<TIdentity>
{
    public IServiceProvider GetOrAddServiceProvider(TIdentity id);

    public ValueTask<bool> TryRemoveServiceProviderAsync(TIdentity id);
}
