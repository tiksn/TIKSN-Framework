using Azure.Data.Tables;

namespace TIKSN.Data.AzureTable;

public interface IAzureTableRepository<in T> : IRepository<T> where T : ITableEntity
{
    Task AddOrMergeAsync(T entity, CancellationToken cancellationToken);

    Task AddOrReplaceAsync(T entity, CancellationToken cancellationToken);

    Task MergeAsync(T entity, CancellationToken cancellationToken);

    Task ReplaceAsync(T entity, CancellationToken cancellationToken);
}
