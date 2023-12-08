using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Session;

public class UserSessionScopeStorage<TIdentity> : IUserSessionScopeStorage<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ConcurrentDictionary<TIdentity, AsyncServiceScope> scopes;
    private readonly IServiceProvider serviceProvider;

    public UserSessionScopeStorage(IServiceProvider serviceProvider)
    {
        this.scopes = new ConcurrentDictionary<TIdentity, AsyncServiceScope>();
        this.serviceProvider = serviceProvider;
    }

    public IServiceProvider GetOrAddServiceProvider(TIdentity id) =>
        this.scopes.GetOrAdd(id, _ => this.serviceProvider.CreateAsyncScope()).ServiceProvider;

    public async ValueTask<bool> TryRemoveServiceProviderAsync(TIdentity id)
    {
        var removed = this.scopes.TryRemove(id, out var removedScope);

        if (removed)
        {
            await removedScope.DisposeAsync().ConfigureAwait(false);
        }

        return removed;
    }
}
