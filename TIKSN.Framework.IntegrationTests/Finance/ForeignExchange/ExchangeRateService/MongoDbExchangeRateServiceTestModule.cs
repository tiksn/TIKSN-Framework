using Autofac;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.Mongo;

namespace TIKSN.IntegrationTests.Finance.ForeignExchange.ExchangeRateService;

public class MongoDbExchangeRateServiceTestModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        _ = builder.RegisterType<ExchangeRateDataRepository>().As<IExchangeRateDataRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ForeignExchangeDataRepository>().As<IForeignExchangeDataRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ExchangeRateRepositoryAdapter>().As<IExchangeRateRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<ForeignExchangeRepositoryAdapter>().As<IForeignExchangeRepository>().InstancePerLifetimeScope();
        _ = builder.RegisterType<MongoUnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
        _ = builder.RegisterType<NullDatabaseInitializer>().As<IDatabaseInitializer>().InstancePerLifetimeScope();
    }
}
