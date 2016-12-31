using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeExceptionTelemeter : IExceptionTelemeter
	{
		private readonly IConfiguration<CommonConfiguration> commonConfiguration;
		private readonly IEnumerable<IExceptionTelemeter> exceptionTelemeters;

		public CompositeExceptionTelemeter(IConfiguration<CommonConfiguration> commonConfiguration, IEnumerable<IExceptionTelemeter> exceptionTelemeters)
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
					try
					{
						await exceptionTelemeter.TrackException(exception);
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
				}
			}
		}

		public async Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
		{
			if (commonConfiguration.GetConfiguration().IsExceptionTrackingEnabled)
			{
				foreach (var exceptionTelemeter in exceptionTelemeters)
				{
					try
					{
						await exceptionTelemeter.TrackException(exception, severityLevel);
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