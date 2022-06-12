using Autofac;
using TIKSN.Data;
using TIKSN.Data.LiteDB;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.LiteDB;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    public class LiteDbExchangeRateServiceTestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterType<ExchangeRateRepository>().As<IExchangeRateRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ForeignExchangeRepository>().As<IForeignExchangeRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<TestLiteDbDatabaseProvider>().As<ILiteDbDatabaseProvider>().SingleInstance();
            _ = builder.RegisterType<NullUnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
        }
    }
}
