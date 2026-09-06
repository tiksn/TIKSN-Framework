namespace TIKSN.Integration.Messages;

public class EntityReference<TEntityIdentity> where TEntityIdentity : IEquatable<TEntityIdentity>
{
    public EntityReference(Type entityType, TEntityIdentity entityID)
    {
        this.EntityType = entityType;
        this.EntityID = entityID;
    }

    public TEntityIdentity EntityID { get; }

    public Type EntityType { get; }
}
