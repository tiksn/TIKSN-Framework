using System.Linq.Expressions;
using Azure.Data.Tables;

namespace TIKSN.Data.AzureTable;

public interface IAzureTableQueryRepository<T> where T : ITableEntity
{
    Task<T> RetrieveAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<T>> SearchAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
}
