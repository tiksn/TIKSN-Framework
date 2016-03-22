namespace TIKSN.Configuration
{
	public interface ICommonConfiguration
	{
		bool IsEventTrackingEnabled { get; set; }

		bool IsExceptionTrackingEnabled { get; set; }

		bool IsMetricTrackingEnabled { get; set; }

		bool IsTraceTrackingEnabled { get; set; }
	}
}