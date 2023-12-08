using Microsoft.Extensions.Logging;
using Serilog;

namespace TIKSN.Analytics.Logging.Serilog;

public class SerilogLoggingSetupBase : ILoggingSetup
{
    protected SerilogLoggingSetupBase() => this.LoggerConfiguration = new LoggerConfiguration();

    protected LoggerConfiguration LoggerConfiguration { get; }

    public void Setup(ILoggingBuilder loggingBuilder)
    {
        this.SetupSerilog();

        _ = loggingBuilder.AddSerilog(this.LoggerConfiguration.CreateLogger(), dispose: true);
    }

    protected virtual void SetupSerilog()
    {
        _ = this.LoggerConfiguration.MinimumLevel.Verbose();
        _ = this.LoggerConfiguration.Enrich.FromLogContext();
    }
}
