﻿using System;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeExceptionTelemeter : IExceptionTelemeter
	{
		private IConfiguration<CommonConfiguration> commonConfiguration;
		private IExceptionTelemeter[] exceptionTelemeters;

		public CompositeExceptionTelemeter(IConfiguration<CommonConfiguration> commonConfiguration, IExceptionTelemeter[] exceptionTelemeters)
		{
			this.commonConfiguration = commonConfiguration;
			this.exceptionTelemeters = exceptionTelemeters;
		}

		public async Task TrackException(Exception exception)
		{
			if (commonConfiguration.GetConfiguration().IsExceptionTrackingEnabled)
			{
				foreach (var exceptionTelemeter in exceptionTelemeters)
				{
					await exceptionTelemeter.TrackException(exception);
				}
			}
		}

		public async Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
		{
			if (commonConfiguration.GetConfiguration().IsExceptionTrackingEnabled)
			{
				foreach (var exceptionTelemeter in exceptionTelemeters)
				{
					await exceptionTelemeter.TrackException(exception, severityLevel);
				}
			}
		}
	}
}