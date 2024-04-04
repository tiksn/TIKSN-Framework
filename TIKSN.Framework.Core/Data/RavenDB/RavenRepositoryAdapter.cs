using TIKSN.Mapping;

namespace TIKSN.Data.RavenDB;

public abstract class RavenRepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity> :
    RepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity>,
    IRavenRepository<TDomainEntity, TDomainIdentity>
    where TDomainEntity : IEntity<TDomainIdentity>
    where TDomainIdentity : IEquatable<TDomainIdentity>
    where TDataEntity : IEntity<TDataIdentity>
    where TDataIdentity : IEquatable<TDataIdentity>
{
    protected RavenRepositoryAdapter(
        IMapper<TDomainEntity, TDataEntity> domainEntityToDataEntityMapper,
        IMapper<TDataEntity, TDomainEntity> dataEntityToDomainEntityMapper,
        IMapper<TDomainIdentity, TDataIdentity> domainIdentityToDataIdentityMapper,
        IMapper<TDataIdentity, TDomainIdentity> dataIdentityToDomainIdentityMapper,
        IRavenRepository<TDataEntity, TDataIdentity> ravenRepository) : base(
            domainEntityToDataEntityMapper,
            dataEntityToDomainEntityMapper,
            domainIdentityToDataIdentityMapper,
            dataIdentityToDomainIdentityMapper,
            ravenRepository,
            ravenRepository,
            ravenRepository) => this.RavenRepository = ravenRepository ?? throw new ArgumentNullException(nameof(ravenRepository));

    protected IRavenRepository<TDataEntity, TDataIdentity> RavenRepository { get; }
}
