namespace TIKSN.Data;

public interface IUserEntity<TEntityIdentity, out TUserIdentity>
    : IEntity<TEntityIdentity>
    where TEntityIdentity : IEquatable<TEntityIdentity>
    where TUserIdentity : IEquatable<TUserIdentity>
{
    TUserIdentity UserID { get; }
}
