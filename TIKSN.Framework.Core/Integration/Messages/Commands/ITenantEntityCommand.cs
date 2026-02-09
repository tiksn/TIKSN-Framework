namespace TIKSN.Integration.Messages.Commands;

public interface ITenantEntityCommand<TEntityIdentity, out TTenantIdentity>
  : ITenantCommand<TTenantIdentity>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
    public IEnumerable<TEntityIdentity> TenantEntityIdentities { get; }
}
