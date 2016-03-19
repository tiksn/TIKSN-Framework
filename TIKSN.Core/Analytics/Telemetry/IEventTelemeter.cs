namespace TIKSN.Analytics.Telemetry
{
	public interface IEventTelemeter
	{
		void TrackEvent(string name);
	}
}