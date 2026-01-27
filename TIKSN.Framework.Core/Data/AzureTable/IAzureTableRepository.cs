using Azure.Data.Tables;

namespace TIKSN.Data.AzureTable;

public interface IAzureTableRepository<in T> : IRepository<T> where T : ITableEntity
{
    public Task AddOrMergeAsync(T entity, CancellationToken cancellationToken);

    public Task AddOrReplaceAsync(T entity, CancellationToken cancellationToken);

    public Task MergeAsync(T entity, CancellationToken cancellationToken);

    public Task ReplaceAsync(T entity, CancellationToken cancellationToken);
}
