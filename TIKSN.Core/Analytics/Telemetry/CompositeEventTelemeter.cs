using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
    public class CompositeEventTelemeter : IEventTelemeter
    {
        private readonly IPartialConfiguration<CommonTelemetryOptions> commonConfiguration;
        private readonly IEnumerable<IEventTelemeter> eventTelemeters;

        public CompositeEventTelemeter(IPartialConfiguration<CommonTelemetryOptions> commonConfiguration,
            IEnumerable<IEventTelemeter> eventTelemeters)
        {
            this.commonConfiguration = commonConfiguration;
            this.eventTelemeters = eventTelemeters;
        }

        public async Task TrackEvent(string name)
        {
            if (this.commonConfiguration.GetConfiguration().IsEventTrackingEnabled)
            {
                foreach (var eventTelemeter in this.eventTelemeters)
                {
                    try
                    {
                        await eventTelemeter.TrackEvent(name);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        public async Task TrackEvent(string name, IDictionary<string, string> properties)
        {
            if (this.commonConfiguration.GetConfiguration().IsEventTrackingEnabled)
            {
                foreach (var eventTelemeter in this.eventTelemeters)
                {
                    try
                    {
                        await eventTelemeter.TrackEvent(name, properties);
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
