using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.EntityFrameworkCore
{
    public class EntityQueryRepository<TContext, TEntity, TIdentity> : EntityRepository<TContext, TEntity>, IQueryRepository<TEntity, TIdentity>
        where TContext : DbContext
        where TEntity : class, IEntity<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {
        public EntityQueryRepository(TContext dbContext) : base(dbContext)
        {
        }

        protected IQueryable<TEntity> Entities => dbContext.Set<TEntity>().AsNoTracking();

        public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken)
        {
            return Entities.SingleAsync(entity => entity.ID.Equals(id), cancellationToken);
        }

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken = default)
        {
            return Entities.SingleOrDefaultAsync(entity => entity.ID.Equals(id), cancellationToken);
        }
    }
}