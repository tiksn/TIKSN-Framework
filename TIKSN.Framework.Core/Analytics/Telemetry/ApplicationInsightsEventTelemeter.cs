using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TIKSN.Analytics.Telemetry
{
	public class ApplicationInsightsEventTelemeter : IEventTelemeter
	{
		public async Task TrackEvent(string name)
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
		}

		public async Task TrackEvent(string name, IDictionary<string, string> properties)
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
		}
	}
}