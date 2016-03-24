using System;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeExceptionTelemeter : IExceptionTelemeter
	{
		private ICommonConfiguration commonConfiguration;
		private IExceptionTelemeter[] exceptionTelemeters;

		public CompositeExceptionTelemeter(ICommonConfiguration commonConfiguration, IExceptionTelemeter[] exceptionTelemeters)
		{
			this.commonConfiguration = commonConfiguration;
			this.exceptionTelemeters = exceptionTelemeters;
		}

		public void TrackException(Exception exception)
		{
			if (commonConfiguration.IsExceptionTrackingEnabled)
			{
				foreach (var exceptionTelemeter in exceptionTelemeters)
				{
					exceptionTelemeter.TrackException(exception);
				}
			}
		}
	}
}