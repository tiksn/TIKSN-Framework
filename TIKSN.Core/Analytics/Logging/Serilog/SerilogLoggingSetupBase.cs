using Microsoft.Extensions.Logging;
using Serilog;

namespace TIKSN.Analytics.Logging.Serilog
{
	public class SerilogLoggingSetupBase : ILoggingSetup
	{
		protected LoggerConfiguration _loggerConfiguration;
		private readonly ILoggerFactory _loggerFactory;

		protected SerilogLoggingSetupBase(ILoggerFactory loggerFactory)
		{
			_loggerConfiguration = new LoggerConfiguration();
			_loggerFactory = loggerFactory;
		}

		public void Setup()
		{
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