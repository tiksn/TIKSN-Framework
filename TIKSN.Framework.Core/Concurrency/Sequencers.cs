using ReactiveUI.Primitives.Concurrency;

namespace TIKSN.Concurrency;

/// <summary>
/// Exposes known sequencers
/// </summary>
public class Sequencers : ISequencers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Sequencers" /> class.
    /// </summary>
    /// <param name="mainThreadSequencer">The sequencer to use to schedule operations on the main thread.</param>
    /// <param name="taskPoolSequencer">The sequencer to use to schedule operations on the task pool.</param>
    public Sequencers(ISequencer mainThreadSequencer, ISequencer taskPoolSequencer)
    {
        this.MainThreadSequencer = mainThreadSequencer ?? throw new ArgumentNullException(nameof(mainThreadSequencer));
        this.TaskPoolSequencer = taskPoolSequencer ?? throw new ArgumentNullException(nameof(taskPoolSequencer));
    }

    /// <summary>
    /// Gets the sequencer for scheduling operations on the main thread.
    /// </summary>
    public ISequencer MainThreadSequencer { get; }

    /// <summary>
    /// Gets the sequencer for scheduling operations on the task pool.
    /// </summary>
    public ISequencer TaskPoolSequencer { get; }
}
