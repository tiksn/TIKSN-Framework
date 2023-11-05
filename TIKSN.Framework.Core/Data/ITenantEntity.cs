namespace TIKSN.Data;

public interface ITenantEntity<TEntityIdentity, out TTenantIdentity>
  : IEntity<TEntityIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
    TTenantIdentity TenantID { get; }
}
