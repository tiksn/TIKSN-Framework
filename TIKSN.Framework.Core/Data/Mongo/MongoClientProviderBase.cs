using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public abstract class MongoClientProviderBase : IMongoClientProvider
{
    private readonly IConfiguration configuration;
    private readonly string connectionStringKey;
    private readonly object locker;
    private MongoClient mongoClient;

    protected MongoClientProviderBase(
        IConfiguration configuration,
        string connectionStringKey)
    {
        this.locker = new object();
        this.configuration = configuration;
        this.connectionStringKey = connectionStringKey;
    }

    public IMongoClient GetMongoClient()
    {
        if (this.mongoClient == null)
        {
            lock (this.locker)
            {
                if (this.mongoClient == null)
                {
                    var connectionString = this.configuration.GetConnectionString(this.connectionStringKey);
                    var mongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);
                    this.ConfigureClientSettings(mongoClientSettings);
                    this.mongoClient = new MongoClient(mongoClientSettings);
                }
            }
        }

        return this.mongoClient;
    }

    protected virtual void ConfigureClientSettings(MongoClientSettings mongoClientSettings)
    {
    }
}
