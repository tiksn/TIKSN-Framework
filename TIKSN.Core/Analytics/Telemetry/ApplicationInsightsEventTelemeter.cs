using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;

namespace TIKSN.Analytics.Telemetry
{
    public class ApplicationInsightsEventTelemeter : IEventTelemeter
    {
        [Obsolete]
        public Task TrackEventAsync(string name)
        {
            try
            {
                var telemetry = new EventTelemetry(name);
                ApplicationInsightsHelper.TrackEvent(telemetry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.FromResult<object>(null);
        }

        [Obsolete]
        public Task TrackEventAsync(string name, IDictionary<string, string> properties)
        {
            try
            {
                var telemetry = new EventTelemetry(name);

                foreach (var property in properties)
                {
                    telemetry.Properties.Add(property);
                }

                ApplicationInsightsHelper.TrackEvent(telemetry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.FromResult<object>(null);
        }
    }
}
