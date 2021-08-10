using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.Localization
{
    /// <summary>
    ///     IStringLocalizer decorator for monitoring not found resources.
    /// </summary>
    public class StringLocalizerMonitor : IStringLocalizer
    {
        private readonly ILogger<StringLocalizerMonitor> _logger;
        private readonly IOptions<StringLocalizerMonitorOptions> _options;
        private readonly IStringLocalizer _stringLocalizer;

        public StringLocalizerMonitor(IStringLocalizer stringLocalizer, ILogger<StringLocalizerMonitor> logger,
            IOptions<StringLocalizerMonitorOptions> options)
        {
            this._stringLocalizer = stringLocalizer;
            this._logger = logger;
            this._options = options;
        }

        public LocalizedString this[string name] => this.Log(this._stringLocalizer[name]);

        public LocalizedString this[string name, params object[] arguments] =>
            this.Log(this._stringLocalizer[name, arguments]);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            this._stringLocalizer.GetAllStrings(includeParentCultures);

        private LocalizedString Log(LocalizedString localizedString)
        {
            if (localizedString.ResourceNotFound)
            {
                this._logger.Log(this._options.Value.LogLevel, 414761847,
                    $"Resource with name '{localizedString.Name}' is not found.", null, (s, e) => s);
            }

            return localizedString;
        }
    }
}
