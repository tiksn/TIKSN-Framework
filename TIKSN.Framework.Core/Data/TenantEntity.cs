namespace TIKSN.Data;

public record TenantEntity<TEntityIdentity, TTenantIdentity>(
  TEntityIdentity ID,
  TTenantIdentity TenantID)
  : ITenantEntity<TEntityIdentity, TTenantIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>;
