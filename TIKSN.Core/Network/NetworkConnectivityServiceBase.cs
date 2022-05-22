using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TIKSN.Network
{
    public abstract class NetworkConnectivityServiceBase : INetworkConnectivityService, IDisposable
    {
        private readonly Subject<InternetConnectivityState> manualChecks;
        protected IObservable<InternetConnectivityState> internetConnectivityStateInternal;

        protected NetworkConnectivityServiceBase() => this.manualChecks = new Subject<InternetConnectivityState>();

        public IObservable<InternetConnectivityState> InternetConnectivityChanged =>
            this.internetConnectivityStateInternal
                .Merge(this.manualChecks)
                .DistinctUntilChanged();

        public InternetConnectivityState GetInternetConnectivityState() => this.GetInternetConnectivityState(true);

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

        public void Dispose() => throw new NotImplementedException();
    }
}
