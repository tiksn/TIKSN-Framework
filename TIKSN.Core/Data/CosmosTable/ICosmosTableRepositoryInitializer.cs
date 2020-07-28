using Microsoft.Azure.Cosmos.Table;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.CosmosTable
{
    public interface ICosmosTableRepositoryInitializer<T> where T : ITableEntity
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}