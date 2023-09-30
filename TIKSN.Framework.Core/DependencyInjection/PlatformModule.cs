using Autofac;

namespace TIKSN.DependencyInjection;

public class PlatformModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<TIKSN.Finance.ForeignExchange.EntityFrameworkCore.DataEntityMapper>()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<TIKSN.Finance.ForeignExchange.LiteDB.DataEntityMapper>()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<TIKSN.Finance.ForeignExchange.Mongo.DataEntityMapper>()
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}
