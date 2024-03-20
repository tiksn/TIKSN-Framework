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
    IScheduler MainThreadScheduler { get; }

    /// <summary>
    /// Gets the scheduler for scheduling operations on the task pool.
    /// </summary>
    IScheduler TaskPoolScheduler { get; }
}
