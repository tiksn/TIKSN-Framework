namespace TIKSN.Integration.Messages.Events;

public interface IUserEvent<out TUserIdentity>
  : IEvent
  where TUserIdentity : IEquatable<TUserIdentity>
{
    public TUserIdentity UserID { get; }
}
