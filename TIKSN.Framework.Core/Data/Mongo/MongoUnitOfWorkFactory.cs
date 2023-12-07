using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Data.Mongo;

public class MongoUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IMongoClientProvider mongoClientProvider;
    private readonly IServiceProvider serviceProvider;

    public MongoUnitOfWorkFactory(IMongoClientProvider mongoClientProvider, IServiceProvider serviceProvider)
    {
        this.mongoClientProvider =
            mongoClientProvider ?? throw new ArgumentNullException(nameof(mongoClientProvider));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken)
    {
        var mongoClient = this.mongoClientProvider.GetMongoClient();
        var clientSessionHandle = await mongoClient.StartSessionAsync(options: null, cancellationToken).ConfigureAwait(false);
        var serviceScope = this.serviceProvider.CreateAsyncScope();
        var mongoClientSessionStore = serviceScope.ServiceProvider.GetRequiredService<IMongoClientSessionStore>();
        mongoClientSessionStore.SetClientSessionHandle(clientSessionHandle);

        clientSessionHandle.StartTransaction();

        return new MongoUnitOfWork(clientSessionHandle, serviceScope);
    }
}
