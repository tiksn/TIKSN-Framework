using Autofac;
using TIKSN.Data;
using TIKSN.Data.Mongo;
using TIKSN.Finance.ForeignExchange.Data;
using TIKSN.Finance.ForeignExchange.Data.Mongo;

namespace TIKSN.Finance.ForeignExchange.ExchangeRateService.IntegrationTests
{
    public class MongoDbExchangeRateServiceTestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterType<ExchangeRateRepository>().As<IExchangeRateRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<ForeignExchangeRepository>().As<IForeignExchangeRepository>().InstancePerLifetimeScope();
            _ = builder.RegisterType<MongoUnitOfWorkFactory>().As<IUnitOfWorkFactory>().InstancePerLifetimeScope();
        }
    }
}
