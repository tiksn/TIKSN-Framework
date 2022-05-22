using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace TIKSN.Data.CosmosTable
{
    public static class CosmosTableExtensions
    {
        public static Task<IEnumerable<T>> RetrieveAllAsync<T>(this ICosmosTableQueryRepository<T> repository,
            CancellationToken cancellationToken)
            where T : ITableEntity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var filters = new Dictionary<string, object>();

            return repository.SearchAsync(filters, cancellationToken);
        }

        public static Task<IEnumerable<T>> SearchAsync<T>(this ICosmosTableQueryRepository<T> repository,
            string fieldName,
            object givenValue,
            CancellationToken cancellationToken)
            where T : ITableEntity
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            var filters = new Dictionary<string, object>
            {
                { fieldName, givenValue }
            };

            return repository.SearchAsync(filters, cancellationToken);
        }
    }
}
