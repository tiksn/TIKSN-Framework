using Autofac;
using TIKSN.Data;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.RavenDB;
using TIKSN.Data.RavenDB;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class RavenDbExchangeRateServiceTestModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        _ = builder.RegisterType<ExchangeRateDataRepository>().As<IExchangeRateDataRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ForeignExchangeDataRepository>().As<IForeignExchangeDataRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ExchangeRateRepositoryAdapter>().As<IExchangeRateRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ForeignExchangeRepositoryAdapter>().As<IForeignExchangeRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<RavenUnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
        _ = builder.RegisterType<RavenDatabaseInitializer>().As<IDatabaseInitializer>().InstancePerLifetimeScope();
    }
}
