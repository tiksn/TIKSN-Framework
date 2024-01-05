using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Data;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.EntityFrameworkCore;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class SQLiteExchangeRateServiceTestModule : Module
{
    private readonly IConfiguration configuration;

    public SQLiteExchangeRateServiceTestModule(IConfiguration configuration)
        => this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    protected override void Load(ContainerBuilder builder)
    {
        _ = builder.RegisterType<ExchangeRateDataRepository>().As<IExchangeRateDataRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ForeignExchangeDataRepository>().As<IForeignExchangeDataRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ExchangeRateRepositoryAdapter>().As<IExchangeRateRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ForeignExchangeRepositoryAdapter>().As<IForeignExchangeRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<EntityUnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
        _ = builder.RegisterType<EntityDatabaseInitializer>().As<IDatabaseInitializer>().InstancePerLifetimeScope();

        this.Populate(builder);
    }

    private void Populate(ContainerBuilder builder)
    {
        var services = new ServiceCollection();
        _ = services.AddSqlite<ExchangeRatesContext>(this.configuration.GetConnectionString("SQLite"));
        builder.Populate(services);
    }
}
