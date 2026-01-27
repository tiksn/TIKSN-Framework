namespace TIKSN.Network;

public interface INetworkConnectivityService
{
    public IObservable<InternetConnectivityState> InternetConnectivityChanged { get; }

    public InternetConnectivityState GetInternetConnectivityState();
}
