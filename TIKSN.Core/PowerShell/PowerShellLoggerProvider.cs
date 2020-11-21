using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace TIKSN.PowerShell
{
    public class PowerShellLoggerProvider : ILoggerProvider
    {
        private readonly ICurrentCommandProvider _currentCommandProvider;
        private readonly ConcurrentDictionary<string, PowerShellLogger> _loggers = new ConcurrentDictionary<string, PowerShellLogger>();
        private readonly IOptions<PowerShellLoggerOptions> options;

        public PowerShellLoggerProvider(ICurrentCommandProvider currentCommandProvider, IOptions<PowerShellLoggerOptions> options)
        {
            this.options = options;
            _currentCommandProvider = currentCommandProvider ?? throw new ArgumentNullException(nameof(currentCommandProvider));
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
            return new PowerShellLogger(_currentCommandProvider, options, name);
        }
    }
}