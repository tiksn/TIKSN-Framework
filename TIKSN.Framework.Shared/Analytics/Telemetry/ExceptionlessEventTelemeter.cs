using Exceptionless;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

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

		public async Task TrackEvent(string name, IDictionary<string, string> properties)
		{
			try
			{
				var eventBuilder = ExceptionlessClient.Default.CreateFeatureUsage(name);

				foreach (var property in properties)
					eventBuilder.SetProperty(property.Key, property.Value);

				eventBuilder.Submit();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}