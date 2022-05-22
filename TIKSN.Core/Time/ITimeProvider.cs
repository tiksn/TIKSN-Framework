using System;

namespace TIKSN.Time
{
    public interface ITimeProvider
    {
        DateTimeOffset GetCurrentTime();
    }
}
