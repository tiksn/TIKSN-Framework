using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public abstract class MongoDatabaseProviderBase : IMongoDatabaseProvider
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionStringKey;
        private readonly IMongoClientProvider _mongoClientProvider;

        public MongoDatabaseProviderBase(IMongoClientProvider mongoClientProvider, IConfiguration configuration,
            string connectionStringKey)
        {
            this._mongoClientProvider =
                mongoClientProvider ?? throw new ArgumentNullException(nameof(mongoClientProvider));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._connectionStringKey =
                connectionStringKey ?? throw new ArgumentNullException(nameof(connectionStringKey));
        }

        public IMongoDatabase GetDatabase()
        {
            var mongoClient = this._mongoClientProvider.GetMongoClient();
            var connectionString = this._configuration.GetConnectionString(this._connectionStringKey);
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
