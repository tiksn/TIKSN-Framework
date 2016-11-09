using System;
using System.Net.NetworkInformation;

namespace TIKSN.Network
{
    public class NetworkConnectivityService : NetworkConnectivityServiceBase
    {
        public NetworkConnectivityService() : base()
        {
        }

        protected override IObservable<InternetConnectivityState> InternetConnectivityStateInternal
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override InternetConnectivityState GetInternetConnectivityStateInternal()
        {
            var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            var allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            var isWiFiAvailable = false;
            var isCellularNetworkAvailable = false;

            foreach (var networkInterface in allNetworkInterfaces)
            {
                switch (networkInterface.NetworkInterfaceType)
                {
                    case NetworkInterfaceType.Wireless80211:
                        isWiFiAvailable = true;
                        break;

                    case NetworkInterfaceType.Wman:
                    case NetworkInterfaceType.Wwanpp:
                    case NetworkInterfaceType.Wwanpp2:
                        isCellularNetworkAvailable = true;
                        break;
                }

                switch (networkInterface.OperationalStatus)
                {
                    case OperationalStatus.Up:
                        break;

                    default:
                        isWiFiAvailable = false;
                        isCellularNetworkAvailable = false;
                        break;
                }
            }

            return new InternetConnectivityState(isNetworkAvailable, isWiFiAvailable, isCellularNetworkAvailable);
        }
    }
}