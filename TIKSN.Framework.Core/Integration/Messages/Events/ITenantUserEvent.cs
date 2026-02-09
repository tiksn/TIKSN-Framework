namespace TIKSN.Integration.Messages.Events;

public interface ITenantUserEvent<out TTenantIdentity, out TUserIdentity>
  : ITenantEvent<TTenantIdentity>
  , IUserEvent<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>;
