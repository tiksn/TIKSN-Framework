using Microsoft.Extensions.DependencyInjection;
using TIKSN.Shell;

namespace TIKSN
{
    public static class DependencyRegistrationDesktop
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IConsoleService, ConsoleService>();
            services.AddSingleton<IShellCommandEngine, ShellCommandEngine>();
        }
    }
}
