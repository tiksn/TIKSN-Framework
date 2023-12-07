using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.Localization;

/// <summary>
///     IStringLocalizer decorator for monitoring not found resources.
/// </summary>
public class StringLocalizerMonitor : IStringLocalizer
{
    private readonly ILogger<StringLocalizerMonitor> logger;
    private readonly IOptions<StringLocalizerMonitorOptions> options;
    private readonly IStringLocalizer stringLocalizer;

    public StringLocalizerMonitor(
        IStringLocalizer stringLocalizer,
        ILogger<StringLocalizerMonitor> logger,
        IOptions<StringLocalizerMonitorOptions> options)
    {
        this.stringLocalizer = stringLocalizer;
        this.logger = logger;
        this.options = options;
    }

    public LocalizedString this[string name] => this.Log(this.stringLocalizer[name]);

    public LocalizedString this[string name, params object[] arguments] =>
        this.Log(this.stringLocalizer[name, arguments]);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        this.stringLocalizer.GetAllStrings(includeParentCultures);

    private LocalizedString Log(LocalizedString localizedString)
    {
        if (localizedString.ResourceNotFound)
        {
            this.logger.Log(this.options.Value.LogLevel, 414761847,
                $"Resource with name '{localizedString.Name}' is not found.", exception: null, (s, _) => s);
        }

        return localizedString;
    }
}
