using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.PowerShell;

public static class PowerShellLoggerFactoryExtensions
{
    public static ILoggerFactory AddPowerShell(
        this ILoggerFactory factory,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(serviceProvider);

#pragma warning disable CA2000 // Dispose objects before losing scope
        var provider = new PowerShellLoggerProvider(
                    serviceProvider.GetRequiredService<ICurrentCommandProvider>(),
                    serviceProvider.GetRequiredService<IOptions<PowerShellLoggerOptions>>());
#pragma warning restore CA2000 // Dispose objects before losing scope

        factory.AddProvider(provider);

        return factory;
    }
}
