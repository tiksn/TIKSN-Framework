using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
    public class CompositeExceptionTelemeter : IExceptionTelemeter
    {
        private readonly IPartialConfiguration<CommonTelemetryOptions> commonConfiguration;
        private readonly IEnumerable<IExceptionTelemeter> exceptionTelemeters;

        public CompositeExceptionTelemeter(IPartialConfiguration<CommonTelemetryOptions> commonConfiguration,
            IEnumerable<IExceptionTelemeter> exceptionTelemeters)
        {
            this.commonConfiguration = commonConfiguration;
            this.exceptionTelemeters = exceptionTelemeters;
        }

        public async Task TrackExceptionAsync(Exception exception)
        {
            if (this.commonConfiguration.GetConfiguration().IsExceptionTrackingEnabled)
            {
                foreach (var exceptionTelemeter in this.exceptionTelemeters)
                {
                    try
                    {
                        await exceptionTelemeter.TrackExceptionAsync(exception).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        public async Task TrackExceptionAsync(Exception exception, TelemetrySeverityLevel severityLevel)
        {
            if (this.commonConfiguration.GetConfiguration().IsExceptionTrackingEnabled)
            {
                foreach (var exceptionTelemeter in this.exceptionTelemeters)
                {
                    try
                    {
                        await exceptionTelemeter.TrackExceptionAsync(exception, severityLevel).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }
    }
}
