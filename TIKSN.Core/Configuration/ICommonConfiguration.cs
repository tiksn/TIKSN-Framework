namespace TIKSN.Configuration
{
	public interface ICommonConfiguration
	{
		bool IsEventTrackingEnabled { get; }

		bool IsExceptionTrackingEnabled { get; }

		bool IsMetricTrackingEnabled { get; }

		bool IsTraceTrackingEnabled { get; }
	}
}