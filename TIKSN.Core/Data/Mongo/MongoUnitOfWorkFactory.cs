﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public class MongoUnitOfWorkFactory : IMongoUnitOfWorkFactory
    {
        private readonly IMongoClientProvider _mongoClientProvider;
        private readonly IServiceProvider _serviceProvider;

        public MongoUnitOfWorkFactory(IMongoClientProvider mongoClientProvider, IServiceProvider serviceProvider)
        {
            _mongoClientProvider = mongoClientProvider ?? throw new ArgumentNullException(nameof(mongoClientProvider));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        
        public async Task<IMongoUnitOfWork> CreateAsync(CancellationToken cancellationToken)
        {
            var mongoClient = _mongoClientProvider.GetMongoClient();
            var clientSessionHandle = await mongoClient.StartSessionAsync(options: null, cancellationToken: cancellationToken);
            var serviceScope = _serviceProvider.CreateScope();
            var mongoClientSessionStore = serviceScope.ServiceProvider.GetRequiredService<IMongoClientSessionStore>();
            mongoClientSessionStore.SetClientSessionHandle(clientSessionHandle);

            clientSessionHandle.StartTransaction();

            return new MongoUnitOfWork(clientSessionHandle, serviceScope);
        }
    }
}