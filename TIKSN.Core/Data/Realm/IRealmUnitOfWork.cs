namespace TIKSN.Data.Realm
{
    public interface IRealmUnitOfWork
    {
        Realms.Realm Realm { get; }
    }
}
