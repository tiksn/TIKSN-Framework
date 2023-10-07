using System;

namespace TIKSN.Network
{
    public interface INetworkConnectivityService
    {
        IObservable<InternetConnectivityState> InternetConnectivityChanged { get; }

        InternetConnectivityState GetInternetConnectivityState();
    }
}
