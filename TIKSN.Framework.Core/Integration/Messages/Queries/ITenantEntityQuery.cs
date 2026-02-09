namespace TIKSN.Integration.Messages.Queries;

public interface ITenantEntityQuery<TEntityIdentity, out TTenantIdentity, out TResult>
  : ITenantQuery<TTenantIdentity, TResult>
  where TEntityIdentity : IEquatable<TEntityIdentity>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
    public IEnumerable<TEntityIdentity> TenantEntityIdentities { get; }
}
