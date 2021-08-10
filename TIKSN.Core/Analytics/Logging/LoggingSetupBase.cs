using Microsoft.Extensions.Logging;

namespace TIKSN.Analytics.Logging
{
    public abstract class LoggingSetupBase : ILoggingSetup
    {
        public virtual void Setup(ILoggingBuilder loggingBuilder) => loggingBuilder.AddDebug();
    }
}
