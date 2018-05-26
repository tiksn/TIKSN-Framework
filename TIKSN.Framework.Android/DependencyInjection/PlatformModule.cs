using Autofac;
using TIKSN.Settings;

namespace TIKSN.DependencyInjection
{
    public class PlatformModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SettingsService>().As<ISettingsService>().SingleInstance();
        }
    }
}