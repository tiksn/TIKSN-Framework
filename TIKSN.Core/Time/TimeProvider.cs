using System;

namespace TIKSN.Time
{
	public class TimeProvider : ITimeProvider
	{
		public DateTimeOffset GetCurrentTime()
		{
			return DateTimeOffset.Now;
		}
	}
}
