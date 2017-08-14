using System;
using System.Threading.Tasks;

namespace TIKSN.Data.Cache
{
	public interface IDistributedEntityCache
	{
		TEntity Get<TEntity, TIdentity>(TIdentity identity) where TEntity : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>;
		Task<TEntity> GetAsync<TEntity, TIdentity>(TIdentity identity) where TEntity : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>;
		void Set<TEntity, TIdentity>(TIdentity identity, TEntity entity) where TEntity : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>;
		Task SetAsync<TEntity, TIdentity>(TIdentity identity, TEntity entity) where TEntity : IEntity<TIdentity> where TIdentity : IEquatable<TIdentity>;
	}
}