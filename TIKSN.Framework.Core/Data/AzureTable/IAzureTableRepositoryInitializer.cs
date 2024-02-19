using Azure.Data.Tables;

namespace TIKSN.Data.AzureTable;

public interface IAzureTableRepositoryInitializer<T> where T : ITableEntity
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
