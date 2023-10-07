namespace TIKSN.Progress
{
    public interface IOperationProgressFactory
    {
        DisposableProgress<OperationProgressReport> Create(string activity, string statusDescription);
    }
}
