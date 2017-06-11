using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TIKSN.Network
{
	public abstract class NetworkConnectivityServiceBase : INetworkConnectivityService
	{
		protected IObservable<InternetConnectivityState> internetConnectivityStateInternal;
		private readonly Subject<InternetConnectivityState> manualChecks;

		protected NetworkConnectivityServiceBase()
		{
			manualChecks = new Subject<InternetConnectivityState>();
		}

		public IObservable<InternetConnectivityState> InternetConnectivityChanged
		{
			get
			{
				return internetConnectivityStateInternal
					.Merge(manualChecks)
					.DistinctUntilChanged();
			}
		}

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