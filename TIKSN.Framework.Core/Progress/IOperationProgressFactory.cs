namespace TIKSN.Progress;

public interface IOperationProgressFactory
{
    public IDisposableProgress<OperationProgressReport> Create(string activity, string statusDescription);
}
