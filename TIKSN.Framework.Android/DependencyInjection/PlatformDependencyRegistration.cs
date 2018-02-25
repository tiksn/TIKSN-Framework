using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.FileSystem;

namespace TIKSN.DependencyInjection
{
    public static class PlatformDependencyRegistration
    {
        public static void Register(IServiceCollection services)
        {
            DependencyRegistration.Register(services);

            services.TryAddSingleton<IKnownFolders, KnownFolders>();
        }
    }
}