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

            _ = loggingBuilder.AddSerilog(this._loggerConfiguration.CreateLogger(), true);
        }

        protected virtual void SetupSerilog()
        {
            _ = this._loggerConfiguration.MinimumLevel.Verbose();
            _ = this._loggerConfiguration.Enrich.FromLogContext();
        }
    }
}
