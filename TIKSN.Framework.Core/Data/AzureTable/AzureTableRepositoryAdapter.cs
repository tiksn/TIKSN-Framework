using Azure.Data.Tables;
using TIKSN.Mapping;

namespace TIKSN.Data.AzureTable;

public class AzureTableRepositoryAdapter<TDomainEntity, TDataEntity>
    : IRepository<TDomainEntity>
    where TDomainEntity : class
    where TDataEntity : class, ITableEntity
{
    private readonly IMapper<TDomainEntity, TDataEntity> mapper;

    public AzureTableRepositoryAdapter(
        IAzureTableRepository<TDataEntity> dataRepository,
        IMapper<TDomainEntity, TDataEntity> mapper)
    {
        this.DataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    protected IAzureTableRepository<TDataEntity> DataRepository { get; }

    public Task AddAsync(TDomainEntity entity, CancellationToken cancellationToken)
    {
        var dataEntity = this.mapper.Map(entity);

        return this.DataRepository.AddAsync(dataEntity, cancellationToken);
    }

    public Task AddRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.AddAsync, cancellationToken);
    }

    public Task RemoveAsync(TDomainEntity entity, CancellationToken cancellationToken)
    {
        var dataEntity = this.mapper.Map(entity);

        return this.DataRepository.RemoveAsync(dataEntity, cancellationToken);
    }

    public Task RemoveRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.RemoveAsync, cancellationToken);
    }

    public Task UpdateAsync(TDomainEntity entity, CancellationToken cancellationToken)
    {
        var dataEntity = this.mapper.Map(entity);

        return this.DataRepository.UpdateAsync(dataEntity, cancellationToken);
    }

    public Task UpdateRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entities);

        return BatchOperationHelper.BatchOperationAsync(entities, this.UpdateAsync, cancellationToken);
    }
}
