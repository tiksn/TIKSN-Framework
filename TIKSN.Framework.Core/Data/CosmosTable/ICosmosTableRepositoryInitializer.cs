using Microsoft.Azure.Cosmos.Table;

namespace TIKSN.Data.CosmosTable;

public interface ICosmosTableRepositoryInitializer<T> where T : ITableEntity
{
    Task InitializeAsync(CancellationToken cancellationToken);
}
