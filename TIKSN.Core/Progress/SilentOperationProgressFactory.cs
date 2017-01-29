namespace TIKSN.Progress
{
	public class SilentOperationProgressFactory : IOperationProgressFactory
	{
		public DisposableProgress<OperationProgressReport> Create(string activity, string statusDescription)
		{
			return new DisposableProgress<OperationProgressReport>();
		}
	}
}