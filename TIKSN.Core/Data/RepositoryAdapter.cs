using TIKSN.Mapping;

namespace TIKSN.Data
{
    public abstract class RepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity>
        : IRepository<TDomainEntity>, IQueryRepository<TDomainEntity, TDomainIdentity>, IStreamRepository<TDomainEntity>
        where TDomainEntity : IEntity<TDomainIdentity>
        where TDomainIdentity : IEquatable<TDomainIdentity>
        where TDataEntity : IEntity<TDataIdentity>
        where TDataIdentity : IEquatable<TDataIdentity>
    {
        protected readonly IMapper<TDataEntity, TDomainEntity> dataEntityToDomainEntityMapper;
        protected readonly IMapper<TDataIdentity, TDomainIdentity> dataIdentityToDomainIdentityMapper;
        protected readonly IMapper<TDomainEntity, TDataEntity> domainEntityToDataEntityMapper;
        protected readonly IMapper<TDomainIdentity, TDataIdentity> domainIdentityToDataIdentityMapper;
        protected readonly IQueryRepository<TDataEntity, TDataIdentity> queryRepository;
        protected readonly IRepository<TDataEntity> repository;
        protected readonly IStreamRepository<TDataEntity> streamRepository;

        protected RepositoryAdapter(
            IMapper<TDomainEntity, TDataEntity> domainEntityToDataEntityMapper,
            IMapper<TDataEntity, TDomainEntity> dataEntityToDomainEntityMapper,
            IMapper<TDomainIdentity, TDataIdentity> domainIdentityToDataIdentityMapper,
            IMapper<TDataIdentity, TDomainIdentity> dataIdentityToDomainIdentityMapper,
            IRepository<TDataEntity> repository,
            IQueryRepository<TDataEntity, TDataIdentity> queryRepository,
            IStreamRepository<TDataEntity> streamRepository)
        {
            this.domainEntityToDataEntityMapper = domainEntityToDataEntityMapper ?? throw new ArgumentNullException(nameof(domainEntityToDataEntityMapper));
            this.dataEntityToDomainEntityMapper = dataEntityToDomainEntityMapper ?? throw new ArgumentNullException(nameof(dataEntityToDomainEntityMapper));
            this.domainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
            this.dataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
            this.streamRepository = streamRepository ?? throw new ArgumentNullException(nameof(streamRepository));
        }

        public Task AddAsync(TDomainEntity entity, CancellationToken cancellationToken)
            => this.repository.AddAsync(this.Map(entity), cancellationToken);

        public Task AddRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
            => this.repository.AddRangeAsync(this.Map(entities), cancellationToken);

        public Task<bool> ExistsAsync(TDomainIdentity id, CancellationToken cancellationToken)
            => this.queryRepository.ExistsAsync(this.Map(id), cancellationToken);

        public async Task<TDomainEntity> GetAsync(TDomainIdentity id, CancellationToken cancellationToken)
            => this.Map(await this.queryRepository.GetAsync(this.Map(id), cancellationToken));

        public async Task<TDomainEntity> GetOrDefaultAsync(TDomainIdentity id, CancellationToken cancellationToken)
            => this.Map(await this.queryRepository.GetOrDefaultAsync(this.Map(id), cancellationToken));

        public async Task<IEnumerable<TDomainEntity>> ListAsync(IEnumerable<TDomainIdentity> ids, CancellationToken cancellationToken)
            => this.Map(await this.queryRepository.ListAsync(this.Map(ids), cancellationToken));

        public Task RemoveAsync(TDomainEntity entity, CancellationToken cancellationToken)
            => this.repository.RemoveAsync(this.Map(entity), cancellationToken);

        public Task RemoveRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
            => this.repository.RemoveRangeAsync(this.Map(entities), cancellationToken);

        public IAsyncEnumerable<TDomainEntity> StreamAllAsync(CancellationToken cancellationToken)
            => this.streamRepository.StreamAllAsync(cancellationToken).Select(this.Map);

        public Task UpdateAsync(TDomainEntity entity, CancellationToken cancellationToken)
            => this.repository.UpdateAsync(this.Map(entity), cancellationToken);

        public Task UpdateRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
            => this.repository.UpdateRangeAsync(this.Map(entities), cancellationToken);

        protected TDomainEntity Map(TDataEntity entity)
        {
            if (entity is null)
            {
                return default;
            }

            return this.dataEntityToDomainEntityMapper.Map(entity);
        }

        protected IReadOnlyList<TDomainEntity> Map(IEnumerable<TDataEntity> entities)
            => entities.Select(this.dataEntityToDomainEntityMapper.Map).ToArray();

        protected IReadOnlyList<TDomainIdentity> Map(IEnumerable<TDataIdentity> identities)
            => identities.Select(this.dataIdentityToDomainIdentityMapper.Map).ToArray();

        protected TDomainIdentity Map(TDataIdentity identity)
            => this.dataIdentityToDomainIdentityMapper.Map(identity);

        protected TDataEntity Map(TDomainEntity entity)
        {
            if (entity is null)
            {
                return default;
            }

            return this.domainEntityToDataEntityMapper.Map(entity);
        }

        protected IReadOnlyList<TDataEntity> Map(IEnumerable<TDomainEntity> entities)
            => entities.Select(this.domainEntityToDataEntityMapper.Map).ToArray();

        protected TDataIdentity Map(TDomainIdentity identity)
            => this.domainIdentityToDataIdentityMapper.Map(identity);

        protected IReadOnlyList<TDataIdentity> Map(IEnumerable<TDomainIdentity> identities)
            => identities.Select(this.domainIdentityToDataIdentityMapper.Map).ToArray();

        protected async Task<IReadOnlyList<TDomainEntity>> MapAsync(
            Func<Task<IReadOnlyList<TDataEntity>>> retriever)
        {
            if (retriever is null)
            {
                throw new ArgumentNullException(nameof(retriever));
            }

            var entities = await retriever().ConfigureAwait(false);

            return this.Map(entities);
        }
    }
}
