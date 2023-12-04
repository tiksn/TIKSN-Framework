namespace TIKSN.Data.RavenDB;

public interface IRavenUnitOfWorkFactory<TUnitOfWork>
    where TUnitOfWork : IUnitOfWork
{
    TUnitOfWork Create();
}
