using Microsoft.ApplicationInsights.DataContracts;
using System;
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
    }
}