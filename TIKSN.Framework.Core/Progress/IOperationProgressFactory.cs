namespace TIKSN.Progress;

public interface IOperationProgressFactory
{
    IDisposableProgress<OperationProgressReport> Create(string activity, string statusDescription);
}
