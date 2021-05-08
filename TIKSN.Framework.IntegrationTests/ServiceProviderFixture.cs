using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TIKSN.Data.Mongo;
using TIKSN.Framework.IntegrationTests.Data.Mongo;

namespace TIKSN.Framework.IntegrationTests
{
    public class ServiceProviderFixture : IDisposable
    {
        private readonly IHost host;

        public ServiceProviderFixture()
        {
            host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => { })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterType<TestMongoRepository>().As<ITestMongoRepository>().InstancePerLifetimeScope();
                    builder.RegisterType<TestMongoDatabaseProvider>().As<IMongoDatabaseProvider>().SingleInstance();
                })
                .ConfigureHostConfiguration(builder => { builder.AddInMemoryCollection(GetInMemoryConfiguration()); })
                .Build();

            static Dictionary<string, string> GetInMemoryConfiguration()
            {
                return new()
                {
                    {"ConnectionStrings:Mongo", "mongodb://root:0b6273775d@localhost:27017/TIKSN_Framework_IntegrationTests?authSource=admin"}
                };
            }
        }

        public IServiceProvider Services => host.Services;

        public void Dispose()
        {
            host?.Dispose();
        }
    }
}