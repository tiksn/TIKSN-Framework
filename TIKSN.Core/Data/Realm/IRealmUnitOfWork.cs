namespace TIKSN.Data.Realm
{
    public interface IRealmUnitOfWork : IUnitOfWork
    {
        Realms.Realm Realm { get; }
    }
}
