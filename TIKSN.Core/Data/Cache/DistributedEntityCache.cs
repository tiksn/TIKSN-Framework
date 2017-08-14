using System;
using System.Threading.Tasks;

namespace TIKSN.Data.Cache
{
	public class DistributedEntityCache : IDistributedEntityCache
	{
		public TEntity Get<TEntity, TIdentity>(TIdentity identity)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>
		{
			throw new NotImplementedException();
		}

		public Task<TEntity> GetAsync<TEntity, TIdentity>(TIdentity identity)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>
		{
			throw new NotImplementedException();
		}

		public void Set<TEntity, TIdentity>(TIdentity identity, TEntity entity)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>
		{
			throw new NotImplementedException();
		}

		public Task SetAsync<TEntity, TIdentity>(TIdentity identity, TEntity entity)
			where TEntity : IEntity<TIdentity>
			where TIdentity : IEquatable<TIdentity>
		{
			throw new NotImplementedException();
		}
	}
}
