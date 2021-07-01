using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TIKSN.Data.Mongo;
using TIKSN.DependencyInjection;
using TIKSN.Framework.IntegrationTests.Data.Mongo;

namespace TIKSN.Framework.IntegrationTests
{
    public class ServiceProviderFixture : IDisposable
    {
        private readonly IHost host;

        public ServiceProviderFixture()
        {
            this.host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddFrameworkPlatform();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule<CoreModule>();
                    builder.RegisterModule<PlatformModule>();
                    builder.RegisterType<TestMongoRepository>().As<ITestMongoRepository>().InstancePerLifetimeScope();
                    builder.RegisterType<TestMongoDatabaseProvider>().As<IMongoDatabaseProvider>().SingleInstance();
                    builder.RegisterType<TestMongoClientProvider>().As<IMongoClientProvider>().SingleInstance();
                })
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddInMemoryCollection(GetInMemoryConfiguration());
                    builder.AddUserSecrets<ServiceProviderFixture>();
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
