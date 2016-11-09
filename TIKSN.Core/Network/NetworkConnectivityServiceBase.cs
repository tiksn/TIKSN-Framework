using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TIKSN.Network
{
    public abstract class NetworkConnectivityServiceBase : INetworkConnectivityService
    {
        private readonly Subject<InternetConnectivityState> manualChecks;

        protected NetworkConnectivityServiceBase()
        {
            manualChecks = new Subject<InternetConnectivityState>();
        }

        public IObservable<InternetConnectivityState> InternetConnectivityChanged
        {
            get
            {
                return InternetConnectivityStateInternal
                    .Merge(manualChecks)
                    .DistinctUntilChanged();
            }
        }

        protected abstract IObservable<InternetConnectivityState> InternetConnectivityStateInternal { get; }

        public InternetConnectivityState GetInternetConnectivityState()
        {
            return GetInternetConnectivityState(true);
        }

        protected abstract InternetConnectivityState GetInternetConnectivityStateInternal();

        private InternetConnectivityState GetInternetConnectivityState(bool broadcast)
        {
            var result = GetInternetConnectivityStateInternal();

            if (broadcast)
                manualChecks.OnNext(result);

            return result;
        }
    }
}