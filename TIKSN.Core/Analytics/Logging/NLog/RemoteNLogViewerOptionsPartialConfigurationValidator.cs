using FluentValidation;
using TIKSN.Configuration.Validator;

namespace TIKSN.Analytics.Logging.NLog
{
    public class
        RemoteNLogViewerOptionsPartialConfigurationValidator : PartialConfigurationFluentValidatorBase<
            RemoteNLogViewerOptions>
    {
        public RemoteNLogViewerOptionsPartialConfigurationValidator() =>
            this.RuleFor(instance => instance.Url.Scheme)
                .Must(IsProperScheme)
                .When(instance => instance.Url != null);

        private static bool IsProperScheme(string scheme)
        {
            switch (scheme.ToLowerInvariant())
            {
                case "tcp":
                case "tcp4":
                case "tcp6":
                case "udp":
                case "udp4":
                case "udp6":
                case "http":
                case "https":
                    return true;

                default:
                    return false;
            }
        }
    }
}
