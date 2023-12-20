using Microsoft.Extensions.Logging;

namespace TIKSN.PowerShell;

public class PowerShellLoggerOptions
{
    public LogLevel MinLevel { get; set; }

    public bool IncludeScopes { get; set; }
}
