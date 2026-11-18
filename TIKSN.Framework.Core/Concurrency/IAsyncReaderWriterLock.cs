namespace TIKSN.Concurrency;

/// <summary>
/// Provides cancellation-aware asynchronous reader and writer access.
/// </summary>
public interface IAsyncReaderWriterLock : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Acquires a shared read lock.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A lease that releases the read lock when disposed.</returns>
    public ValueTask<IAsyncDisposable> AcquireReadAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Acquires an exclusive write lock.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A lease that releases the write lock when disposed.</returns>
    public ValueTask<IAsyncDisposable> AcquireWriteAsync(CancellationToken cancellationToken);
}
