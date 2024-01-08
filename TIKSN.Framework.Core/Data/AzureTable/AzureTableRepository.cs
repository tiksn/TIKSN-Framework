using System.Linq.Expressions;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.AzureTable;

public class AzureTableRepository<T> :
    IAzureTableRepository<T>,
    IAzureTableQueryRepository<T>,
    IAzureTableRepositoryInitializer<T> where T : class, ITableEntity
{
    private readonly TableClient tableClient;
    private readonly string tableName;
    private readonly TableServiceClient tableServiceClient;

    protected AzureTableRepository(
        string tableName,
        IConfigurationRoot configuration,
        string connectionStringKey)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
        }

        ArgumentNullException.ThrowIfNull(configuration);

        if (string.IsNullOrEmpty(connectionStringKey))
        {
            throw new ArgumentException($"'{nameof(connectionStringKey)}' cannot be null or empty.", nameof(connectionStringKey));
        }

        this.tableName = tableName;

        this.tableServiceClient = new TableServiceClient(configuration.GetConnectionString(connectionStringKey));
        this.tableClient = new TableClient(configuration.GetConnectionString(connectionStringKey), tableName);
    }

    public Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.tableClient.AddEntityAsync(entity, cancellationToken);
    }

    public Task AddOrMergeAsync(T entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.tableClient.UpsertEntityAsync(entity, TableUpdateMode.Merge, cancellationToken);
    }

    public Task AddOrReplaceAsync(T entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.tableClient.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
    }

    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.AddAsync, cancellationToken);
    }

    public Task InitializeAsync(CancellationToken cancellationToken)
        => this.tableServiceClient.CreateTableIfNotExistsAsync(this.tableName, cancellationToken);

    public Task MergeAsync(T entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Merge, cancellationToken);
    }

    public Task RemoveAsync(T entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, entity.ETag, cancellationToken);
    }

    public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.RemoveAsync, cancellationToken);
    }

    public Task ReplaceAsync(T entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.tableClient.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace, cancellationToken);
    }

    public async Task<T> RetrieveAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
    {
        var response = await this.tableClient.GetEntityAsync<T>(partitionKey, rowKey, cancellationToken: cancellationToken).ConfigureAwait(false);
        return response.Value;
    }

    public async Task<IEnumerable<T>> SearchAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken)
    {
        var result = this.tableClient.QueryAsync(filter, maxPerPage: null, select: null, cancellationToken);
        return await result.ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.MergeAsync(entity, cancellationToken);
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.UpdateAsync, cancellationToken);
    }
}
