using System.Reactive.Concurrency;

namespace TIKSN.Concurrency;

/// <summary>
/// Exposes known schedulers
/// </summary>
public interface ISchedulers
{
    /// <summary>
    /// Gets the scheduler for scheduling operations on the main thread.
    /// </summary>
    public IScheduler MainThreadScheduler { get; }

    /// <summary>
    /// Gets the scheduler for scheduling operations on the task pool.
    /// </summary>
    public IScheduler TaskPoolScheduler { get; }
}
