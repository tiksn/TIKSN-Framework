using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.Advertising;
using TIKSN.FileSystem;
using TIKSN.Network;
using TIKSN.Settings;

namespace TIKSN.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFrameworkPlatform(this IServiceCollection services)
        {
            services.AddFrameworkCore();

            services.TryAddSingleton<IAdUnitSelector, AdUnitSelector>();
            services.TryAddSingleton<IKnownFolders, KnownFolders>();
            services.TryAddSingleton<INetworkConnectivityService, NetworkConnectivityService>();
            services.TryAddSingleton<ISettingsService, SettingsService>();

            return services;
        }
    }
}
