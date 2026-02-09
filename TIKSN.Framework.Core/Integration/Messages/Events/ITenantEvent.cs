namespace TIKSN.Integration.Messages.Events;

public interface ITenantEvent<out TTenantIdentity>
  : IEvent
  where TTenantIdentity : IEquatable<TTenantIdentity>
{
    public TTenantIdentity TenantID { get; }
}
