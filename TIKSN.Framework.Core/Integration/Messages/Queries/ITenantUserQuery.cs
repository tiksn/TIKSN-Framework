namespace TIKSN.Integration.Messages.Queries;

public interface ITenantUserQuery<out TTenantIdentity, out TUserIdentity, out TResult>
  : ITenantQuery<TTenantIdentity, TResult>
  , IUserQuery<TUserIdentity, TResult>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>;

