using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TIKSN.Data.Mongo;

public abstract class MongoClientProviderBase : IMongoClientProvider, IDisposable
{
    private readonly IConfiguration configuration;
    private readonly string connectionStringKey;
    private readonly Lock locker;
    private bool disposedValue;
    private MongoClient? mongoClient;

    protected MongoClientProviderBase(
        IConfiguration configuration,
        string connectionStringKey)
    {
        this.locker = new Lock();
        this.configuration = configuration;
        this.connectionStringKey = connectionStringKey;
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
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

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            this.disposedValue = true;
        }
    }
}
