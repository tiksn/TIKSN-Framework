using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeEventTelemeter : IEventTelemeter
	{
		private readonly IConfiguration<CommonConfiguration> commonConfiguration;
		private readonly IEventTelemeter[] eventTelemeters;

		public CompositeEventTelemeter(IConfiguration<CommonConfiguration> commonConfiguration, IEventTelemeter[] eventTelemeters)
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
					await eventTelemeter.TrackEvent(name);
				}
			}
		}
	}
}