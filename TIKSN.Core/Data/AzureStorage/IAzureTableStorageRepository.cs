using Microsoft.WindowsAzure.Storage.Table;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.AzureStorage
{
	public interface IAzureTableStorageRepository<T> where T : ITableEntity
	{
		Task AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

		Task AddOrMergeAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

		Task AddOrReplaceAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

		Task DeleteAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

		Task MergeAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

		Task ReplaceAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
	}
}