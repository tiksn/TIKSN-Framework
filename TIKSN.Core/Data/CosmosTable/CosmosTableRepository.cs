﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;

namespace TIKSN.Data.CosmosTable
{
    public class CosmosTableRepository<T> :
        CosmosTableBase,
        ICosmosTableRepository<T>,
        ICosmosTableQueryRepository<T>,
        ICosmosTableRepositoryInitializer<T> where T : TableEntity, new()
    {
        private readonly string _tableName;

        protected CosmosTableRepository(string tableName, IConfigurationRoot configuration, string connectionStringKey)
            : base(configuration, connectionStringKey) => this._tableName = tableName;

        public async Task<T> RetrieveAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            var retriveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            var retrivalResult = await table.ExecuteAsync(retriveOperation, null, null, cancellationToken);

            if (retrivalResult.Result == null)
            {
                return default;
            }

            return (T)retrivalResult.Result;
        }

        public Task<IEnumerable<T>> SearchAsync(IDictionary<string, object> filters,
            CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            string combinedFilter = null;

            foreach (var filter in filters)
            {
                var filterCondition = GenerateFilterCondition(filter.Key, QueryComparisons.Equal, filter.Value);

                if (string.IsNullOrEmpty(combinedFilter))
                {
                    combinedFilter = filterCondition;
                }
                else
                {
                    combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, filterCondition);
                }
            }

            var query = new TableQuery<T>().Where(combinedFilter);

            return this.SearchAsync(table, query, cancellationToken);
        }

        public Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            var insertOperation = TableOperation.Insert(entity);

            return table.ExecuteAsync(insertOperation, null, null, cancellationToken);
        }

        public Task AddOrMergeAsync(T entity, CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            var insertOperation = TableOperation.InsertOrMerge(entity);

            return table.ExecuteAsync(insertOperation, null, null, cancellationToken);
        }

        public Task AddOrReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            var insertOperation = TableOperation.InsertOrReplace(entity);

            return table.ExecuteAsync(insertOperation, null, null, cancellationToken);
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            var deleteOperation = TableOperation.Delete(entity);

            return table.ExecuteAsync(deleteOperation, null, null, cancellationToken);
        }

        public Task MergeAsync(T entity, CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            var mergeOperation = TableOperation.Merge(entity);

            return table.ExecuteAsync(mergeOperation, null, null, cancellationToken);
        }

        public Task ReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            var mergeOperation = TableOperation.Replace(entity);

            return table.ExecuteAsync(mergeOperation, null, null, cancellationToken);
        }

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            var table = this.GetCloudTable();

            return table.CreateIfNotExistsAsync();
        }

        protected CloudTable GetCloudTable()
        {
            var storageAccount = this.GetCloudStorageAccount();

            var tableClient = storageAccount.CreateCloudTableClient();

            return tableClient.GetTableReference(this._tableName);
        }

        private static string GenerateFilterCondition(string fieldName, string operation, object givenValue)
        {
            switch (givenValue)
            {
                case string s:
                    return TableQuery.GenerateFilterCondition(fieldName, operation, s);

                case byte[] binary:
                    return TableQuery.GenerateFilterConditionForBinary(fieldName, operation, binary);

                case bool boolean:
                    return TableQuery.GenerateFilterConditionForBool(fieldName, operation, boolean);

                case DateTimeOffset date:
                    return TableQuery.GenerateFilterConditionForDate(fieldName, operation, date);

                case int i:
                    return TableQuery.GenerateFilterConditionForInt(fieldName, operation, i);

                case long l:
                    return TableQuery.GenerateFilterConditionForLong(fieldName, operation, l);

                case double d:
                    return TableQuery.GenerateFilterConditionForDouble(fieldName, operation, d);

                case Guid g:
                    return TableQuery.GenerateFilterConditionForGuid(fieldName, operation, g);

                default:
                    throw new NotSupportedException("Search filter value type is not supported.");
            }
        }

        private async Task<IEnumerable<T>> SearchAsync(CloudTable table, TableQuery<T> query,
            CancellationToken cancellationToken)
        {
            var result = new List<T>();

            TableContinuationToken continuationToken = null;
            do
            {
                var tableSegment =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken, null, null, cancellationToken);

                result.AddRange(tableSegment.Results);

                continuationToken = tableSegment.ContinuationToken;
            } while (continuationToken != null);

            return result;
        }
    }
}
