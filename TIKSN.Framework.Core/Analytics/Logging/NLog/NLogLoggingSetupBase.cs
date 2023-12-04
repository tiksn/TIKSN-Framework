using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using TIKSN.Configuration;
using LogLevel = NLog.LogLevel;

namespace TIKSN.Analytics.Logging.NLog;

public abstract class NLogLoggingSetupBase : ILoggingSetup
{
    protected readonly LoggingConfiguration loggingConfiguration;
    private readonly IPartialConfiguration<RemoteNLogViewerOptions> remoteNLogViewerOptions;

    protected NLogLoggingSetupBase(IPartialConfiguration<RemoteNLogViewerOptions> remoteNLogViewerOptions)
    {
        this.loggingConfiguration = new LoggingConfiguration();
        this.remoteNLogViewerOptions = remoteNLogViewerOptions;
    }

    public void Setup(ILoggingBuilder loggingBuilder)
    {
        this.SetupNLog();

        var options = this.remoteNLogViewerOptions.GetConfiguration();

        if (options.Url != null)
        {
            var nLogViewerTarget = new NLogViewerTarget("RemoteNLogViewer")
            {
                IncludeNLogData = options.IncludeNLogData,
                IncludeCallSite = options.IncludeCallSite,
                IncludeSourceInfo = options.IncludeSourceInfo,
                IncludeMdc = options.IncludeMdc,
                IncludeMdlc = options.IncludeMdlc,
                IncludeNdc = options.IncludeNdc,
                Address = Layout.FromString(options.Url.AbsoluteUri),
            };

            if (!string.IsNullOrWhiteSpace(options.AppInfo))
            {
                nLogViewerTarget.AppInfo = options.AppInfo;
            }

            this.AddForAllLevels(nLogViewerTarget);
        }

        _ = loggingBuilder.AddNLog();
        LogManager.Configuration = this.loggingConfiguration;
    }

    protected void AddForAllLevels(Target target)
    {
        this.loggingConfiguration.AddTarget(target);
        this.loggingConfiguration.AddRuleForAllLevels(target);
    }

    protected void AddForOneLevel(LogLevel level, Target target)
    {
        this.loggingConfiguration.AddTarget(target);
        this.loggingConfiguration.AddRuleForOneLevel(level, target);
    }

    protected abstract void SetupNLog();
}
