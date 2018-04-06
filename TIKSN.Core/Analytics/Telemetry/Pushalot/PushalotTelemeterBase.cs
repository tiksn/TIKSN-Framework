using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry.Pushalot
{
    public abstract class PushalotTelemeterBase
    {
        protected readonly Lazy<PushalotClient<TelemetrySeverityLevel>> lazyClient;
        protected readonly IPartialConfiguration<PushalotOptions> pushalotConfiguration;

        protected PushalotTelemeterBase(IPartialConfiguration<PushalotOptions> pushalotConfiguration)
        {
            this.pushalotConfiguration = pushalotConfiguration;

            lazyClient = new Lazy<PushalotClient<TelemetrySeverityLevel>>(CreatePushalotClient);
        }

        protected abstract IEnumerable<string> GetAuthorizationTokens(PushalotOptions pushalotConfiguration);

        protected abstract IEnumerable<string> GetAuthorizationTokens(PushalotOptions pushalotConfiguration, TelemetrySeverityLevel severityLevel);

        protected async Task SendMessage(string title, string content)
        {
            try
            {
                var mbuilder = new PushalotMessageBuilder();
                mbuilder.MessageLinkTitle = title;
                mbuilder.MessageBody = content;

                var message = mbuilder.Build();

                await lazyClient.Value.SendMessage(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private PushalotClient<TelemetrySeverityLevel> CreatePushalotClient()
        {
            var client = new PushalotClient<TelemetrySeverityLevel>();

            var authorizationTokens = GetAuthorizationTokens(pushalotConfiguration.GetConfiguration());

            foreach (var authorizationToken in authorizationTokens)
            {
                client.Subscribe(new PushalotAuthorizationToken(authorizationToken));
            }

            foreach (var severityLevel in Enum.GetValues(typeof(TelemetrySeverityLevel)).Cast<TelemetrySeverityLevel>())
            {
                var severityLevelAuthorizationTokens = GetAuthorizationTokens(pushalotConfiguration.GetConfiguration(), severityLevel);

                foreach (var severityLevelAuthorizationToken in severityLevelAuthorizationTokens)
                {
                    client.Subscribe(severityLevel, new PushalotAuthorizationToken(severityLevelAuthorizationToken));
                }
            }

            return client;
        }
    }
}