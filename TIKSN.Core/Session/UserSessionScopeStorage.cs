using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace TIKSN.Session
{
	public class UserSessionScopeStorage<TIdentity> : IUserSessionScopeStorage<TIdentity> where TIdentity : IEquatable<TIdentity>
	{
		private readonly ConcurrentDictionary<TIdentity, IServiceScope> _scopes;
		private readonly IServiceProvider _serviceProvider;

		public UserSessionScopeStorage(IServiceProvider serviceProvider)
		{
			_scopes = new ConcurrentDictionary<TIdentity, IServiceScope>();
			_serviceProvider = serviceProvider;
		}

		public IServiceProvider GetOrAddServiceProvider(TIdentity id)
		{
			return _scopes.GetOrAdd(id, key => _serviceProvider.CreateScope()).ServiceProvider;
		}

		public bool TryRemoveServiceProvider(TIdentity id)
		{
			var removed = _scopes.TryRemove(id, out IServiceScope removedScope);

			if (removed)
				removedScope.Dispose();

			return removed;
		}
	}
}
