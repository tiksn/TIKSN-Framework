using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;

namespace TIKSN.Data.RavenDB;

public class RavenUnitOfWorkFactory : IUnitOfWorkFactory, IDisposable
{
    private readonly IServiceProvider serviceProvider;
    private readonly IDocumentStore store;
    private bool disposed;

    public RavenUnitOfWorkFactory(
        IServiceProvider serviceProvider,
        IOptions<RavenUnitOfWorkFactoryOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.store = new DocumentStore { Urls = [.. options.Value.Urls], Database = options.Value.Database };
        this.store = this.store.Initialize();
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken)
    {
        var serviceScope = this.serviceProvider.CreateAsyncScope();
        return Task.FromResult<IUnitOfWork>(new RavenUnitOfWork(this.store, serviceScope));
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed && disposing)
        {
            this.store.Dispose();
        }
        this.disposed = true;
    }
}
