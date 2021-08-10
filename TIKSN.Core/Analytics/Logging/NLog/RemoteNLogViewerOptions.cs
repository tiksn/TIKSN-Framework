using System;

namespace TIKSN.Analytics.Logging.NLog
{
    public class RemoteNLogViewerOptions
    {
        public const string RemoteNLogViewerConfigurationSection = "RemoteNLogViewer";

        private string _address;

        /// <summary>
        ///     Gets or sets a value indicating whether to include NLog-specific extensions to log4j schema.
        /// </summary>
        public bool IncludeNLogData { get; set; }

        /// <summary>
        ///     Gets or sets the AppInfo field. By default it's the friendly name of the current AppDomain.
        /// </summary>
        public string AppInfo { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include call site (class and method name) in the information sent over
        ///     the network.
        /// </summary>
        public bool IncludeCallSite { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include source info (file name and line number) in the information sent
        ///     over the network.
        /// </summary>
        public bool IncludeSourceInfo { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include NLog.MappedDiagnosticsContext dictionary contents.
        /// </summary>
        public bool IncludeMdc { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include NLog.MappedDiagnosticsLogicalContext dictionary contents.
        /// </summary>
        public bool IncludeMdlc { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include NLog.NestedDiagnosticsContext stack contents.
        /// </summary>
        public bool IncludeNdc { get; set; }

        /// <summary>
        ///     Gets or sets the network address.
        /// </summary>
        /// <remarks>
        ///     The network address can be: tcp://host:port - TCP (auto select IPv4/IPv6) (not supported on Windows Phone 7.0)
        ///     tcp4://host:port - force TCP/IPv4 (not supported on Windows Phone 7.0) tcp6://host:port - force TCP/IPv6 (not
        ///     supported on Windows Phone 7.0) udp://host:port - UDP (auto select IPv4/IPv6, not supported on Silverlight and on
        ///     Windows Phone 7.0) udp4://host:port - force UDP/IPv4 (not supported on Silverlight and on Windows Phone 7.0)
        ///     udp6://host:port - force UDP/IPv6 (not supported on Silverlight and on Windows Phone 7.0) http://host:port/pageName
        ///     - HTTP using POST verb https://host:port/pageName - HTTPS using POST verb For SOAP-based webservice support over
        ///     HTTP use WebService target.
        /// </remarks>
        public string Address
        {
            get => this._address;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.Url = null;
                }
                else
                {
                    this.Url = new Uri(value);
                }

                this._address = value;
            }
        }

        /// <summary>
        ///     Gets the network address.
        /// </summary>
        public Uri Url { get; private set; }
    }
}
