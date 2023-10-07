using System.Diagnostics;
using Exceptionless;

namespace TIKSN.Analytics.Telemetry
{
    public class ExceptionlessEventTelemeter : ExceptionlessTelemeterBase, IEventTelemeter
    {
        public Task TrackEventAsync(string name)
        {
            try
            {
                ExceptionlessClient.Default.CreateFeatureUsage(name).Submit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.CompletedTask;
        }

        public Task TrackEventAsync(string name, IReadOnlyDictionary<string, string> properties)
        {
            try
            {
                var eventBuilder = ExceptionlessClient.Default.CreateFeatureUsage(name);

                if (properties is not null)
                {
                    foreach (var property in properties)
                    {
                        _ = eventBuilder.SetProperty(property.Key, property.Value);
                    }
                }

                eventBuilder.Submit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.CompletedTask;
        }
    }
}
