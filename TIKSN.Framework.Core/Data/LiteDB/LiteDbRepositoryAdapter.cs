using TIKSN.Mapping;

namespace TIKSN.Data.LiteDB;

public abstract class LiteDbRepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity> :
    RepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity>,
    ILiteDbRepository<TDomainEntity, TDomainIdentity>
    where TDomainEntity : IEntity<TDomainIdentity>
    where TDomainIdentity : IEquatable<TDomainIdentity>
    where TDataEntity : IEntity<TDataIdentity>
    where TDataIdentity : IEquatable<TDataIdentity>
{
    protected LiteDbRepositoryAdapter(
        IMapper<TDomainEntity, TDataEntity> domainEntityToDataEntityMapper,
        IMapper<TDataEntity, TDomainEntity> dataEntityToDomainEntityMapper,
        IMapper<TDomainIdentity, TDataIdentity> domainIdentityToDataIdentityMapper,
        IMapper<TDataIdentity, TDomainIdentity> dataIdentityToDomainIdentityMapper,
        ILiteDbRepository<TDataEntity, TDataIdentity> liteDbRepository) : base(
            domainEntityToDataEntityMapper,
            dataEntityToDomainEntityMapper,
            domainIdentityToDataIdentityMapper,
            dataIdentityToDomainIdentityMapper,
            liteDbRepository,
            liteDbRepository,
            liteDbRepository) => this.LiteDbRepository = liteDbRepository ?? throw new ArgumentNullException(nameof(liteDbRepository));

    protected ILiteDbRepository<TDataEntity, TDataIdentity> LiteDbRepository { get; }

    public Task AddOrUpdateAsync(TDomainEntity entity, CancellationToken cancellationToken)
        => this.LiteDbRepository.AddOrUpdateAsync(this.Map(entity), cancellationToken);

    public Task AddOrUpdateRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
        => this.LiteDbRepository.AddOrUpdateRangeAsync(this.Map(entities), cancellationToken);
}
