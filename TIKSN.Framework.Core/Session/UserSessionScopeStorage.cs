using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Session;

public class UserSessionScopeStorage<TIdentity> : IUserSessionScopeStorage<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ConcurrentDictionary<TIdentity, IServiceScope> _scopes;
    private readonly IServiceProvider _serviceProvider;

    public UserSessionScopeStorage(IServiceProvider serviceProvider)
    {
        this._scopes = new ConcurrentDictionary<TIdentity, IServiceScope>();
        this._serviceProvider = serviceProvider;
    }

    public IServiceProvider GetOrAddServiceProvider(TIdentity id) =>
        this._scopes.GetOrAdd(id, key => this._serviceProvider.CreateScope()).ServiceProvider;

    public bool TryRemoveServiceProvider(TIdentity id)
    {
        var removed = this._scopes.TryRemove(id, out var removedScope);

        if (removed)
        {
            removedScope.Dispose();
        }

        return removed;
    }
}
