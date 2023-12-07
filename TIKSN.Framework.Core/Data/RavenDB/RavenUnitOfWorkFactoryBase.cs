using Microsoft.Extensions.Options;
using Raven.Client.Documents;

namespace TIKSN.Data.RavenDB;

public abstract class RavenUnitOfWorkFactoryBase<TUnitOfWork> : IRavenUnitOfWorkFactory<TUnitOfWork>, IDisposable
    where TUnitOfWork : IUnitOfWork
{
    private readonly IDocumentStore store;
    private bool disposed;

    protected RavenUnitOfWorkFactoryBase(IOptions<RavenUnitOfWorkFactoryOptions<TUnitOfWork>> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.store = new DocumentStore { Urls = options.Value.Urls, Database = options.Value.Database };
        this.store = this.store.Initialize();
    }

    public TUnitOfWork Create() => this.Create(this.store);

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected abstract TUnitOfWork Create(IDocumentStore store);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed && disposing)
        {
            this.store.Dispose();
        }
        this.disposed = true;
    }
}
