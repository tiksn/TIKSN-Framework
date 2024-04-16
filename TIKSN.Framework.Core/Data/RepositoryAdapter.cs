using TIKSN.Mapping;

namespace TIKSN.Data;

public abstract class RepositoryAdapter<TDomainEntity, TDomainIdentity, TDataEntity, TDataIdentity>
    : IRepository<TDomainEntity>, IQueryRepository<TDomainEntity, TDomainIdentity>, IStreamRepository<TDomainEntity>
    where TDomainEntity : IEntity<TDomainIdentity>
    where TDomainIdentity : IEquatable<TDomainIdentity>
    where TDataEntity : IEntity<TDataIdentity>
    where TDataIdentity : IEquatable<TDataIdentity>
{
    protected RepositoryAdapter(
        IMapper<TDomainEntity, TDataEntity> domainEntityToDataEntityMapper,
        IMapper<TDataEntity, TDomainEntity> dataEntityToDomainEntityMapper,
        IMapper<TDomainIdentity, TDataIdentity> domainIdentityToDataIdentityMapper,
        IMapper<TDataIdentity, TDomainIdentity> dataIdentityToDomainIdentityMapper,
        IRepository<TDataEntity> repository,
        IQueryRepository<TDataEntity, TDataIdentity> queryRepository,
        IStreamRepository<TDataEntity> streamRepository)
    {
        this.DomainEntityToDataEntityMapper = domainEntityToDataEntityMapper ?? throw new ArgumentNullException(nameof(domainEntityToDataEntityMapper));
        this.DataEntityToDomainEntityMapper = dataEntityToDomainEntityMapper ?? throw new ArgumentNullException(nameof(dataEntityToDomainEntityMapper));
        this.DomainIdentityToDataIdentityMapper = domainIdentityToDataIdentityMapper ?? throw new ArgumentNullException(nameof(domainIdentityToDataIdentityMapper));
        this.DataIdentityToDomainIdentityMapper = dataIdentityToDomainIdentityMapper ?? throw new ArgumentNullException(nameof(dataIdentityToDomainIdentityMapper));
        this.Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.QueryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        this.StreamRepository = streamRepository ?? throw new ArgumentNullException(nameof(streamRepository));
    }

    protected IMapper<TDataEntity, TDomainEntity> DataEntityToDomainEntityMapper { get; }
    protected IMapper<TDataIdentity, TDomainIdentity> DataIdentityToDomainIdentityMapper { get; }
    protected IMapper<TDomainEntity, TDataEntity> DomainEntityToDataEntityMapper { get; }
    protected IMapper<TDomainIdentity, TDataIdentity> DomainIdentityToDataIdentityMapper { get; }
    protected IQueryRepository<TDataEntity, TDataIdentity> QueryRepository { get; }
    protected IRepository<TDataEntity> Repository { get; }
    protected IStreamRepository<TDataEntity> StreamRepository { get; }

    public Task AddAsync(TDomainEntity entity, CancellationToken cancellationToken)
        => this.Repository.AddAsync(this.Map(entity), cancellationToken);

    public Task AddRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
        => this.Repository.AddRangeAsync(this.Map(entities), cancellationToken);

    public Task<bool> ExistsAsync(TDomainIdentity id, CancellationToken cancellationToken)
        => this.QueryRepository.ExistsAsync(this.Map(id), cancellationToken);

    public async Task<TDomainEntity> GetAsync(TDomainIdentity id, CancellationToken cancellationToken)
        => this.Map(await this.QueryRepository.GetAsync(this.Map(id), cancellationToken).ConfigureAwait(false));

    public async Task<TDomainEntity?> GetOrDefaultAsync(TDomainIdentity id, CancellationToken cancellationToken)
        => this.MapOrDefault(await this.QueryRepository.GetOrDefaultAsync(this.Map(id), cancellationToken).ConfigureAwait(false));

    public async Task<IEnumerable<TDomainEntity>> ListAsync(IEnumerable<TDomainIdentity> ids, CancellationToken cancellationToken)
        => this.Map(await this.QueryRepository.ListAsync(this.Map(ids), cancellationToken).ConfigureAwait(false));

    public async Task<PageResult<TDomainEntity>> PageAsync(PageQuery pageQuery, CancellationToken cancellationToken)
    {
        var pageResult = await this.QueryRepository.PageAsync(pageQuery, cancellationToken).ConfigureAwait(false);
        return new PageResult<TDomainEntity>(pageResult.Page, this.Map(pageResult.Items), pageResult.TotalItems);
    }

    public Task RemoveAsync(TDomainEntity entity, CancellationToken cancellationToken)
        => this.Repository.RemoveAsync(this.Map(entity), cancellationToken);

    public Task RemoveRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
        => this.Repository.RemoveRangeAsync(this.Map(entities), cancellationToken);

    public IAsyncEnumerable<TDomainEntity> StreamAllAsync(CancellationToken cancellationToken)
        => this.StreamRepository.StreamAllAsync(cancellationToken).Select(this.Map);

    public Task UpdateAsync(TDomainEntity entity, CancellationToken cancellationToken)
        => this.Repository.UpdateAsync(this.Map(entity), cancellationToken);

    public Task UpdateRangeAsync(IEnumerable<TDomainEntity> entities, CancellationToken cancellationToken)
        => this.Repository.UpdateRangeAsync(this.Map(entities), cancellationToken);

    protected TDomainEntity Map(TDataEntity entity)
        => this.DataEntityToDomainEntityMapper.Map(entity);

    protected IReadOnlyList<TDomainEntity> Map(IEnumerable<TDataEntity> entities)
        => entities.Select(this.DataEntityToDomainEntityMapper.Map).ToArray();

    protected IReadOnlyList<TDomainIdentity> Map(IEnumerable<TDataIdentity> identities)
        => identities.Select(this.DataIdentityToDomainIdentityMapper.Map).ToArray();

    protected TDomainIdentity Map(TDataIdentity identity)
        => this.DataIdentityToDomainIdentityMapper.Map(identity);

    protected TDataEntity Map(TDomainEntity entity)
        => this.DomainEntityToDataEntityMapper.Map(entity);

    protected IReadOnlyList<TDataEntity> Map(IEnumerable<TDomainEntity> entities)
        => entities.Select(this.DomainEntityToDataEntityMapper.Map).ToArray();

    protected TDataIdentity Map(TDomainIdentity identity)
        => this.DomainIdentityToDataIdentityMapper.Map(identity);

    protected IReadOnlyList<TDataIdentity> Map(IEnumerable<TDomainIdentity> identities)
        => identities.Select(this.DomainIdentityToDataIdentityMapper.Map).ToArray();

    protected async Task<IReadOnlyList<TDomainEntity>> MapAsync(
        Func<Task<IReadOnlyList<TDataEntity>>> retriever)
    {
        ArgumentNullException.ThrowIfNull(retriever);

        var entities = await retriever().ConfigureAwait(false);

        return this.Map(entities);
    }

    protected TDataEntity? MapOrDefault(TDomainEntity? entity)
    {
        if (entity is null)
        {
            return default;
        }

        return this.DomainEntityToDataEntityMapper.Map(entity);
    }

    protected TDomainEntity? MapOrDefault(TDataEntity? entity)
    {
        if (entity is null)
        {
            return default;
        }

        return this.DataEntityToDomainEntityMapper.Map(entity);
    }
}
