using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Management.Automation;
using System.Text;

namespace TIKSN.PowerShell
{
    public class PowerShellLogger : ILogger
    {
        private readonly Cmdlet cmdlet;
        private readonly string name;
        private readonly IOptions<PowerShellLoggerOptions> options;
        private readonly PowerShellLoggerScopeDisposable scopeDisposable;
        private readonly ConcurrentStack<object> scopes;

        public PowerShellLogger(Cmdlet cmdlet, IOptions<PowerShellLoggerOptions> options, string name)
        {
            this.options = options;
            scopes = new ConcurrentStack<object>();
            scopeDisposable = new PowerShellLoggerScopeDisposable(scopes);
            this.name = name;
            this.cmdlet = cmdlet;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            scopes.Push(state);
            return scopeDisposable;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return options.Value.MinLevel >= logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
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
                WriteMessage(logLevel, eventId, message, exception);
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

            var logLevelString = GetLogLevelString(logLevel);

            if (!string.IsNullOrEmpty(logLevelString))
            {
                logBuilder.Append(logLevelString);
                logBuilder.Append(": ");
            }

            logBuilder.Append(name);
            logBuilder.Append("[");
            logBuilder.Append(eventId);
            logBuilder.Append("]");

            if (options.Value.IncludeScopes)
            {
                foreach (var scope in scopes)
                {
                    logBuilder.Append(" => ");
                    logBuilder.Append(scope);
                }

                if (scopes.Count > 0)
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
                    cmdlet.WriteVerbose(logBuilder.ToString());
                    break;

                case LogLevel.Debug:
                    cmdlet.WriteDebug(logBuilder.ToString());
                    break;

                case LogLevel.Warning:
                    cmdlet.WriteWarning(logBuilder.ToString());
                    break;

                case LogLevel.Error:
                case LogLevel.Critical:
                    cmdlet.WriteError(new ErrorRecord(new Exception(logBuilder.ToString(), exception), eventId.ToString(), ErrorCategory.InvalidOperation, null));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }
}