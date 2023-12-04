using System.Management.Automation;

namespace TIKSN.Analytics.Telemetry;

public class PowerShellTraceTelemeter : ITraceTelemeter
{
    private readonly Cmdlet cmdlet;

    public PowerShellTraceTelemeter(Cmdlet cmdlet) => this.cmdlet = cmdlet;

    public Task TrackTraceAsync(string message, TelemetrySeverityLevel severityLevel)
    {
        switch (severityLevel)
        {
            case TelemetrySeverityLevel.Critical:
            case TelemetrySeverityLevel.Error:
                this.cmdlet.WriteError(new ErrorRecord(new Exception(message), errorId: null, ErrorCategory.InvalidOperation,
targetObject: null));
                break;

            case TelemetrySeverityLevel.Information:
                this.cmdlet.WriteDebug(message);
                break;

            case TelemetrySeverityLevel.Verbose:
                this.cmdlet.WriteVerbose(message);
                break;

            case TelemetrySeverityLevel.Warning:
                this.cmdlet.WriteWarning(message);
                break;

            default:
                throw new NotSupportedException();
        }

        return Task.FromResult<object>(null);
    }

    public Task TrackTraceAsync(string message) => this.TrackTraceAsync(message, TelemetrySeverityLevel.Verbose);
}
