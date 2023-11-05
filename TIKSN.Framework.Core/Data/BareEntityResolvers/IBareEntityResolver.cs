namespace TIKSN.Data.BareEntityResolvers;

#pragma warning disable S2326 // Unused type parameters should be removed
public interface IBareEntityResolver<TEntity, TBareEntity, in TIdentity>
#pragma warning restore S2326 // Unused type parameters should be removed
  : IBareEntityResolver<TBareEntity, TIdentity>
  where TEntity : IEntity<TIdentity>
  where TBareEntity : IEntity<TIdentity>
  where TIdentity : IEquatable<TIdentity>
{
}

public interface IBareEntityResolver<TBareEntity, in TIdentity>
  where TBareEntity : IEntity<TIdentity>
  where TIdentity : IEquatable<TIdentity>
{
    Task<TBareEntity> ResolveAsync(
      TIdentity id,
      CancellationToken cancellationToken);
}
