using Microsoft.Extensions.Logging;
using Serilog;

namespace TIKSN.Analytics.Logging
{
	public class SerilogLoggingSetupBase : LoggingSetupBase
	{
		protected LoggerConfiguration _loggerConfiguration;

		protected SerilogLoggingSetupBase(ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			_loggerConfiguration = new LoggerConfiguration();
		}

		public override void Setup()
		{
			base.Setup();

			SetupSerilog();

			_loggerFactory.AddSerilog(_loggerConfiguration.CreateLogger());
		}

		protected virtual void SetupSerilog()
		{
			_loggerConfiguration.MinimumLevel.Verbose();
			_loggerConfiguration.Enrich.FromLogContext();
		}
	}
}
