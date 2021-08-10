using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace TIKSN.Data.CosmosTable
{
    public class CosmosTableRepositoryAdapter<T> : IRepository<T> where T : ITableEntity
    {
        private readonly ICosmosTableRepository<T> _azureTableStorageRepository;
        private readonly IOptions<CosmosTableRepositoryAdapterOptions> _options;

        public CosmosTableRepositoryAdapter(ICosmosTableRepository<T> azureTableStorageRepository,
            IOptions<CosmosTableRepositoryAdapterOptions> options)
        {
            this._azureTableStorageRepository = azureTableStorageRepository;
            this._options = options;
        }

        public Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            switch (this._options.Value.AddOption)
            {
                case CosmosTableRepositoryAdapterOptions.AddOptions.Add:
                    return this._azureTableStorageRepository.AddAsync(entity, cancellationToken);

                case CosmosTableRepositoryAdapterOptions.AddOptions.AddOrMerge:
                    return this._azureTableStorageRepository.AddOrMergeAsync(entity, cancellationToken);

                case CosmosTableRepositoryAdapterOptions.AddOptions.AddOrReplace:
                    return this._azureTableStorageRepository.AddOrReplaceAsync(entity, cancellationToken);

                default:
                    throw new NotSupportedException($"Add option '{this._options.Value.AddOption}' is not supported.");
            }
        }

        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.AddAsync);

        public Task RemoveAsync(T entity, CancellationToken cancellationToken) =>
            this._azureTableStorageRepository.DeleteAsync(entity, cancellationToken);

        public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.RemoveAsync);

        public Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            switch (this._options.Value.UpdateOption)
            {
                case CosmosTableRepositoryAdapterOptions.UpdateOptions.Merge:
                    return this._azureTableStorageRepository.MergeAsync(entity, cancellationToken);

                case CosmosTableRepositoryAdapterOptions.UpdateOptions.Replace:
                    return this._azureTableStorageRepository.ReplaceAsync(entity, cancellationToken);

                default:
                    throw new NotSupportedException(
                        $"Update option '{this._options.Value.AddOption}' is not supported.");
            }
        }

        public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);
    }

    public class AzureTableStorageRepositoryAdapter<TBusinessEntity, TDataEntity>
        : CosmosTableRepositoryAdapter<TDataEntity>, IRepository<TBusinessEntity>
        where TDataEntity : class, ITableEntity where TBusinessEntity : class
    {
        private readonly IMapper<TBusinessEntity, TDataEntity> _mapper;

        public AzureTableStorageRepositoryAdapter(
            ICosmosTableRepository<TDataEntity> azureTableStorageRepository,
            IOptions<CosmosTableRepositoryAdapterOptions> options,
            IMapper<TBusinessEntity, TDataEntity> mapper) : base(azureTableStorageRepository, options) =>
            this._mapper = mapper;

        public Task AddAsync(TBusinessEntity entity, CancellationToken cancellationToken)
        {
            var dataEntity = this._mapper.Map(entity);

            return this.AddAsync(dataEntity, cancellationToken);
        }

        public Task AddRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.AddAsync);

        public Task RemoveAsync(TBusinessEntity entity, CancellationToken cancellationToken)
        {
            var dataEntity = this._mapper.Map(entity);

            return this.RemoveAsync(dataEntity, cancellationToken);
        }

        public Task RemoveRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.RemoveAsync);

        public Task UpdateAsync(TBusinessEntity entity, CancellationToken cancellationToken)
        {
            var dataEntity = this._mapper.Map(entity);

            return this.UpdateAsync(dataEntity, cancellationToken);
        }

        public Task UpdateRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.UpdateAsync);
    }
}
