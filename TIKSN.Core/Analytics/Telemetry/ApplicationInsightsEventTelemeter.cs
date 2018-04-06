using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class ApplicationInsightsEventTelemeter : IEventTelemeter
    {
        public Task TrackEvent(string name)
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

        public Task TrackEvent(string name, IDictionary<string, string> properties)
        {
            try
            {
                var telemetry = new EventTelemetry(name);

                foreach (var property in properties)
                    telemetry.Properties.Add(property);

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