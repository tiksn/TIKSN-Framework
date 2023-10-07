using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;
using TIKSN.Mapping;

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

        public Task AddAsync(T entity, CancellationToken cancellationToken) => this._options.Value.AddOption switch
        {
            CosmosTableRepositoryAdapterOptions.AddOptions.Add => this._azureTableStorageRepository.AddAsync(entity, cancellationToken),
            CosmosTableRepositoryAdapterOptions.AddOptions.AddOrMerge => this._azureTableStorageRepository.AddOrMergeAsync(entity, cancellationToken),
            CosmosTableRepositoryAdapterOptions.AddOptions.AddOrReplace => this._azureTableStorageRepository.AddOrReplaceAsync(entity, cancellationToken),
            _ => throw new NotSupportedException($"Add option '{this._options.Value.AddOption}' is not supported."),
        };

        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.AddAsync);

        public Task RemoveAsync(T entity, CancellationToken cancellationToken) =>
            this._azureTableStorageRepository.DeleteAsync(entity, cancellationToken);

        public Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken) =>
            BatchOperationHelper.BatchOperationAsync(entities, cancellationToken, this.RemoveAsync);

        public Task UpdateAsync(T entity, CancellationToken cancellationToken) => this._options.Value.UpdateOption switch
        {
            CosmosTableRepositoryAdapterOptions.UpdateOptions.Merge => this._azureTableStorageRepository.MergeAsync(entity, cancellationToken),
            CosmosTableRepositoryAdapterOptions.UpdateOptions.Replace => this._azureTableStorageRepository.ReplaceAsync(entity, cancellationToken),
            _ => throw new NotSupportedException(
$"Update option '{this._options.Value.AddOption}' is not supported."),
        };

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
