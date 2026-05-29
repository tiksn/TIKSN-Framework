using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Testcontainers.MongoDb;
using Testcontainers.RavenDb;
using TIKSN.Data.Mongo;
using TIKSN.Data.RavenDB;
using TIKSN.DependencyInjection;
using TIKSN.Finance.ForeignExchange;
using TIKSN.IntegrationTests.Data.Mongo;
using TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;
using Xunit;

namespace TIKSN.IntegrationTests;

public class ServiceProviderFixture : IDisposable, IAsyncLifetime
{
    private const string MongoDatabaseName = "TIKSN_Framework_IntegrationTests";

    private readonly Dictionary<string, IHost> hosts;
    private readonly MongoDbContainer mongoDbContainer;
    private readonly RavenDbContainer ravenDbContainer;
    private bool disposedValue;

    public ServiceProviderFixture()
    {
        this.hosts = [];
        this.mongoDbContainer = new MongoDbBuilder("mongo:5")
            .WithReplicaSet("my-replica-set")
            .Build();
        this.ravenDbContainer = new RavenDbBuilder("ravendb/ravendb:latest")
            .Build();
    }

    public void CreateHost(string key, Action<HostBuilderContext, ContainerBuilder> configureContainer)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                _ = services.AddFrameworkCore();
                _ = services.Configure<RavenUnitOfWorkFactoryOptions>(options =>
                {
                    options.Urls = [this.ravenDbContainer.GetConnectionString(),];
                    options.Database = "TIKSN-Framework-IntegrationTests";
                });
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, builder) =>
            {
                _ = builder.RegisterModule<CoreModule>();
                _ = builder.RegisterType<TextLocalizer>().As<IStringLocalizer>().SingleInstance();
                _ = builder.RegisterType<TestMongoRepository>().As<ITestMongoRepository>().InstancePerLifetimeScope();
                _ = builder.RegisterType<TestMongoFileRepository>().As<ITestMongoFileRepository>()
                    .InstancePerLifetimeScope();
                _ = builder.RegisterType<TestMongoDatabaseProvider>().As<IMongoDatabaseProvider>().SingleInstance();
                _ = builder.RegisterType<TestMongoClientProvider>().As<IMongoClientProvider>().SingleInstance();
                _ = builder.RegisterType<TestExchangeRateService>().As<IExchangeRateService>()
                    .InstancePerLifetimeScope();

                configureContainer(context, builder);
            })
            .ConfigureHostConfiguration(builder =>
            {
                _ = builder.AddInMemoryCollection(GetInMemoryConfiguration());
                _ = builder.AddUserSecrets<ServiceProviderFixture>();
            })
            .Build();

        _ = BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        this.hosts.Add(key, host);

        Dictionary<string, string> GetInMemoryConfiguration() => new()
        {
            { "ConnectionStrings:Mongo", this.GetMongoConnectionString() },
            {
                "ConnectionStrings:LiteDB",
                "Filename=TIKSN-Framework-IntegrationTests.db;Upgrade=true;Connection=Shared"
            },
            { "ConnectionStrings:SQLite", "Data Source=TIKSN-Framework-IntegrationTests.sqlite;" },
        };
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        this.Dispose(disposing: true);

        await this.mongoDbContainer.DisposeAsync().ConfigureAwait(false);
        await this.ravenDbContainer.DisposeAsync().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    public IServiceProvider GetServiceProvider() => this.hosts[string.Empty].Services;

    public IServiceProvider GetServiceProvider(string key) => this.hosts[key].Services;

    public async ValueTask InitializeAsync()
    {
        try
        {
            await Task.WhenAll(
                this.mongoDbContainer.StartAsync(),
                this.ravenDbContainer.StartAsync()).ConfigureAwait(false);

            this.CreateHost(string.Empty, (context, builder) => { });
            this.CreateHost("LiteDB",
                (context, builder) => builder.RegisterModule<LiteDbExchangeRateServiceTestModule>());
            this.CreateHost("MongoDB",
                (context, builder) => builder.RegisterModule<MongoDbExchangeRateServiceTestModule>());
            this.CreateHost("SQLite",
                (context, builder) =>
                    builder.RegisterModule(new SQLiteExchangeRateServiceTestModule(context.Configuration)));
            this.CreateHost("RavenDB",
                (context, builder) => builder.RegisterModule<RavenDbExchangeRateServiceTestModule>());
        }
        catch
        {
            await this.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                foreach (var host in this.hosts.Values)
                {
                    host.Dispose();
                }
            }

            this.disposedValue = true;
        }
    }

    private string GetMongoConnectionString()
    {
        var connectionString = this.mongoDbContainer.GetConnectionString();
        var queryStart = connectionString.IndexOf('?');
        var baseConnectionString = queryStart < 0 ? connectionString : connectionString[..queryStart];
        var query = queryStart < 0 ? string.Empty : connectionString[(queryStart + 1)..];
        var databaseConnectionString = $"{baseConnectionString.TrimEnd('/')}/{MongoDatabaseName}";
        var parameters = string.IsNullOrWhiteSpace(query)
            ? "authSource=admin&w=majority"
            : $"{query}&authSource=admin&w=majority";

        return $"{databaseConnectionString}?{parameters}";
    }
}
