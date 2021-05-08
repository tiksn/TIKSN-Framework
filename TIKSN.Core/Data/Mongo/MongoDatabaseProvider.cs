using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public class MongoDatabaseProvider : IMongoDatabaseProvider
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionStringKey;

        public MongoDatabaseProvider(IConfiguration configuration, string connectionStringKey)
        {
            _configuration = configuration;
            _connectionStringKey = connectionStringKey;
        }

        public IMongoDatabase GetDatabase()
        {
            var connectionString = _configuration.GetConnectionString(_connectionStringKey);
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            return new MongoClient(connectionString).GetDatabase(databaseName);
        }
    }
}