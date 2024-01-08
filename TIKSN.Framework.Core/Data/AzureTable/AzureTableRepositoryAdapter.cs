using Azure.Data.Tables;
using TIKSN.Mapping;

namespace TIKSN.Data.AzureTable;

public class AzureTableRepositoryAdapter<TBusinessEntity, TDataEntity>
    : IRepository<TBusinessEntity>
    where TDataEntity : class, ITableEntity where TBusinessEntity : class
{
    private readonly IMapper<TBusinessEntity, TDataEntity> mapper;

    public AzureTableRepositoryAdapter(
        IAzureTableRepository<TDataEntity> dataRepository,
        IMapper<TBusinessEntity, TDataEntity> mapper)
    {
        this.DataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    protected IAzureTableRepository<TDataEntity> DataRepository { get; }

    public Task AddAsync(TBusinessEntity entity, CancellationToken cancellationToken)
    {
        var dataEntity = this.mapper.Map(entity);

        return this.DataRepository.AddAsync(dataEntity, cancellationToken);
    }

    public Task AddRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.AddAsync, cancellationToken);
    }

    public Task RemoveAsync(TBusinessEntity entity, CancellationToken cancellationToken)
    {
        var dataEntity = this.mapper.Map(entity);

        return this.DataRepository.RemoveAsync(dataEntity, cancellationToken);
    }

    public Task RemoveRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.RemoveAsync, cancellationToken);
    }

    public Task UpdateAsync(TBusinessEntity entity, CancellationToken cancellationToken)
    {
        var dataEntity = this.mapper.Map(entity);

        return this.DataRepository.UpdateAsync(dataEntity, cancellationToken);
    }

    public Task UpdateRangeAsync(IEnumerable<TBusinessEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.UpdateAsync, cancellationToken);
    }
}
