namespace TIKSN.Integration.Messages.Queries;

public interface ITenantQuery<out TTenantIdentity, out TResult>
  : IQuery<TResult>
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
    public TTenantIdentity TenantID { get; }
}
