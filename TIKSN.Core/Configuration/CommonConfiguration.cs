namespace TIKSN.Configuration
{
	public class CommonConfiguration
	{
		public CommonConfiguration(bool isEventTrackingEnabled, bool isExceptionTrackingEnabled, bool isMetricTrackingEnabled, bool isTraceTrackingEnabled)
		{
			this.IsEventTrackingEnabled = isEventTrackingEnabled;
			this.IsExceptionTrackingEnabled = isExceptionTrackingEnabled;
			this.IsMetricTrackingEnabled = isMetricTrackingEnabled;
			this.IsTraceTrackingEnabled = isTraceTrackingEnabled;
		}

		public bool IsEventTrackingEnabled { get; }

		public bool IsExceptionTrackingEnabled { get; }

		public bool IsMetricTrackingEnabled { get; }

		public bool IsTraceTrackingEnabled { get; }
	}
}