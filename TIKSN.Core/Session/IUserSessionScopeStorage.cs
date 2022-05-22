using System;

namespace TIKSN.Session
{
    public interface IUserSessionScopeStorage<TIdentity> where TIdentity : IEquatable<TIdentity>
    {
        IServiceProvider GetOrAddServiceProvider(TIdentity id);

        bool TryRemoveServiceProvider(TIdentity id);
    }
}
