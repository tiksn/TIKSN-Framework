namespace TIKSN.Progress;

public class SilentOperationProgressFactory : IOperationProgressFactory
{
    public IDisposableProgress<OperationProgressReport> Create(string activity, string statusDescription)
        => new SilentOperationProgress();
}
