using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace TIKSN.Data.CosmosTable
{
    public interface ICosmosTableQueryRepository<T> where T : ITableEntity
    {
        Task<T> RetrieveAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);

        Task<IEnumerable<T>> SearchAsync(IDictionary<string, object> filters, CancellationToken cancellationToken);
    }
}
