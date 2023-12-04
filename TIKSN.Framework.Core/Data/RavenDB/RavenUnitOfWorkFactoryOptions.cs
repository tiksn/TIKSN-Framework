namespace TIKSN.Data.RavenDB;

public class RavenUnitOfWorkFactoryOptions<TUnitOfWork>
    where TUnitOfWork : IUnitOfWork
{
    public string[] Urls { get; set; }

    public string Database { get; set; }
}
