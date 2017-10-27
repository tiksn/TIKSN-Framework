using System;
using Microsoft.Extensions.Logging;
using TIKSN.Analytics.Logging;
using TIKSN.PowerShell;

namespace PowerShellCoreModule
{
	public class LoggingSetup : LoggingSetupBase
	{
		private readonly IServiceProvider _serviceProvider;

		public LoggingSetup(ILoggerFactory loggerFactory, IServiceProvider serviceProvider) : base(loggerFactory)
		{
			_serviceProvider = serviceProvider;
		}

		public override void Setup()
		{
			_loggerFactory.AddPowerShell(_serviceProvider);
			base.Setup();
		}
	}
}
