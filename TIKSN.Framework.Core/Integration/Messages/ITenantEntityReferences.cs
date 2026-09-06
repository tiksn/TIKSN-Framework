namespace TIKSN.Integration.Messages;

public interface ITenantEntityReferences<TEntityIdentity>
    where TEntityIdentity : IEquatable<TEntityIdentity>
{
    public IEnumerable<EntityReference<TEntityIdentity>> TenantEntityReferences { get; }
}
