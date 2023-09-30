using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.FileSystem;
using TIKSN.Shell;

namespace TIKSN.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFrameworkPlatform(
            this IServiceCollection services)
        {
            _ = services.AddFrameworkCore();

            services.TryAddSingleton<IConsoleService, ConsoleService>();
            services.TryAddSingleton<IKnownFolders, KnownFolders>();

            return services;
        }
    }
}
