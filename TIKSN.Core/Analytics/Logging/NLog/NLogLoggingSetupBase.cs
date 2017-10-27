using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace TIKSN.Analytics.Logging
{
	public abstract class NLogLoggingSetupBase : ILoggingSetup
    {
		protected readonly LoggingConfiguration _loggingConfiguration;
        private readonly ILoggerFactory _loggerFactory;

        protected NLogLoggingSetupBase(ILoggerFactory loggerFactory)
		{
			_loggingConfiguration = new LoggingConfiguration();
            _loggerFactory = loggerFactory;
        }

		public void Setup()
		{
			SetupNLog();

			_loggerFactory.AddNLog();
			_loggerFactory.ConfigureNLog(_loggingConfiguration);
		}

		protected abstract void SetupNLog();

		protected void AddForAllLevels(Target target)
		{
			_loggingConfiguration.AddTarget(target);
			_loggingConfiguration.AddRuleForAllLevels(target);
		}

		protected void AddForOneLevel(NLog.LogLevel level, Target target)
		{
			_loggingConfiguration.AddTarget(target);
			_loggingConfiguration.AddRuleForOneLevel(level, target);
		}
	}
}
