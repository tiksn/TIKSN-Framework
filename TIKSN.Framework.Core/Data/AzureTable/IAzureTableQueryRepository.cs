using System.Linq.Expressions;
using Azure.Data.Tables;

namespace TIKSN.Data.AzureTable;

public interface IAzureTableQueryRepository<T> where T : ITableEntity
{
    Task<T> RetrieveAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);

    Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
}
