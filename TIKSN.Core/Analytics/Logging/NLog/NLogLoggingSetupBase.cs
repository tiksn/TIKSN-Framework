using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Logging.NLog
{
    public abstract class NLogLoggingSetupBase : ILoggingSetup
    {
        protected readonly LoggingConfiguration _loggingConfiguration;
        private readonly IPartialConfiguration<RemoteNLogViewerOptions> _remoteNLogViewerOptions;

        protected NLogLoggingSetupBase(IPartialConfiguration<RemoteNLogViewerOptions> remoteNLogViewerOptions)
        {
            _loggingConfiguration = new LoggingConfiguration();
            _remoteNLogViewerOptions = remoteNLogViewerOptions;
        }

        public void Setup(ILoggingBuilder loggingBuilder)
        {
            SetupNLog();

            var options = _remoteNLogViewerOptions.GetConfiguration();

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
                    Address = Layout.FromString(options.Url.AbsoluteUri)
                };

                if (!string.IsNullOrWhiteSpace(options.AppInfo))
                    nLogViewerTarget.AppInfo = options.AppInfo;

                AddForAllLevels(nLogViewerTarget);
            }

            loggingBuilder.AddNLog();
            LogManager.Configuration = _loggingConfiguration;
        }

        protected void AddForAllLevels(Target target)
        {
            _loggingConfiguration.AddTarget(target);
            _loggingConfiguration.AddRuleForAllLevels(target);
        }

        protected void AddForOneLevel(global::NLog.LogLevel level, Target target)
        {
            _loggingConfiguration.AddTarget(target);
            _loggingConfiguration.AddRuleForOneLevel(level, target);
        }

        protected abstract void SetupNLog();
    }
}