namespace TIKSN.Integration.Messages.Commands;

public interface IUserCommand<out TUserIdentity>
  : ICommand
  where TUserIdentity : IEquatable<TUserIdentity>
{
    public TUserIdentity UserID { get; }
}
