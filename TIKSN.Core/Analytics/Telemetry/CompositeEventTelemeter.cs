﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeEventTelemeter : IEventTelemeter
	{
		private readonly IConfiguration<CommonConfiguration> commonConfiguration;
		private readonly IEnumerable<IEventTelemeter> eventTelemeters;

		public CompositeEventTelemeter(IConfiguration<CommonConfiguration> commonConfiguration, IEnumerable<IEventTelemeter> eventTelemeters)
		{
			this.commonConfiguration = commonConfiguration;
			this.eventTelemeters = eventTelemeters;
		}

		public async Task TrackEvent(string name)
		{
			if (commonConfiguration.GetConfiguration().IsEventTrackingEnabled)
			{
				foreach (var eventTelemeter in eventTelemeters)
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
	}
}