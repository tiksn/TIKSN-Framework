using Microsoft.Extensions.Logging;
using TIKSN.Analytics.Logging;

namespace Shell_Commander
{
    public class LoggingSetup : LoggingSetupBase
    {
        public LoggingSetup(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public override void Setup()
        {
            base.Setup();

            _loggerFactory.AddDebug(LogLevel.Debug);
            _loggerFactory.AddConsole(LogLevel.Debug);
        }
    }
}
