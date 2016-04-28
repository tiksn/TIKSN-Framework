namespace TIKSN.Progress
{
	public class ProgressStatus<T>
	{
		public ProgressStatus(T status, double percentage)
		{
			Status = status;
			Percentage = percentage;
		}

		public double Percentage { get; private set; }
		public T Status { get; private set; }
	}
}