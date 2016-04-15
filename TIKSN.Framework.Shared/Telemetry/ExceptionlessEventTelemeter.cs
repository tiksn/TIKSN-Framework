using Exceptionless;
using System;
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
			ExceptionlessClient.Default.CreateFeatureUsage(name).Submit();
		}
	}
}