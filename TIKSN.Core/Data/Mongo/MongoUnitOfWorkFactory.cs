using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.Data.Mongo
{
    public class MongoUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IMongoClientProvider _mongoClientProvider;
        private readonly IServiceProvider _serviceProvider;

        public MongoUnitOfWorkFactory(IMongoClientProvider mongoClientProvider, IServiceProvider serviceProvider)
        {
            this._mongoClientProvider =
                mongoClientProvider ?? throw new ArgumentNullException(nameof(mongoClientProvider));
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<IUnitOfWork> CreateAsync(CancellationToken cancellationToken)
        {
            var mongoClient = this._mongoClientProvider.GetMongoClient();
            var clientSessionHandle = await mongoClient.StartSessionAsync(null, cancellationToken).ConfigureAwait(false);
            var serviceScope = this._serviceProvider.CreateScope();
            var mongoClientSessionStore = serviceScope.ServiceProvider.GetRequiredService<IMongoClientSessionStore>();
            mongoClientSessionStore.SetClientSessionHandle(clientSessionHandle);

            clientSessionHandle.StartTransaction();

            return new MongoUnitOfWork(clientSessionHandle, serviceScope);
        }
    }
}
