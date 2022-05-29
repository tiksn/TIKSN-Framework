using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using TIKSN.Data;
using TIKSN.Data.LiteDB;
using TIKSN.Data.Mongo;
using TIKSN.DependencyInjection;
using TIKSN.FileSystem;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.LiteDB;
using TIKSN.IntegrationTests;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    public class LiteDbServiceProviderFixture : IDisposable
    {
        private readonly IHost host;

        public LiteDbServiceProviderFixture()
        {
            this.host = Host.CreateDefaultBuilder()
                .ConfigureServices(services => _ = services.AddFrameworkPlatform())
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    _ = builder.RegisterModule<CoreModule>();
                    _ = builder.RegisterModule<PlatformModule>();
                    _ = builder.RegisterType<ExchangeRateRepository>().As<IExchangeRateRepository>().InstancePerLifetimeScope();
                    _ = builder.RegisterType<ForeignExchangeRepository>().As<IForeignExchangeRepository>().InstancePerLifetimeScope();
                    _ = builder.RegisterType<TestLiteDbDatabaseProvider>().As<ILiteDbDatabaseProvider>().SingleInstance();
                    _ = builder.RegisterType<TestUnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
                    _ = builder.RegisterType<TestExchangeRateService>().As<IExchangeRateService>().InstancePerLifetimeScope();
                    _ = builder.RegisterType<TextLocalizer>().As<IStringLocalizer>().SingleInstance();
                    _ = builder.RegisterInstance(new KnownFoldersConfiguration(this.GetType().Assembly, KnownFolderVersionConsideration.None));
                })
                .ConfigureHostConfiguration(builder =>
                {
                    _ = builder.AddInMemoryCollection(GetInMemoryConfiguration());
                    _ = builder.AddUserSecrets<LiteDbServiceProviderFixture>();
                })
                .Build();

            static Dictionary<string, string> GetInMemoryConfiguration() => new()
            {
                {
                    "ConnectionStrings:LiteDB",
                    "Filename=TIKSN-Framework-IntegrationTests.db;Upgrade=true;Connection=Shared"
                }
            };
        }

        public IServiceProvider Services => this.host.Services;

        public void Dispose() => this.host?.Dispose();
    }
}
