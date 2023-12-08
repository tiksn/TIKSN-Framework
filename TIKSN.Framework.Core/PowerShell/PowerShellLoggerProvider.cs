using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.PowerShell;

public class PowerShellLoggerProvider : ILoggerProvider
{
    private readonly ICurrentCommandProvider currentCommandProvider;
    private readonly ConcurrentDictionary<string, PowerShellLogger> loggers = new(StringComparer.Ordinal);
    private readonly IOptions<PowerShellLoggerOptions> options;

    public PowerShellLoggerProvider(
        ICurrentCommandProvider currentCommandProvider,
        IOptions<PowerShellLoggerOptions> options)
    {
        this.options = options;
        this.currentCommandProvider = currentCommandProvider ??
                                       throw new ArgumentNullException(nameof(currentCommandProvider));
    }

    public ILogger CreateLogger(string categoryName) =>
        this.loggers.GetOrAdd(categoryName, this.CreateLoggerImplementation);

    public void Dispose()
    {
    }

    private PowerShellLogger CreateLoggerImplementation(string name) =>
        new(this.currentCommandProvider, this.options, name);
}
