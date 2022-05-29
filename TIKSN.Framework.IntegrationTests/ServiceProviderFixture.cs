using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TIKSN.Data.Mongo;
using TIKSN.Data.Mongo.IntegrationTests;
using TIKSN.DependencyInjection;
using TIKSN.Framework.IntegrationTests.Data.Mongo;

namespace TIKSN.IntegrationTests
{
    public class ServiceProviderFixture : IDisposable
    {
        private readonly IHost host;

        public ServiceProviderFixture()
        {
            this.host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => _ = services.AddFrameworkPlatform())
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    _ = builder.RegisterModule<CoreModule>();
                    _ = builder.RegisterModule<PlatformModule>();
                    _ = builder.RegisterType<TestMongoRepository>().As<ITestMongoRepository>().InstancePerLifetimeScope();
                    _ = builder.RegisterType<TestMongoDatabaseProvider>().As<IMongoDatabaseProvider>().SingleInstance();
                    _ = builder.RegisterType<TestMongoClientProvider>().As<IMongoClientProvider>().SingleInstance();
                })
                .ConfigureHostConfiguration(builder =>
                {
                    _ = builder.AddInMemoryCollection(GetInMemoryConfiguration());
                    _ = builder.AddUserSecrets<ServiceProviderFixture>();
                })
                .Build();

            static Dictionary<string, string> GetInMemoryConfiguration() => new()
            {
                {
                    "ConnectionStrings:Mongo",
                    "mongodb://localhost:27017/TIKSN_Framework_IntegrationTests?w=majority"
                }
            };
        }

        public IServiceProvider Services => this.host.Services;

        public void Dispose() => this.host?.Dispose();
    }
}
