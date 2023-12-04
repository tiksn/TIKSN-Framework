using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.PowerShell;

public static class PowerShellLoggerFactoryExtensions
{
    public static ILoggerFactory AddPowerShell(this ILoggerFactory factory, IServiceProvider serviceProvider)
    {
        factory.AddProvider(new PowerShellLoggerProvider(
            serviceProvider.GetRequiredService<ICurrentCommandProvider>(),
            serviceProvider.GetRequiredService<IOptions<PowerShellLoggerOptions>>()));

        return factory;
    }
}
