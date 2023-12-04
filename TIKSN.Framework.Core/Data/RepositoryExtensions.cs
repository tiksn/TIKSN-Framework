using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Data;

public static class RepositoryExtensions
{
    public static async Task<Option<TEntity>> GetOrNoneAsync<TEntity, TIdentity>(
        this IQueryRepository<TEntity, TIdentity> queryRepository,
        TIdentity id,
        CancellationToken cancellationToken)
          where TEntity : IEntity<TIdentity>
          where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(queryRepository);

        var entity = await queryRepository.GetOrDefaultAsync(id, cancellationToken).ConfigureAwait(false);

        return Optional(entity);
    }
}
