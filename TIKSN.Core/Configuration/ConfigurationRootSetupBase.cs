using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TIKSN.Analytics.Logging.NLog;
using TIKSN.FileSystem;

namespace TIKSN.Configuration
{
    public abstract class ConfigurationRootSetupBase
    {
        protected readonly ConfigurationBuilder _configurationBuilder;
        protected readonly object _configurationRootLocker = new();
        protected IConfigurationRoot _configurationRoot;
        protected IDictionary<string, string> _switchMappings;

        protected ConfigurationRootSetupBase()
        {
            this._configurationBuilder = new ConfigurationBuilder();
            this._switchMappings = new Dictionary<string, string>();

            this._switchMappings.Add("--nlog-viewer-address",
                ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection,
                    nameof(RemoteNLogViewerOptions.Address)));
            this._switchMappings.Add("--nlog-viewer-include-call-site",
                ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection,
                    nameof(RemoteNLogViewerOptions.IncludeCallSite)));
            this._switchMappings.Add("--nlog-viewer-include-source-info",
                ConfigurationPath.Combine(RemoteNLogViewerOptions.RemoteNLogViewerConfigurationSection,
                    nameof(RemoteNLogViewerOptions.IncludeSourceInfo)));
        }

        public virtual IConfigurationRoot GetConfigurationRoot()
        {
            if (this._configurationRoot == null)
            {
                lock (this._configurationRootLocker)
                {
                    if (this._configurationRoot == null)
                    {
                        var builder = new ConfigurationBuilder();

                        this.SetupConfiguration(builder);

                        this._configurationRoot = builder.Build();
                    }
                }
            }

            return this._configurationRoot;
        }

        public virtual void UpdateSources(IKnownFolders knownFolders) => this._configurationRoot.Reload();

        protected virtual void SetupConfiguration(IConfigurationBuilder builder) => builder.AddInMemoryCollection();
    }
}
