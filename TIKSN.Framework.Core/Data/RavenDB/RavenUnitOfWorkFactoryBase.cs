using Microsoft.Extensions.Options;
using Raven.Client.Documents;

namespace TIKSN.Data.RavenDB;

public abstract class RavenUnitOfWorkFactoryBase<TUnitOfWork> : IRavenUnitOfWorkFactory<TUnitOfWork>, IDisposable
    where TUnitOfWork : IUnitOfWork
{
    private readonly IDocumentStore _store;

    protected RavenUnitOfWorkFactoryBase(IOptions<RavenUnitOfWorkFactoryOptions<TUnitOfWork>> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        this._store = new DocumentStore { Urls = options.Value.Urls, Database = options.Value.Database }.Initialize();
    }

    public void Dispose() => this._store.Dispose();

    public TUnitOfWork Create() => this.Create(this._store);

    protected abstract TUnitOfWork Create(IDocumentStore store);
}
