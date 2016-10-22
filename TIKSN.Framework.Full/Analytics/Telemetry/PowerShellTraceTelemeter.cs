﻿using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class PowerShellTraceTelemeter : ITraceTelemeter
    {
        private readonly Cmdlet cmdlet;

        public PowerShellTraceTelemeter(Cmdlet cmdlet)
        {
            this.cmdlet = cmdlet;
        }

        public Task TrackTrace(string message, TelemetrySeverityLevel severityLevel)
        {
            switch (severityLevel)
            {
                case TelemetrySeverityLevel.Critical:
                case TelemetrySeverityLevel.Error:
                    cmdlet.WriteError(new ErrorRecord(new Exception(message), null, ErrorCategory.InvalidOperation, null));
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

        public Task TrackTrace(string message)
        {
            return TrackTrace(message, TelemetrySeverityLevel.Verbose);
        }
    }
}