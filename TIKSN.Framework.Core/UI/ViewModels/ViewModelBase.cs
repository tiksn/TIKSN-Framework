using System.Reactive.Linq;
using LanguageExt;
using ReactiveUI;
using ReactiveUI.Primitives;
using TIKSN.Concurrency;

namespace TIKSN.UI.ViewModels;

public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private int _busyOperationCount;

    protected ViewModelBase(
        ISequencers sequencers,
        IMessageBus messageBus,
        IScreen hostScreen,
        Seq<string> urlPathSegments)
    {
        ArgumentNullException.ThrowIfNull(sequencers);
        ArgumentNullException.ThrowIfNull(messageBus);
        ArgumentNullException.ThrowIfNull(hostScreen);

        if (urlPathSegments.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(urlPathSegments));
        }

        this.Sequencers = sequencers;
        this.MessageBus = messageBus;
        this.HostScreen = hostScreen;

        this.UrlPathSegment = string.Join('/', urlPathSegments);

        this.Activator = new ViewModelActivator();

        this.ShowAlert = new Interaction<AlertViewModel, RxVoid>(sequencers.MainThreadSequencer);
        this.OpenBrowser = new Interaction<Uri, RxVoid>(sequencers.MainThreadSequencer);
    }

    public ViewModelActivator Activator { get; }

    public string? ErrorMessage
    {
        get;
        protected set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public IScreen HostScreen { get; }

    public bool IsBusy
    {
        get;
        protected set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public IMessageBus MessageBus { get; }

    public Interaction<Uri, RxVoid> OpenBrowser { get; }

    public Interaction<AlertViewModel, RxVoid> ShowAlert { get; }

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
