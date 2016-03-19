namespace TIKSN.Analytics.Telemetry
{
	public interface ITraceTelemeter
	{
		void TrackTrace(string message);
	}
}