using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Data.LiteDB
{
    public abstract class LiteDbRepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity> :
        RepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity>,
        ILiteDbRepository<TDomainEntity, TDomainIdentity>
        where TDomainEntity : IEntity<TDomainIdentity>
        where TDomainIdentity : IEquatable<TDomainIdentity>
        where TDataEntity : IEntity<TDataIdentity>
        where TDataIdentity : IEquatable<TDataIdentity>
    {
        protected readonly ILiteDbRepository<TDataEntity, TDataIdentity> liteDbRepository;

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
                liteDbRepository)
        {
            this.liteDbRepository = liteDbRepository ?? throw new ArgumentNullException(nameof(liteDbRepository));
        }

        public Task AddOrUpdateAsync(TDomainEntity entity, CancellationToken cancellationToken)
            => this.liteDbRepository.AddOrUpdateAsync(this.Map(entity), cancellationToken);

        public Task AddOrUpdateRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
            => this.liteDbRepository.AddOrUpdateRangeAsync(this.Map(entities), cancellationToken);
    }
}
