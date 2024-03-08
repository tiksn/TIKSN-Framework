using System.Collections.Concurrent;
using System.Management.Automation;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.PowerShell;

public class PowerShellLogger : ILogger, IDisposable
{
    private readonly ICurrentCommandProvider currentCommandProvider;
    private readonly string name;
    private readonly IOptions<PowerShellLoggerOptions> options;
    private readonly PowerShellLoggerScopeDisposable scopeDisposable;
    private readonly ConcurrentStack<object> scopes;
    private bool disposedValue;

    public PowerShellLogger(
        ICurrentCommandProvider currentCommandProvider,
        IOptions<PowerShellLoggerOptions> options,
        string name)
    {
        this.options = options;
        this.scopes = new ConcurrentStack<object>();
        this.scopeDisposable = new PowerShellLoggerScopeDisposable(this.scopes);
        this.name = name;
        this.currentCommandProvider = currentCommandProvider;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        this.scopes.Push(state);
        return this.scopeDisposable;
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public bool IsEnabled(LogLevel logLevel) => this.options.Value.MinLevel >= logLevel;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!this.IsEnabled(logLevel))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(formatter);

        var message = formatter(state, exception);

        if (!string.IsNullOrEmpty(message) || exception != null)
        {
            this.WriteMessage(logLevel, eventId, message, exception);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.scopeDisposable.Dispose();
            }

            this.disposedValue = true;
        }
    }

    private static string GetLogLevelString(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace or LogLevel.Critical => logLevel.ToString(),
        LogLevel.Information or LogLevel.Debug or LogLevel.Warning or LogLevel.Error => string.Empty,
        LogLevel.None => throw new NotSupportedException("LogLevel.None is not supported."),
        _ => throw new ArgumentOutOfRangeException(nameof(logLevel)),
    };

#pragma warning disable MA0051 // Method is too long

    private void WriteMessage(
        LogLevel logLevel,
        EventId eventId,
        string message,
        Exception exception)
#pragma warning restore MA0051 // Method is too long
    {
        var logBuilder = new StringBuilder();

        var logLevelString = GetLogLevelString(logLevel);

        if (!string.IsNullOrEmpty(logLevelString))
        {
            _ = logBuilder.Append(logLevelString);
            _ = logBuilder.Append(": ");
        }

        _ = logBuilder.Append(this.name);
        _ = logBuilder.Append('[');
        _ = logBuilder.Append(eventId);
        _ = logBuilder.Append(']');

        if (this.options.Value.IncludeScopes)
        {
            foreach (var scope in this.scopes)
            {
                _ = logBuilder.Append(" => ");
                _ = logBuilder.Append(scope);
            }

            if (!this.scopes.IsEmpty)
            {
                _ = logBuilder.Append(" |");
            }
        }

        _ = logBuilder.Append(message);

        if (exception != null)
        {
            _ = logBuilder.AppendLine();
            _ = logBuilder.Append(exception).AppendLine();
        }

        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Information:
                this.currentCommandProvider.GetCurrentCommand().WriteVerbose(logBuilder.ToString());
                break;

            case LogLevel.Debug:
                this.currentCommandProvider.GetCurrentCommand().WriteDebug(logBuilder.ToString());
                break;

            case LogLevel.Warning:
                this.currentCommandProvider.GetCurrentCommand().WriteWarning(logBuilder.ToString());
                break;

            case LogLevel.Error:
            case LogLevel.Critical:
                this.currentCommandProvider.GetCurrentCommand().WriteError(
                    new ErrorRecord(new InvalidOperationException(logBuilder.ToString(), exception), eventId.ToString(),
                        ErrorCategory.InvalidOperation, targetObject: null));
                break;

            case LogLevel.None:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel));
        }
    }
}
