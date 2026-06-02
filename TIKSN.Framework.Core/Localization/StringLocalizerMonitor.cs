using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TIKSN.Localization;

/// <summary>
///     IStringLocalizer decorator for monitoring not found resources.
/// </summary>
public partial class StringLocalizerMonitor : IStringLocalizer
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

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        this.stringLocalizer.GetAllStrings(includeParentCultures);

    [LoggerMessage(
        EventId = 76338392,
        Message = "Resource with name '{ResourceName}' is not found.")]
    private static partial void LogResourceNotFound(ILogger logger, LogLevel logLevel, string resourceName);

    private LocalizedString Log(LocalizedString localizedString)
    {
        if (localizedString.ResourceNotFound)
        {
            LogResourceNotFound(this.logger, this.options.Value.LogLevel, localizedString.Name);
        }

        return localizedString;
    }

    public LocalizedString this[string name] => this.Log(this.stringLocalizer[name]);

    public LocalizedString this[string name, params object[] arguments] =>
        this.Log(this.stringLocalizer[name, arguments]);
}
