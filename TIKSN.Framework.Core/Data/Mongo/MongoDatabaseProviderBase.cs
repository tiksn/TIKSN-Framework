using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public abstract class MongoDatabaseProviderBase : IMongoDatabaseProvider
    {
        private readonly IConfiguration configuration;
        private readonly string connectionStringKey;
        private readonly IMongoClientProvider mongoClientProvider;

        protected MongoDatabaseProviderBase(
            IMongoClientProvider mongoClientProvider,
            IConfiguration configuration,
            string connectionStringKey)
        {
            this.mongoClientProvider =
                mongoClientProvider ?? throw new ArgumentNullException(nameof(mongoClientProvider));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.connectionStringKey =
                connectionStringKey ?? throw new ArgumentNullException(nameof(connectionStringKey));
        }

        public IMongoDatabase GetDatabase()
        {
            var mongoClient = this.mongoClientProvider.GetMongoClient();
            var connectionString = this.configuration.GetConnectionString(this.connectionStringKey);
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
