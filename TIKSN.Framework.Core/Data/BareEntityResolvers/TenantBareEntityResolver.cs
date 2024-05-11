namespace TIKSN.Data.BareEntityResolvers;

public class TenantBareEntityResolver<TEntity, TEntityIdentity, TTenantIdentity>
    : IBareEntityResolver<TEntity, TenantEntity<TEntityIdentity, TTenantIdentity>, TEntityIdentity>
    where TEntity : ITenantEntity<TEntityIdentity, TTenantIdentity>
    where TEntityIdentity : IEquatable<TEntityIdentity>
    where TTenantIdentity : IEquatable<TTenantIdentity>
{
    private readonly IQueryRepository<TEntity, TEntityIdentity> queryRepository;

    public TenantBareEntityResolver(
        IQueryRepository<TEntity, TEntityIdentity> queryRepository)
            => this.queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));

    public async Task<TenantEntity<TEntityIdentity, TTenantIdentity>> ResolveAsync(
        TEntityIdentity id,
        CancellationToken cancellationToken)
    {
        var entity = await this.queryRepository.GetAsync(id, cancellationToken).ConfigureAwait(false);

        return new TenantEntity<TEntityIdentity, TTenantIdentity>(
          entity.ID, entity.TenantID);
    }
}
