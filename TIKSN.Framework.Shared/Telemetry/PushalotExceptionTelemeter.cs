using System;
using System.Collections.Generic;
using TIKSN.Analytics.Telemetry;
using TIKSN.Configuration;

namespace TIKSN.Pushalot.Analytics.Telemetry
{
	public class PushalotExceptionTelemeter : PushalotTelemeterBase, IExceptionTelemeter
	{
		public PushalotExceptionTelemeter(IPushalotConfiguration pushalotConfiguration)
			: base(pushalotConfiguration)
		{

		}

		public void TrackException(Exception exception)
		{
			SendMessage(exception.GetType().FullName, exception.Message + Environment.NewLine + exception.StackTrace);
		}

		protected override IEnumerable<string> GetAuthorizationTokens(IPushalotConfiguration pushalotConfiguration)
		{
			return pushalotConfiguration.ExceptionAuthorizationTokens;
		}
	}
}
