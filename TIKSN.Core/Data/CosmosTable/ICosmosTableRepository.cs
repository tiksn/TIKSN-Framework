using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace TIKSN.Data.CosmosTable
{
    public interface ICosmosTableRepository<T> where T : ITableEntity
    {
        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task AddOrMergeAsync(T entity, CancellationToken cancellationToken);

        Task AddOrReplaceAsync(T entity, CancellationToken cancellationToken);

        Task DeleteAsync(T entity, CancellationToken cancellationToken);

        Task MergeAsync(T entity, CancellationToken cancellationToken);

        Task ReplaceAsync(T entity, CancellationToken cancellationToken);
    }
}
