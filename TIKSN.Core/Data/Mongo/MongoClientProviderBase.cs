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
            this._locker = new object();
            this._configuration = configuration;
            this._connectionStringKey = connectionStringKey;
        }

        public IMongoClient GetMongoClient()
        {
            if (this._mongoClient == null)
            {
                lock (this._locker)
                {
                    if (this._mongoClient == null)
                    {
                        var connectionString = this._configuration.GetConnectionString(this._connectionStringKey);
                        var mongoUrl = MongoUrl.Create(connectionString);
                        this._mongoClient = new MongoClient(mongoUrl);
                    }
                }
            }

            return this._mongoClient;
        }
    }
}
