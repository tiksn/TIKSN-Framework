using FluentValidation;
using TIKSN.Configuration.Validator;

namespace TIKSN.Analytics.Logging.NLog;

public class
    RemoteNLogViewerOptionsPartialConfigurationValidator : PartialConfigurationFluentValidatorBase<
        RemoteNLogViewerOptions>
{
    public RemoteNLogViewerOptionsPartialConfigurationValidator() =>
        this.RuleFor(instance => instance.Url.Scheme)
            .Must(IsProperScheme)
            .When(instance => instance.Url != null);

    private static bool IsProperScheme(string scheme) => scheme.ToLowerInvariant() switch
    {
        "tcp" or "tcp4" or "tcp6" or "udp" or "udp4" or "udp6" or "http" or "https" => true,
        _ => false,
    };
}
