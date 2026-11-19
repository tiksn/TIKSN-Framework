namespace TIKSN.Concurrency;

/// <summary>
/// An asynchronous reader/writer lock that allows concurrent readers and exclusive writers.
/// </summary>
public sealed class AsyncReaderWriterLock : IAsyncReaderWriterLock
{
    private readonly DotNext.Threading.AsyncReaderWriterLock readerWriterLock = new();

    /// <inheritdoc />
    public async ValueTask<IAsyncDisposable> AcquireReadAsync(CancellationToken cancellationToken)
    {
        await this.readerWriterLock.EnterReadLockAsync(cancellationToken).ConfigureAwait(false);
        return new AsyncLockLease(this.readerWriterLock.Release);
    }

    /// <inheritdoc />
    public async ValueTask<IAsyncDisposable> AcquireWriteAsync(CancellationToken cancellationToken)
    {
        await this.readerWriterLock.EnterWriteLockAsync(cancellationToken).ConfigureAwait(false);
        return new AsyncLockLease(this.readerWriterLock.Release);
    }

    /// <inheritdoc />
    public void Dispose() => this.readerWriterLock.Dispose();

    /// <inheritdoc />
    public ValueTask DisposeAsync() => this.readerWriterLock.DisposeAsync();
}
