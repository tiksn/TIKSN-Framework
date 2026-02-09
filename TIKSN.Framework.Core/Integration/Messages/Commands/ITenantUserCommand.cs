namespace TIKSN.Integration.Messages.Commands;

public interface ITenantUserCommand<out TTenantIdentity, out TUserIdentity>
  : ITenantCommand<TTenantIdentity>, IUserCommand<TUserIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
  where TUserIdentity : IEquatable<TUserIdentity>;
