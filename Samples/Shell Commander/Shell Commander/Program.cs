using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using TIKSN.Shell;

namespace Shell_Commander
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var config = new ConfigurationRootSetup(args);
            var composition = new CompositionRootSetup(config.GetConfigurationRoot());

            var serviceProvider = composition.CreateServiceProvider();

            var engine = serviceProvider.GetRequiredService<IShellCommandEngine>();

            var thisAssembly = typeof(Program).Assembly;

            engine.AddAssembly(thisAssembly);

            await engine.RunAsync();
        }
    }
}