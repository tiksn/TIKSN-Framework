using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TIKSN.Analytics.Logging.NLog;

namespace TIKSN.Configuration
{
    public abstract class ConfigurationRootSetupBase
    {
        protected IDictionary<string, string> _switchMappings;

        protected ConfigurationRootSetupBase()
        {
            _switchMappings = new Dictionary<string, string>();

            _switchMappings.Add("--nlog-viewer-address", ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection, nameof(RemoteNLogViewerOptions.Address)));
            _switchMappings.Add("--nlog-viewer-include-call-site", ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection, nameof(RemoteNLogViewerOptions.IncludeCallSite)));
            _switchMappings.Add("--nlog-viewer-include-source-info", ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection, nameof(RemoteNLogViewerOptions.IncludeSourceInfo)));
        }

        public virtual IConfigurationRoot CreateConfigurationRoot()
        {
            var builder = new ConfigurationBuilder();

            SetupConfiguration(builder);

            return builder.Build();
        }

        protected virtual void SetupConfiguration(IConfigurationBuilder builder)
        {
            builder.AddInMemoryCollection();
        }
    }
}