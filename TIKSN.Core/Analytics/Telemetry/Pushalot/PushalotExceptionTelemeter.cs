using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
    public class PushalotExceptionTelemeter : PushalotTelemeterBase, IExceptionTelemeter
    {
        public PushalotExceptionTelemeter(IPartialConfiguration<PushalotOptions> pushalotConfiguration)
            : base(pushalotConfiguration)
        {
        }

        public async Task TrackException(Exception exception) => await this.SendMessage(exception.GetType().FullName,
            exception.Message + Environment.NewLine + exception.StackTrace);

        public async Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel) =>
            await this.SendMessage(string.Format("{0} - {1}", exception.GetType().FullName, severityLevel),
                exception.Message + Environment.NewLine + exception.StackTrace);

        protected override IEnumerable<string> GetAuthorizationTokens(PushalotOptions pushalotConfiguration) =>
            pushalotConfiguration.ExceptionAuthorizationTokens;

        protected override IEnumerable<string> GetAuthorizationTokens(PushalotOptions pushalotConfiguration,
            TelemetrySeverityLevel severityLevel)
        {
            if (pushalotConfiguration.SeverityLevelExceptionAuthorizationTokens.ContainsKey(severityLevel))
            {
                return pushalotConfiguration.SeverityLevelExceptionAuthorizationTokens[severityLevel];
            }

            return Enumerable.Empty<string>();
        }
    }
}
