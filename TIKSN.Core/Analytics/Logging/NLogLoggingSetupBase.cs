using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace TIKSN.Analytics.Logging
{
	public abstract class NLogLoggingSetupBase : LoggingSetupBase
	{
		protected readonly LoggingConfiguration _loggingConfiguration;

		protected NLogLoggingSetupBase(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			_loggingConfiguration = new LoggingConfiguration();
		}

		public override void Setup()
		{
			base.Setup();

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
