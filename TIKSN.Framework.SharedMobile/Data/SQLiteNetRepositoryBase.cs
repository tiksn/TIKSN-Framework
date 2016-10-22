using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Data
{
    public abstract partial class SQLiteNetRepositoryBase<T> : IRepository<T> where T : class
    {
        private readonly IConfiguration<DatabaseConfiguration> databaseConfiguration;

        public SQLiteNetRepositoryBase(IConfiguration<DatabaseConfiguration> databaseConfiguration)
        {
            this.databaseConfiguration = databaseConfiguration;
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = CreateConnection();

            await connection.InsertAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = CreateConnection();

            await connection.InsertAllAsync(entities, cancellationToken);
        }

        public async Task RemoveAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = CreateConnection();

            await connection.DeleteAsync(entity, cancellationToken);
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = CreateConnection();

            foreach (var entity in entities)
            {
                await connection.DeleteAsync(entity, cancellationToken);
            }
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = CreateConnection();

            await connection.UpdateAsync(entity, cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = CreateConnection();

            await connection.UpdateAllAsync(entities, cancellationToken);
        }
    }
}