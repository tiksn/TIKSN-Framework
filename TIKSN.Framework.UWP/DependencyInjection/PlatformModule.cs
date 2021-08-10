using Autofac;
using TIKSN.Advertising;
using TIKSN.Network;
using TIKSN.Settings;

namespace TIKSN.DependencyInjection
{
    public class PlatformModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AdUnitSelector>().As<IAdUnitSelector>().SingleInstance();
            builder.RegisterType<NetworkConnectivityService>().As<INetworkConnectivityService>().SingleInstance();
            builder.RegisterType<SettingsService>().As<ISettingsService>().SingleInstance();
        }
    }
}
