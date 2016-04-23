using System.Threading.Tasks;

namespace TIKSN.Analytics.Telemetry
{
	public interface IEventTelemeter
	{
		Task TrackEvent(string name);
	}
}