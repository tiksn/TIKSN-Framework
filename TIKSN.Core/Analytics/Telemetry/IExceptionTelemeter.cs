using System;

namespace TIKSN.Analytics.Telemetry
{
	public interface IExceptionTelemeter
	{
		void TrackException(Exception exception);
	}
}