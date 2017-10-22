using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.Advertising;
using TIKSN.Network;
using TIKSN.Settings;

namespace TIKSN.DependencyInjection
{
    public static class PlatformDependencyRegistration
    {
        public static void Register(IServiceCollection services)
        {
            DependencyRegistration.Register(services);

            services.TryAddSingleton<IAdUnitSelector, AdUnitSelector>();
            services.TryAddSingleton<INetworkConnectivityService, NetworkConnectivityService>();
            services.TryAddSingleton<ISettingsService, SettingsService>();
        }
    }
}