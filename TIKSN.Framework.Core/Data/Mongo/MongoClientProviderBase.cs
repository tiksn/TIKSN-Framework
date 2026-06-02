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
        var client = this.mongoClient;
        if (client is null)
        {
            lock (this.locker)
            {
                client = this.mongoClient;
                if (client is null)
                {
                    var connectionString = this.configuration.GetConnectionString(this.connectionStringKey);
                    var mongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);
                    this.ConfigureClientSettings(mongoClientSettings);
                    client = new MongoClient(mongoClientSettings);
                    this.mongoClient = client;
                }
            }
        }

        return client;
    }

    protected virtual void ConfigureClientSettings(MongoClientSettings mongoClientSettings)
    {
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.mongoClient?.Dispose();
            }

            this.disposedValue = true;
        }
    }
}
