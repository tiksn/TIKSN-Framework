using Microsoft.Azure.Cosmos.Table;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.AzureStorage
{
    public interface IAzureTableStorageRepositoryInitializer<T> where T : ITableEntity
    {
        Task InitializeAsync(CancellationToken cancellationToken);
    }
}