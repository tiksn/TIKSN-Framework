using Microsoft.Extensions.Logging;

namespace TIKSN.Analytics.Logging
{
	public abstract class LoggingSetupBase
	{
		protected readonly ILoggerFactory _loggerFactory;

		protected LoggingSetupBase(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}

		public virtual void Setup()
		{
			_loggerFactory.AddDebug(LogLevel.Trace);
		}
	}
}
