using System;
using System.Collections.Concurrent;
using System.Management.Automation;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.PowerShell
{
    public class PowerShellLogger : ILogger
    {
        private readonly ICurrentCommandProvider _currentCommandProvider;
        private readonly string name;
        private readonly IOptions<PowerShellLoggerOptions> options;
        private readonly PowerShellLoggerScopeDisposable scopeDisposable;
        private readonly ConcurrentStack<object> scopes;

        public PowerShellLogger(ICurrentCommandProvider currentCommandProvider,
            IOptions<PowerShellLoggerOptions> options, string name)
        {
            this.options = options;
            this.scopes = new ConcurrentStack<object>();
            this.scopeDisposable = new PowerShellLoggerScopeDisposable(this.scopes);
            this.name = name;
            this._currentCommandProvider = currentCommandProvider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            this.scopes.Push(state);
            return this.scopeDisposable;
        }

        public bool IsEnabled(LogLevel logLevel) => this.options.Value.MinLevel >= logLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                this.WriteMessage(logLevel, eventId, message, exception);
            }
        }

        private string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Critical:
                    return logLevel.ToString();

                case LogLevel.Information:
                case LogLevel.Debug:
                case LogLevel.Warning:
                case LogLevel.Error:
                    return string.Empty;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private void WriteMessage(LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            var logBuilder = new StringBuilder();

            var logLevelString = this.GetLogLevelString(logLevel);

            if (!string.IsNullOrEmpty(logLevelString))
            {
                logBuilder.Append(logLevelString);
                logBuilder.Append(": ");
            }

            logBuilder.Append(this.name);
            logBuilder.Append("[");
            logBuilder.Append(eventId);
            logBuilder.Append("]");

            if (this.options.Value.IncludeScopes)
            {
                foreach (var scope in this.scopes)
                {
                    logBuilder.Append(" => ");
                    logBuilder.Append(scope);
                }

                if (this.scopes.Count > 0)
                {
                    logBuilder.Append(" |");
                }
            }

            logBuilder.Append(message);

            if (exception != null)
            {
                logBuilder.AppendLine();
                logBuilder.AppendLine(exception.ToString());
            }

            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Information:
                    this._currentCommandProvider.GetCurrentCommand().WriteVerbose(logBuilder.ToString());
                    break;

                case LogLevel.Debug:
                    this._currentCommandProvider.GetCurrentCommand().WriteDebug(logBuilder.ToString());
                    break;

                case LogLevel.Warning:
                    this._currentCommandProvider.GetCurrentCommand().WriteWarning(logBuilder.ToString());
                    break;

                case LogLevel.Error:
                case LogLevel.Critical:
                    this._currentCommandProvider.GetCurrentCommand().WriteError(
                        new ErrorRecord(new Exception(logBuilder.ToString(), exception), eventId.ToString(),
                            ErrorCategory.InvalidOperation, null));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }
}
