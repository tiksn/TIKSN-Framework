using System.Reactive.Linq;
using LanguageExt;
using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.SourceGenerators;
using TIKSN.Concurrency;
using TIKSN.UI.Events;

namespace TIKSN.UI.ViewModels;

public abstract partial class ViewModelBase : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private int _busyOperationCount;

    protected ViewModelBase(
        ISequencers sequencers,
        IReactiveEventAggregator eventAggregator,
        IScreen hostScreen,
        Seq<string> urlPathSegments)
    {
        ArgumentNullException.ThrowIfNull(sequencers);
        ArgumentNullException.ThrowIfNull(eventAggregator);
        ArgumentNullException.ThrowIfNull(hostScreen);

        if (urlPathSegments.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(urlPathSegments));
        }

        this.Sequencers = sequencers;
        this.EventAggregator = eventAggregator;
        this.HostScreen = hostScreen;

        this.UrlPathSegment = string.Join('/', urlPathSegments);

        this.Activator = new ViewModelActivator();
    }

    public ViewModelActivator Activator { get; }

    [Reactive]
    public partial string? ErrorMessage { get; protected set; }

    public IReactiveEventAggregator EventAggregator { get; }

    public IScreen HostScreen { get; }

    [Reactive]
    public partial bool IsBusy { get; protected set; }

    public string UrlPathSegment { get; }

    protected ISequencers Sequencers { get; }

    public async Task RunWithBusyAsync(Func<Task> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        this.BeginBusy();
        try
        {
            await operation().ConfigureAwait(false);
        }
        finally
        {
            this.EndBusy();
        }
    }

    protected virtual void HandleException(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        this.ErrorMessage = null;
        this.ErrorMessage = exception.Message;
    }

    protected void ObserveCommandErrors<TInput, TOutput>(ReactiveCommand<TInput, TOutput> command)
    {
        ArgumentNullException.ThrowIfNull(command);

        _ = ObservableExtensions.Subscribe(
            command.ThrownExceptions.ObserveOn(this.Sequencers.MainThreadSequencer),
            this.HandleException);

        _ = ObservableExtensions.Subscribe(
            command.IsExecuting.ObserveOn(this.Sequencers.MainThreadSequencer),
            isExecuting =>
            {
                if (isExecuting)
                {
                    this.BeginBusy();
                }
                else
                {
                    this.EndBusy();
                }
            });
    }

    private void BeginBusy()
    {
        this._busyOperationCount++;
        this.IsBusy = this._busyOperationCount > 0;
    }

    private void EndBusy()
    {
        this._busyOperationCount = Math.Max(0, this._busyOperationCount - 1);
        this.IsBusy = this._busyOperationCount > 0;
    }
}
