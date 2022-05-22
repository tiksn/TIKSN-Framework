using System.Reactive.Linq;
using Windows.Networking.Connectivity;

namespace TIKSN.Network
{
    public class NetworkConnectivityService : NetworkConnectivityServiceBase
    {
        public NetworkConnectivityService() : base()
        {
            internetConnectivityStateInternal = Observable.FromEvent<NetworkStatusChangedEventHandler, InternetConnectivityState>(
                h => (e) => GetInternetConnectivityStateInternal(),
                h => NetworkInformation.NetworkStatusChanged += h,
                h => NetworkInformation.NetworkStatusChanged -= h);
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
