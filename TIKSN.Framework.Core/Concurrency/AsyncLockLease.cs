namespace TIKSN.Concurrency;

internal sealed class AsyncLockLease : IAsyncDisposable
{
    private Action? release;

    public AsyncLockLease(Action release)
    {
        ArgumentNullException.ThrowIfNull(release);
        this.release = release;
    }

    public ValueTask DisposeAsync()
    {
        var releaseAction = Interlocked.Exchange(ref this.release, null);
        if (releaseAction is null)
        {
            return ValueTask.CompletedTask;
        }

        releaseAction();
        return ValueTask.CompletedTask;
    }
}
