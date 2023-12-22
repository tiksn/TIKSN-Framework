namespace TIKSN.Progress;

public class SilentOperationProgress : Progress<OperationProgressReport>, IDisposableProgress<OperationProgressReport>
{
    private bool disposedValue;

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            this.disposedValue = true;
        }
    }
}
