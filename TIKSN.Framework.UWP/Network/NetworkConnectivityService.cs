using System;
using System.Reactive.Linq;
using Windows.Networking.Connectivity;

namespace TIKSN.Network
{
	public class NetworkConnectivityService : NetworkConnectivityServiceBase
	{
		public NetworkConnectivityService(IObserver<InternetConnectivityState> internetConnectivityStateObserver) : base(internetConnectivityStateObserver)
		{
			Observable.FromEvent<NetworkStatusChangedEventHandler, InternetConnectivityState>(
				h => (e) => GetInternetConnectivityStateInternal(),
				h => NetworkInformation.NetworkStatusChanged += h,
				h => NetworkInformation.NetworkStatusChanged -= h)
				.Subscribe(internetConnectivityStateObserver);
		}

		protected override InternetConnectivityState GetInternetConnectivityStateInternal()
		{
			var connectionProfile = NetworkInformation.GetInternetConnectionProfile();

			if (connectionProfile == null || connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.InternetAccess)
				return new InternetConnectivityState(false, false, false);

			return new InternetConnectivityState(true, connectionProfile.IsWlanConnectionProfile, connectionProfile.IsWwanConnectionProfile);
		}
	}
}
