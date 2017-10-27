using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Management.Automation;

namespace TIKSN.PowerShell
{
    public class PowerShellLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, PowerShellLogger> _loggers = new ConcurrentDictionary<string, PowerShellLogger>();
        private readonly Cmdlet cmdlet;
        private readonly IOptions<PowerShellLoggerOptions> options;

        public PowerShellLoggerProvider(Cmdlet cmdlet, IOptions<PowerShellLoggerOptions> options)
        {
            this.cmdlet = cmdlet;
            this.options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        public void Dispose()
        {
        }

        private PowerShellLogger CreateLoggerImplementation(string name)
        {
            return new PowerShellLogger(cmdlet, options, name);
        }
    }
}