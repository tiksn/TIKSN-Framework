using System;
using System.Collections.Generic;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public abstract class PushalotTelemeterBase
	{
		protected readonly PushalotClient client;
		protected readonly IPushalotConfiguration pushalotConfiguration;

		public PushalotTelemeterBase(IPushalotConfiguration pushalotConfiguration)
		{
			client = new PushalotClient();

			var authorizationTokens = GetAuthorizationTokens(pushalotConfiguration);

			foreach (var authorizationToken in authorizationTokens)
			{
				client.Subscribe(new PushalotAuthorizationToken(authorizationToken));
			}
		}

		protected abstract IEnumerable<string> GetAuthorizationTokens(IPushalotConfiguration pushalotConfiguration);

		protected void SendMessage(string title, string content)
		{
			var mbuilder = new PushalotMessageBuilder();
			mbuilder.MessageLinkTitle = title;
			mbuilder.MessageBody = content;

			var message = mbuilder.Build();

			client.SendMessage(message).Wait();
		}
	}
}
