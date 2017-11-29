using Microsoft.Extensions.Logging;
using System;
using TIKSN.Analytics.Logging;
using TIKSN.PowerShell;

namespace PowerShell_Module
{
    public class LoggingSetup : LoggingSetupBase
    {
        private readonly IServiceProvider _serviceProvider;

        public LoggingSetup(ILoggerFactory loggerFactory, IServiceProvider serviceProvider) : base(loggerFactory)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public override void Setup()
        {
            base.Setup();

            _loggerFactory.AddPowerShell(_serviceProvider);
        }
    }
}
