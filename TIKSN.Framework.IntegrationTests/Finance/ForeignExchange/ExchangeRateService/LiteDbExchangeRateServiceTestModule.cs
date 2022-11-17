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
            _ = builder.RegisterType<ExchangeRateDataRepository>().As<IExchangeRateDataRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ForeignExchangeDataRepository>().As<IForeignExchangeDataRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<TestLiteDbDatabaseProvider>().As<ILiteDbDatabaseProvider>().SingleInstance();
            _ = builder.RegisterType<NullUnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
        }
    }
}
