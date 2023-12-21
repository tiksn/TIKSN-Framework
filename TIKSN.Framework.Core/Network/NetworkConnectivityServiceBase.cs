using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TIKSN.Network;

public abstract class NetworkConnectivityServiceBase : INetworkConnectivityService, IDisposable
{
    private readonly Subject<InternetConnectivityState> manualChecks;
    private bool disposedValue;

    protected NetworkConnectivityServiceBase() => this.manualChecks = new Subject<InternetConnectivityState>();

    public IObservable<InternetConnectivityState> InternetConnectivityChanged =>
        this.InternalInternetConnectivityState
            .Merge(this.manualChecks)
            .DistinctUntilChanged();

    protected IObservable<InternetConnectivityState> InternalInternetConnectivityState { get; set; }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public InternetConnectivityState GetInternetConnectivityState() => this.GetInternetConnectivityState(broadcast: true);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.manualChecks?.Dispose();
            }

            this.disposedValue = true;
        }
    }

    protected abstract InternetConnectivityState GetInternetConnectivityStateInternal();

    private InternetConnectivityState GetInternetConnectivityState(bool broadcast)
    {
        var result = this.GetInternetConnectivityStateInternal();

        if (broadcast)
        {
            this.manualChecks.OnNext(result);
        }

        return result;
    }
}
