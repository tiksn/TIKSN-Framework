using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TIKSN.DependencyInjection
{
    public abstract class PlatformCompositionRootSetupBase : CompositionRootSetupBase
    {
        protected PlatformCompositionRootSetupBase(IConfigurationRoot configurationRoot) : base(configurationRoot)
        {
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            PlatformDependencyRegistration.Register(services);
        }
    }
}