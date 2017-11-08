using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TIKSN.Analytics.Logging.NLog;
using TIKSN.FileSystem;

namespace TIKSN.Configuration
{
    public abstract class ConfigurationRootSetupBase
    {
        protected readonly ConfigurationBuilder _configurationBuilder;
        protected readonly object _configurationRootLocker = new object();
        protected IConfigurationRoot _configurationRoot;
        protected IDictionary<string, string> _switchMappings;

        protected ConfigurationRootSetupBase()
        {
            _configurationBuilder = new ConfigurationBuilder();
            _switchMappings = new Dictionary<string, string>();

            _switchMappings.Add("--nlog-viewer-address", ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection, nameof(RemoteNLogViewerOptions.Address)));
            _switchMappings.Add("--nlog-viewer-include-call-site", ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection, nameof(RemoteNLogViewerOptions.IncludeCallSite)));
            _switchMappings.Add("--nlog-viewer-include-source-info", ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection, nameof(RemoteNLogViewerOptions.IncludeSourceInfo)));
        }

        public virtual IConfigurationRoot GetConfigurationRoot()
        {
            if (_configurationRoot == null)
            {
                lock (_configurationRootLocker)
                {
                    if (_configurationRoot == null)
                    {
                        var builder = new ConfigurationBuilder();

                        SetupConfiguration(builder);

                        _configurationRoot = builder.Build();
                    }
                }
            }

            return _configurationRoot;
        }

        public virtual void UpdateSources(IKnownFolders knownFolders)
        {
            _configurationRoot.Reload();
        }

        protected virtual void SetupConfiguration(IConfigurationBuilder builder)
        {
            builder.AddInMemoryCollection();
        }
    }
}