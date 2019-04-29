using Microsoft.Extensions.Logging;
using Serilog;

namespace TIKSN.Analytics.Logging.Serilog
{
    public class SerilogLoggingSetupBase : ILoggingSetup
    {
        protected LoggerConfiguration _loggerConfiguration;

        protected SerilogLoggingSetupBase()
        {
            _loggerConfiguration = new LoggerConfiguration();
        }

        public void Setup(ILoggingBuilder loggingBuilder)
        {
            SetupSerilog();

            loggingBuilder.AddSerilog(_loggerConfiguration.CreateLogger(), dispose: true);
        }

        protected virtual void SetupSerilog()
        {
            _loggerConfiguration.MinimumLevel.Verbose();
            _loggerConfiguration.Enrich.FromLogContext();
        }
    }
}