namespace TIKSN.Data;

public record TenantUserEntity<TEntityIdentity, TTenantIdentity, TUserIdentity>(
  TEntityIdentity ID,
  TTenantIdentity TenantID,
  TUserIdentity UserID)
  : ITenantEntity<TEntityIdentity, TTenantIdentity>, IUserEntity<TEntityIdentity, TUserIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>;
