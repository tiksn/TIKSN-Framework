using Microsoft.Azure.Mobile;
using System;
using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public class MobileCenterExceptionTelemeter : IExceptionTelemeter
	{
		public Task TrackException(Exception exception)
		{
			return TrackException(exception, TelemetrySeverityLevel.Information);
		}

		public Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
		{
			switch (severityLevel)
			{
				case TelemetrySeverityLevel.Verbose:
					MobileCenterLog.Verbose(MobileCenterLog.LogTag, exception.Message, exception);
					break;
				case TelemetrySeverityLevel.Information:
					MobileCenterLog.Info(MobileCenterLog.LogTag, exception.Message, exception);
					break;
				case TelemetrySeverityLevel.Warning:
					MobileCenterLog.Warn(MobileCenterLog.LogTag, exception.Message, exception);
					break;
				case TelemetrySeverityLevel.Error:
				case TelemetrySeverityLevel.Critical:
					MobileCenterLog.Error(MobileCenterLog.LogTag, exception.Message, exception);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(severityLevel));
			}

			return Task.CompletedTask;
		}
	}
}
