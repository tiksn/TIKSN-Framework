using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace TIKSN.Advertising
{
    public class AdUnitSelector : IAdUnitSelector
    {
        private readonly ILogger<AdUnitSelector> _logger;
        private readonly IOptions<AdUnitSelectorOptions> _options;

        public AdUnitSelector(IOptions<AdUnitSelectorOptions> options, ILogger<AdUnitSelector> logger)
        {
            _options = options;
            _logger = logger;
        }

        public AdUnit Select(AdUnitBundle adUnitBundle)
        {
            if (_options.Value.IsDebuggerSensitive && Debugger.IsAttached)
            {
                return adUnitBundle.DesignTime;
            }

            return adUnitBundle.Production;
        }
    }
}