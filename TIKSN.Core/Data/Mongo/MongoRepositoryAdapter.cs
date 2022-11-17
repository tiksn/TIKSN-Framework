using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.Mongo
{
    public abstract class MongoRepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity> :
        RepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity>,
        IMongoRepository<TDomainEntity, TDomainIdentity>
        where TDomainEntity : IEntity<TDomainIdentity>
        where TDomainIdentity : IEquatable<TDomainIdentity>
        where TDataEntity : IEntity<TDataIdentity>
        where TDataIdentity : IEquatable<TDataIdentity>
    {
        protected readonly IMongoRepository<TDataEntity, TDataIdentity> mongoRepository;

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
                mongoRepository)
        {
            this.mongoRepository = mongoRepository ?? throw new ArgumentNullException(nameof(mongoRepository));
        }

        public Task AddOrUpdateAsync(TDomainEntity entity, CancellationToken cancellationToken)
            => this.mongoRepository.AddOrUpdateAsync(this.Map(entity), cancellationToken);
    }
}
