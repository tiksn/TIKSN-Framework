using LiteGuard;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.AzureStorage
{
    public static class AzureTableStorageExtensions
    {
        public static Task<IEnumerable<T>> RetrieveAllAsync<T>(this IAzureTableStorageQueryRepository<T> repository,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : ITableEntity
        {
            Guard.AgainstNullArgument(nameof(repository), repository);

            var filters = new Dictionary<string, object>();

            return repository.SearchAsync(filters, cancellationToken);
        }

        public static Task<IEnumerable<T>> SearchAsync<T>(this IAzureTableStorageQueryRepository<T> repository,
            string fieldName,
            object givenValue,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : ITableEntity
        {
            Guard.AgainstNullArgument(nameof(repository), repository);

            var filters = new Dictionary<string, object>();

            filters.Add(fieldName, givenValue);

            return repository.SearchAsync(filters, cancellationToken);
        }
    }
}