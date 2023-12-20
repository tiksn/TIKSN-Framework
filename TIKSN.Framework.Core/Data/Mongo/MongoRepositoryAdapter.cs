using TIKSN.Mapping;

namespace TIKSN.Data.Mongo;

public abstract class MongoRepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity> :
    RepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity>,
    IMongoRepository<TDomainEntity, TDomainIdentity>
    where TDomainEntity : IEntity<TDomainIdentity>
    where TDomainIdentity : IEquatable<TDomainIdentity>
    where TDataEntity : IEntity<TDataIdentity>
    where TDataIdentity : IEquatable<TDataIdentity>
{
    protected MongoRepositoryAdapter(
        IMapper<TDomainEntity, TDataEntity> domainEntityToDataEntityMapper,
        IMapper<TDataEntity, TDomainEntity> dataEntityToDomainEntityMapper,
        IMapper<TDomainIdentity, TDataIdentity> domainIdentityToDataIdentityMapper,
        IMapper<TDataIdentity, TDomainIdentity> dataIdentityToDomainIdentityMapper,
        IMongoRepository<TDataEntity, TDataIdentity> mongoRepository) : base(
            domainEntityToDataEntityMapper,
            dataEntityToDomainEntityMapper,
            domainIdentityToDataIdentityMapper,
            dataIdentityToDomainIdentityMapper,
            mongoRepository,
            mongoRepository,
            mongoRepository) => this.MongoRepository = mongoRepository ?? throw new ArgumentNullException(nameof(mongoRepository));

    protected IMongoRepository<TDataEntity, TDataIdentity> MongoRepository { get; }

    public Task AddOrUpdateAsync(TDomainEntity entity, CancellationToken cancellationToken)
        => this.MongoRepository.AddOrUpdateAsync(this.Map(entity), cancellationToken);
}
