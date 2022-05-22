using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TIKSN.Data.EntityFrameworkCore
{
    public class EntityRepository<TContext, TEntity> : IRepository<TEntity>
        where TContext : DbContext
        where TEntity : class, new()
    {
        protected readonly TContext dbContext;

        public EntityRepository(TContext dbContext) =>
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _ = this.dbContext.Add(entity);

            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            this.dbContext.AddRange(entities);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.dbContext.Entry(entity).State = EntityState.Deleted;

            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var entity in entities)
            {
                this.dbContext.Entry(entity).State = EntityState.Deleted;
            }

            return Task.CompletedTask;
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.dbContext.Entry(entity).State = EntityState.Modified;

            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var entity in entities)
            {
                this.dbContext.Entry(entity).State = EntityState.Modified;
            }

            return Task.CompletedTask;
        }
    }
}
