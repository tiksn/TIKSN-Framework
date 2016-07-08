﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Configuration;

namespace TIKSN.Analytics.Telemetry
{
	public class PushalotExceptionTelemeter : PushalotTelemeterBase, IExceptionTelemeter
	{
		public PushalotExceptionTelemeter(IConfiguration<PushalotConfiguration> pushalotConfiguration)
			: base(pushalotConfiguration)
		{
		}

		public async Task TrackException(Exception exception)
		{
			await SendMessage(exception.GetType().FullName, exception.Message + Environment.NewLine + exception.StackTrace);
		}

		public async Task TrackException(Exception exception, TelemetrySeverityLevel severityLevel)
		{
			await SendMessage(string.Format("{0} - {1}", exception.GetType().FullName, severityLevel), exception.Message + Environment.NewLine + exception.StackTrace);
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotConfiguration pushalotConfiguration)
		{
			return pushalotConfiguration.ExceptionAuthorizationTokens;
		}

		protected override IEnumerable<string> GetAuthorizationTokens(PushalotConfiguration pushalotConfiguration, TelemetrySeverityLevel severityLevel)
		{
			if (pushalotConfiguration.SeverityLevelExceptionAuthorizationTokens.ContainsKey(severityLevel))
			{
				return pushalotConfiguration.SeverityLevelExceptionAuthorizationTokens[severityLevel];
			}

			return Enumerable.Empty<string>();
		}
	}
}