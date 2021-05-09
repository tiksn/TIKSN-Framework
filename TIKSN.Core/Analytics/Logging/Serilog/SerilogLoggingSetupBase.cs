using Microsoft.Extensions.Logging;
using Serilog;

namespace TIKSN.Analytics.Logging.Serilog
{
    public class SerilogLoggingSetupBase : ILoggingSetup
    {
        protected LoggerConfiguration _loggerConfiguration;

        protected SerilogLoggingSetupBase() => this._loggerConfiguration = new LoggerConfiguration();

        public void Setup(ILoggingBuilder loggingBuilder)
        {
            this.SetupSerilog();

            loggingBuilder.AddSerilog(this._loggerConfiguration.CreateLogger(), true);
        }

        protected virtual void SetupSerilog()
        {
            this._loggerConfiguration.MinimumLevel.Verbose();
            this._loggerConfiguration.Enrich.FromLogContext();
        }
    }
}
