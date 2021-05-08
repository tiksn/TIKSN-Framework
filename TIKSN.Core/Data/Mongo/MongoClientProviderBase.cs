using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo
{
    public abstract class MongoClientProviderBase : IMongoClientProvider
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionStringKey;
        private readonly object _locker;
        private MongoClient _mongoClient;

        protected MongoClientProviderBase(IConfiguration configuration, string connectionStringKey)
        {
            _locker = new object();
            _configuration = configuration;
            _connectionStringKey = connectionStringKey;
        }

        public IMongoClient GetMongoClient()
        {
            if (_mongoClient == null)
            {
                lock (_locker)
                {
                    if (_mongoClient == null)
                    {
                        var connectionString = _configuration.GetConnectionString(_connectionStringKey);
                        var mongoUrl = MongoUrl.Create(connectionString);
                        _mongoClient = new MongoClient(mongoUrl);
                    }
                }
            }

            return _mongoClient;
        }
    }
}