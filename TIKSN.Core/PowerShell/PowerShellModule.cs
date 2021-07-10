using Autofac;

namespace TIKSN.PowerShell
{
    public class PowerShellModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<CurrentCommandContext>().As<ICurrentCommandStore>().As<ICurrentCommandProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
