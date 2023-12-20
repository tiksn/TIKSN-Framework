using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using TIKSN.Data.Mongo;
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
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

        this.hosts = [];

        this.CreateHost(string.Empty, builder => { });
        this.CreateHost("LiteDB", builder => builder.RegisterModule<LiteDbExchangeRateServiceTestModule>());
        this.CreateHost("MongoDB", builder => builder.RegisterModule<MongoDbExchangeRateServiceTestModule>());
    }

    public void CreateHost(string key, Action<ContainerBuilder> configureContainer)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => _ = services.AddFrameworkCore())
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                _ = builder.RegisterModule<CoreModule>();
                _ = builder.RegisterType<TextLocalizer>().As<IStringLocalizer>().SingleInstance();
                _ = builder.RegisterType<TestMongoRepository>().As<ITestMongoRepository>().InstancePerLifetimeScope();
                _ = builder.RegisterType<TestMongoDatabaseProvider>().As<IMongoDatabaseProvider>().SingleInstance();
                _ = builder.RegisterType<TestMongoClientProvider>().As<IMongoClientProvider>().SingleInstance();
                _ = builder.RegisterType<TestExchangeRateService>().As<IExchangeRateService>().InstancePerLifetimeScope();

                configureContainer(builder);
            })
            .ConfigureHostConfiguration(builder =>
            {
                _ = builder.AddInMemoryCollection(GetInMemoryConfiguration());
                _ = builder.AddUserSecrets<ServiceProviderFixture>();
            })
            .Build();

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
            }
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
