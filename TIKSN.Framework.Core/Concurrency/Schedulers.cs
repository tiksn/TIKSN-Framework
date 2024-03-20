using System.Reactive.Concurrency;

namespace TIKSN.Concurrency;

/// <summary>
/// Exposes known schedulers
/// </summary>
public class Schedulers : ISchedulers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Schedulers"/> class.
    /// </summary>
    /// <param name="mainThreadScheduler">The scheduler to use to schedule operations on the main thread.</param>
    /// <param name="taskPoolScheduler">The scheduler to use to schedule operations on the task pool.</param>
    public Schedulers(IScheduler mainThreadScheduler, IScheduler taskPoolScheduler)
    {
        this.MainThreadScheduler = mainThreadScheduler ?? throw new ArgumentNullException(nameof(mainThreadScheduler));
        this.TaskPoolScheduler = taskPoolScheduler ?? throw new ArgumentNullException(nameof(taskPoolScheduler));
    }

    /// <summary>
    /// Gets the scheduler for scheduling operations on the main thread.
    /// </summary>
    public IScheduler MainThreadScheduler { get; }

    /// <summary>
    /// Gets the scheduler for scheduling operations on the task pool.
    /// </summary>
    public IScheduler TaskPoolScheduler { get; }
}
