using System;

namespace TIKSN.Time
{
    public class TimeProvider : ITimeProvider
    {
        public DateTimeOffset GetCurrentTime()
        {
#pragma warning disable DateTimeNowAnalyzer // DateTime.Now should not be used.
            return DateTimeOffset.Now;
#pragma warning restore DateTimeNowAnalyzer // DateTime.Now should not be used.
        }
    }
}
