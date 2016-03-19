using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class CompositeEventTelemeter : IEventTelemeter
	{
		private ICommonConfiguration commonConfiguration;
		private IEventTelemeter[] eventTelemeters;

		public CompositeEventTelemeter(ICommonConfiguration commonConfiguration, IEventTelemeter[] eventTelemeters)
		{
			this.commonConfiguration = commonConfiguration;
			this.eventTelemeters = eventTelemeters;
		}

		public void TrackEvent(string name)
		{
			if (commonConfiguration.IsEventTrackingEnabled)
			{
				foreach (var eventTelemeter in eventTelemeters)
				{
					eventTelemeter.TrackEvent(name);
				}
			}
		}
	}
}