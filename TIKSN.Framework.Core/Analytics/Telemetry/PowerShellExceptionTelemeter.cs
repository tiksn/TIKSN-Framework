using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class PowerShellExceptionTelemeter : IExceptionTelemeter
    {
        private readonly Cmdlet cmdlet;

        public PowerShellExceptionTelemeter(Cmdlet cmdlet) => this.cmdlet = cmdlet;

        public Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            this.cmdlet.WriteError(new ErrorRecord(exception, null, ErrorCategory.InvalidOperation, null));

            return Task.FromResult<object>(null);
        }

        public Task TrackExceptionAsync(Exception exception)
        {
            this.cmdlet.WriteError(new ErrorRecord(exception, null, ErrorCategory.InvalidOperation, null));

            return Task.FromResult<object>(null);
        }
    }
}
