namespace TIKSN.Data.BareEntityResolvers;

public class TenantUserBareEntityResolver<TEntity, TEntityIdentity, TTenantIdentity, TUserIdentity>
  : IBareEntityResolver<TEntity, TenantUserEntity<TEntityIdentity, TTenantIdentity, TUserIdentity>, TEntityIdentity>
  where TEntity : ITenantEntity<TEntityIdentity, TTenantIdentity>, IUserEntity<TEntityIdentity, TUserIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>
{
    private readonly IQueryRepository<TEntity, TEntityIdentity> queryRepository;

    public TenantUserBareEntityResolver(
      IQueryRepository<TEntity, TEntityIdentity> queryRepository)
        => this.queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));

    public async Task<TenantUserEntity<TEntityIdentity, TTenantIdentity, TUserIdentity>> ResolveAsync(
      TEntityIdentity id,
      CancellationToken cancellationToken)
    {
        var entity = await this.queryRepository.GetAsync(id, cancellationToken).ConfigureAwait(false);

        return new TenantUserEntity<TEntityIdentity, TTenantIdentity, TUserIdentity>(
          entity.ID, entity.TenantID, entity.UserID);
    }
}
