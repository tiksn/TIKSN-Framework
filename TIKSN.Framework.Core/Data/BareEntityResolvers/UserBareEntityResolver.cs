namespace TIKSN.Data.BareEntityResolvers;

public class UserBareEntityResolver<TEntity, TEntityIdentity, TUserIdentity>
    : IBareEntityResolver<TEntity, UserEntity<TEntityIdentity, TUserIdentity>, TEntityIdentity>
    where TEntity : IUserEntity<TEntityIdentity, TUserIdentity>
    where TEntityIdentity : IEquatable<TEntityIdentity>
    where TUserIdentity : IEquatable<TUserIdentity>
{
    private readonly IQueryRepository<TEntity, TEntityIdentity> queryRepository;

    public UserBareEntityResolver(
        IQueryRepository<TEntity, TEntityIdentity> queryRepository)
            => this.queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));

    public async Task<UserEntity<TEntityIdentity, TUserIdentity>> ResolveAsync(
        TEntityIdentity id,
        CancellationToken cancellationToken)
    {
        var entity = await this.queryRepository.GetAsync(id, cancellationToken).ConfigureAwait(false);

        return new UserEntity<TEntityIdentity, TUserIdentity>(
          entity.ID, entity.UserID);
    }
}
