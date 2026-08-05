using ReactiveUI.Primitives.Concurrency;

namespace TIKSN.Concurrency;

/// <summary>
/// Exposes known sequencers
/// </summary>
public interface ISequencers
{
    /// <summary>
    /// Gets the sequencer for scheduling operations on the main thread.
    /// </summary>
    public ISequencer MainThreadSequencer { get; }

    /// <summary>
    /// Gets the sequencer for scheduling operations on the task pool.
    /// </summary>
    public ISequencer TaskPoolSequencer { get; }
}
