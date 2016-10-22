using Exceptionless;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
    public class ExceptionlessEventTelemeter : ExceptionlessTelemeterBase, IEventTelemeter
    {
        public ExceptionlessEventTelemeter()
        {
        }

        public async Task TrackEvent(string name)
        {
            try
            {
                ExceptionlessClient.Default.CreateFeatureUsage(name).Submit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}