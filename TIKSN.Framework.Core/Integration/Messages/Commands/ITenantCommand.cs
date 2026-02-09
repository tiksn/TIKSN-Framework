namespace TIKSN.Integration.Messages.Commands;

public interface ITenantCommand<out TTenantIdentity>
  : ICommand
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
    public TTenantIdentity TenantID { get; }
}
