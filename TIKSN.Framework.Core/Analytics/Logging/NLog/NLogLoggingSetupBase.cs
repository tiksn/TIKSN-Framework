using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using LogLevel = NLog.LogLevel;

namespace TIKSN.Analytics.Logging.NLog;

public abstract class NLogLoggingSetupBase : ILoggingSetup
{
    protected NLogLoggingSetupBase() => this.LoggingConfiguration = new LoggingConfiguration();

    protected LoggingConfiguration LoggingConfiguration { get; }

    public void Setup(ILoggingBuilder loggingBuilder)
    {
        this.SetupNLog();

        _ = loggingBuilder.AddNLog(this.LoggingConfiguration);
    }

    protected void AddForAllLevels(Target target)
    {
        this.LoggingConfiguration.AddTarget(target);
        this.LoggingConfiguration.AddRuleForAllLevels(target);
    }

    protected void AddForOneLevel(LogLevel level, Target target)
    {
        this.LoggingConfiguration.AddTarget(target);
        this.LoggingConfiguration.AddRuleForOneLevel(level, target);
    }

    protected abstract void SetupNLog();
}
