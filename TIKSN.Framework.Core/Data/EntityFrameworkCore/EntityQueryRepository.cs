using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TIKSN.Data.EntityFrameworkCore
{
    public class EntityQueryRepository<TContext, TEntity, TIdentity> : EntityRepository<TContext, TEntity>,
        IQueryRepository<TEntity, TIdentity>, IStreamRepository<TEntity>
        where TContext : DbContext
        where TEntity : class, IEntity<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
    {
        public EntityQueryRepository(TContext dbContext) : base(dbContext)
        {
        }

        protected IQueryable<TEntity> Entities => this.dbContext.Set<TEntity>().AsNoTracking();

        public Task<bool> ExistsAsync(TIdentity id, CancellationToken cancellationToken) =>
            this.Entities.AnyAsync(a => a.ID.Equals(id), cancellationToken);

        public Task<TEntity> GetAsync(TIdentity id, CancellationToken cancellationToken) =>
            this.Entities.SingleAsync(entity => entity.ID.Equals(id), cancellationToken);

        public Task<TEntity> GetOrDefaultAsync(TIdentity id, CancellationToken cancellationToken) =>
            this.Entities.SingleOrDefaultAsync(entity => entity.ID.Equals(id), cancellationToken);

        public async Task<IEnumerable<TEntity>> ListAsync(IEnumerable<TIdentity> ids,
            CancellationToken cancellationToken)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            return await this.Entities.Where(entity => ids.Contains(entity.ID)).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async IAsyncEnumerable<TEntity> StreamAllAsync(CancellationToken cancellationToken)
        {
            await foreach (var entity in this.Entities.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return entity;
            }
        }
    }
}
