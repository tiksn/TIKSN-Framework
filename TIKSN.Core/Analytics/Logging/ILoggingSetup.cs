using Microsoft.Extensions.Logging;

namespace TIKSN.Analytics.Logging
{
    public interface ILoggingSetup
    {
        void Setup(ILoggingBuilder loggingBuilder);
    }
}
