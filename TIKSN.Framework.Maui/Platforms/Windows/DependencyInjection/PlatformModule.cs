using Autofac;
using TIKSN.Security.Antimalware;

namespace TIKSN.DependencyInjection;

public class PlatformModule : PlatformModuleBase
{
    protected override void Load(ContainerBuilder builder) => _ = builder.RegisterType<AntimalwareScanner>().As<IAntimalwareScanner>().InstancePerLifetimeScope();
}
