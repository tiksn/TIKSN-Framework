using Autofac;
using TIKSN.DependencyInjection;
using TIKSN.Platforms.Windows.Security.Antimalware;
using TIKSN.Security.Antimalware;

namespace TIKSN.Platforms.Windows.DependencyInjection;

public class PlatformModule : PlatformModuleBase
{
    protected override void Load(ContainerBuilder builder) => _ = builder.RegisterType<AntimalwareScanner>().As<IAntimalwareScanner>().InstancePerLifetimeScope();
}
