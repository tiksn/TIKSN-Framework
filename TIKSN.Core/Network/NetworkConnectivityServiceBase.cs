using System;

namespace TIKSN.Network
{
	public abstract class NetworkConnectivityServiceBase : INetworkConnectivityService
	{
		private readonly IObserver<InternetConnectivityState> internetConnectivityStateObserver;

		public NetworkConnectivityServiceBase(IObserver<InternetConnectivityState> internetConnectivityStateObserver)
		{
			this.internetConnectivityStateObserver = internetConnectivityStateObserver;
		}

		public InternetConnectivityState GetInternetConnectivityState()
		{
			return GetInternetConnectivityState(true);
		}

		private InternetConnectivityState GetInternetConnectivityState(bool broadcast)
		{
			var result = GetInternetConnectivityStateInternal();

			if (broadcast)
				internetConnectivityStateObserver.OnNext(result);

			return result;
		}

		protected abstract InternetConnectivityState GetInternetConnectivityStateInternal();
	}
}