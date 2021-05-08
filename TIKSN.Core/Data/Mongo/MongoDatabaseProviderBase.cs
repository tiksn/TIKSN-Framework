using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public abstract class MongoDatabaseProviderBase : IMongoDatabaseProvider
    {
        private readonly IMongoClientProvider _mongoClientProvider;
        private readonly IConfiguration _configuration;
        private readonly string _connectionStringKey;

        public MongoDatabaseProviderBase(IMongoClientProvider mongoClientProvider, IConfiguration configuration, string connectionStringKey)
        {
            _mongoClientProvider = mongoClientProvider ?? throw new ArgumentNullException(nameof(mongoClientProvider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionStringKey = connectionStringKey ?? throw new ArgumentNullException(nameof(connectionStringKey));
        }

        public IMongoDatabase GetDatabase()
        {
            var mongoClient = _mongoClientProvider.GetMongoClient();
            var connectionString = _configuration.GetConnectionString(_connectionStringKey);
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            return mongoClient.GetDatabase(databaseName);
        }
    }
}