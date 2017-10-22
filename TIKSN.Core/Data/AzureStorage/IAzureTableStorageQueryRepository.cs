using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.AzureStorage
{
    public interface IAzureTableStorageQueryRepository<T> where T : ITableEntity
    {
        Task<T> RetrieveAsync(string partitionKey, string rowKey, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<T>> SearchAsync(IDictionary<string, object> filters, CancellationToken cancellationToken = default(CancellationToken));
    }
}