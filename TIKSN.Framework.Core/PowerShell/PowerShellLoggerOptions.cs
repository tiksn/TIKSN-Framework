using Microsoft.Extensions.Logging;

namespace TIKSN.PowerShell;

public class PowerShellLoggerOptions
{
    public bool IncludeScopes { get; set; }
    public LogLevel MinLevel { get; set; }
}
