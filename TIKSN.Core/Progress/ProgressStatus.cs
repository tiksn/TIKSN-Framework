using System.Diagnostics;

namespace TIKSN.Progress
{
	public class ProgressStatus
	{
		public ProgressStatus(double percentage)
		{
			Debug.Assert(percentage >= 0d);
			Debug.Assert(percentage <= 100d);

			Percentage = percentage;
		}

		public ProgressStatus(int completed, int overall)
		{
			Debug.Assert(completed <= overall);
			Debug.Assert(completed >= 0);

			Percentage = overall > 0 ? completed * 100d / overall : 0d;
		}

		public double Percentage { get; private set; }
	}

	public class ProgressStatus<T> : ProgressStatus
	{
		public ProgressStatus(T status, double percentage) : base(percentage)
		{
			Status = status;
		}

		public ProgressStatus(T status, int completed, int overall) : base(completed, overall)
		{
			Status = status;
		}

		public T Status { get; private set; }
	}
}