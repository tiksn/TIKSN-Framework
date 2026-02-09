namespace TIKSN.Integration.Messages.Queries;

public interface IUserQuery<out TUserIdentity, out TResult>
  : IQuery<TResult>
  where TUserIdentity : IEquatable<TUserIdentity>
{
    public TUserIdentity UserID { get; }
}
