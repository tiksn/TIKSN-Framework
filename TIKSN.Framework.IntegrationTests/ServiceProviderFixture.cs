using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using TIKSN.Data.Mongo;
using TIKSN.Data.RavenDB;
using TIKSN.DependencyInjection;
using TIKSN.Finance.ForeignExchange;
using TIKSN.IntegrationTests.Data.Mongo;
using TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

namespace TIKSN.IntegrationTests;

public class ServiceProviderFixture : IDisposable
{
    private readonly Dictionary<string, IHost> hosts;
    private bool disposedValue;

    public ServiceProviderFixture()
    {
        this.hosts = [];

        this.CreateHost(string.Empty, (context, builder) => { });
        this.CreateHost("LiteDB", (context, builder) => builder.RegisterModule<LiteDbExchangeRateServiceTestModule>());
        this.CreateHost("MongoDB", (context, builder) => builder.RegisterModule<MongoDbExchangeRateServiceTestModule>());
        this.CreateHost("SQLite", (context, builder) => builder.RegisterModule(new SQLiteExchangeRateServiceTestModule(context.Configuration)));
        this.CreateHost("RavenDB", (context, builder) => builder.RegisterModule<RavenDbExchangeRateServiceTestModule>());
    }

    public void CreateHost(string key, Action<HostBuilderContext, ContainerBuilder> configureContainer)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                _ = services.AddFrameworkCore();
                _ = services.Configure<RavenUnitOfWorkFactoryOptions>(options =>
                {
                    options.Urls = ["http://localhost:8080"];
                    options.Database = "TIKSN-Framework-IntegrationTests";
                });
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, builder) =>
            {
                _ = builder.RegisterModule<CoreModule>();
                _ = builder.RegisterType<TextLocalizer>().As<IStringLocalizer>().SingleInstance();
                _ = builder.RegisterType<TestMongoRepository>().As<ITestMongoRepository>().InstancePerLifetimeScope();
                _ = builder.RegisterType<TestMongoFileRepository>().As<ITestMongoFileRepository>().InstancePerLifetimeScope();
                _ = builder.RegisterType<TestMongoDatabaseProvider>().As<IMongoDatabaseProvider>().SingleInstance();
                _ = builder.RegisterType<TestMongoClientProvider>().As<IMongoClientProvider>().SingleInstance();
                _ = builder.RegisterType<TestExchangeRateService>().As<IExchangeRateService>().InstancePerLifetimeScope();

                configureContainer(context, builder);
            })
            .ConfigureHostConfiguration(builder =>
            {
                _ = builder.AddInMemoryCollection(GetInMemoryConfiguration());
                _ = builder.AddUserSecrets<ServiceProviderFixture>();
            })
            .Build();

        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        this.hosts.Add(key, host);

        static Dictionary<string, string> GetInMemoryConfiguration() => new()
        {
            {
                "ConnectionStrings:Mongo",
                "mongodb://localhost:27017/TIKSN_Framework_IntegrationTests?w=majority"
            },
            {
                "ConnectionStrings:LiteDB",
                "Filename=TIKSN-Framework-IntegrationTests.db;Upgrade=true;Connection=Shared"
            },
            {
                "ConnectionStrings:SQLite",
                "Data Source=TIKSN-Framework-IntegrationTests.sqlite;"
            },
        };
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public IServiceProvider GetServiceProvider() => this.hosts[string.Empty].Services;

    public IServiceProvider GetServiceProvider(string key) => this.hosts[key].Services;

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
}
