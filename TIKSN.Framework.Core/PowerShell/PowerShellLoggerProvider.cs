using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.PowerShell
{
    public class PowerShellLoggerProvider : ILoggerProvider
    {
        private readonly ICurrentCommandProvider _currentCommandProvider;
        private readonly ConcurrentDictionary<string, PowerShellLogger> _loggers = new(StringComparer.Ordinal);
        private readonly IOptions<PowerShellLoggerOptions> options;

        public PowerShellLoggerProvider(ICurrentCommandProvider currentCommandProvider,
            IOptions<PowerShellLoggerOptions> options)
        {
            this.options = options;
            this._currentCommandProvider = currentCommandProvider ??
                                           throw new ArgumentNullException(nameof(currentCommandProvider));
        }

        public ILogger CreateLogger(string categoryName) =>
            this._loggers.GetOrAdd(categoryName, this.CreateLoggerImplementation);

        public void Dispose()
        {
        }

        private PowerShellLogger CreateLoggerImplementation(string name) =>
            new(this._currentCommandProvider, this.options, name);
    }
}
