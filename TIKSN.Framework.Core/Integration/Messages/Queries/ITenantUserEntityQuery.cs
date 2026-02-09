namespace TIKSN.Integration.Messages.Queries;

public interface ITenantUserEntityQuery<TEntityIdentity, out TTenantIdentity, out TUserIdentity, out TResult>
  : ITenantEntityQuery<TEntityIdentity, TTenantIdentity, TResult>
  , IUserQuery<TUserIdentity, TResult>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>;

