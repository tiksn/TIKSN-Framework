﻿using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using TIKSN.Analytics.Logging.NLog;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Logging
{
    public abstract class NLogLoggingSetupBase : ILoggingSetup
    {
        protected readonly LoggingConfiguration _loggingConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPartialConfiguration<RemoteNLogViewerOptions> _remoteNLogViewerOptions;

        protected NLogLoggingSetupBase(ILoggerFactory loggerFactory, IPartialConfiguration<RemoteNLogViewerOptions> remoteNLogViewerOptions)
        {
            _loggingConfiguration = new LoggingConfiguration();
            _loggerFactory = loggerFactory;
            _remoteNLogViewerOptions = remoteNLogViewerOptions;
        }

        public void Setup()
        {
            SetupNLog();

            var options = _remoteNLogViewerOptions.GetConfiguration();
            var nLogViewerTarget = new NLogViewerTarget()
            {
                IncludeNLogData = options.IncludeNLogData,
                AppInfo = options.AppInfo,
                IncludeCallSite = options.IncludeCallSite,
                IncludeSourceInfo = options.IncludeSourceInfo,
                IncludeMdc = options.IncludeMdc,
                IncludeMdlc = options.IncludeMdlc,
                IncludeNdc = options.IncludeNdc
            };

            if (options.Url != null)
                nLogViewerTarget.Layout = Layout.FromString(options.Url.AbsoluteUri);

            AddForAllLevels(nLogViewerTarget);

            _loggerFactory.AddNLog();
            _loggerFactory.ConfigureNLog(_loggingConfiguration);
        }

        protected abstract void SetupNLog();

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
    }
}
