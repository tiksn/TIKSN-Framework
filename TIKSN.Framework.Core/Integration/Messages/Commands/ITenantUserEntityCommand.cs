namespace TIKSN.Integration.Messages.Commands;

public interface ITenantUserEntityCommand<TEntityIdentity, out TTenantIdentity, out TUserIdentity>
  : ITenantEntityCommand<TEntityIdentity, TTenantIdentity>, ITenantUserCommand<TTenantIdentity, TUserIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>;
