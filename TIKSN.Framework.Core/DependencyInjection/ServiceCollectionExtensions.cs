using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.FileSystem;

namespace TIKSN.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFrameworkPlatform(this IServiceCollection services)
        {
            _ = services.AddFrameworkCore();

            services.TryAddSingleton<IKnownFolders, KnownFolders>();

            return services;
        }
    }
}
