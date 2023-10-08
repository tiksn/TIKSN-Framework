using Microsoft.Extensions.Logging;
using Serilog;

namespace TIKSN.Analytics.Logging.Serilog
{
    public class SerilogLoggingSetupBase : ILoggingSetup
    {
        protected LoggerConfiguration loggerConfiguration;

        protected SerilogLoggingSetupBase() => this.loggerConfiguration = new LoggerConfiguration();

        public void Setup(ILoggingBuilder loggingBuilder)
        {
            this.SetupSerilog();

            _ = loggingBuilder.AddSerilog(this.loggerConfiguration.CreateLogger(), dispose: true);
        }

        protected virtual void SetupSerilog()
        {
            _ = this.loggerConfiguration.MinimumLevel.Verbose();
            _ = this.loggerConfiguration.Enrich.FromLogContext();
        }
    }
}
