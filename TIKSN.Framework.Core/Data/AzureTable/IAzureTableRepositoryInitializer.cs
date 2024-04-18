using Azure.Data.Tables;

namespace TIKSN.Data.AzureTable;

#pragma warning disable S2326 // Unused type parameters should be removed

public interface IAzureTableRepositoryInitializer<T> where T : ITableEntity
#pragma warning restore S2326 // Unused type parameters should be removed
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
