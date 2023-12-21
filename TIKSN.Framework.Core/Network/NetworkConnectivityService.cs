using System.Net.NetworkInformation;
using System.Reactive.Linq;

namespace TIKSN.Network;

public class NetworkConnectivityService : NetworkConnectivityServiceBase
{
    public NetworkConnectivityService() =>
        this.InternalInternetConnectivityState =
            Observable.FromEvent<NetworkAvailabilityChangedEventHandler, InternetConnectivityState>(
                _ => (s, e) => this.GetInternetConnectivityStateInternal(),
                h => NetworkChange.NetworkAvailabilityChanged += h,
                h => NetworkChange.NetworkAvailabilityChanged -= h);

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

                case NetworkInterfaceType.Unknown:
                case NetworkInterfaceType.Ethernet:
                case NetworkInterfaceType.TokenRing:
                case NetworkInterfaceType.Fddi:
                case NetworkInterfaceType.BasicIsdn:
                case NetworkInterfaceType.PrimaryIsdn:
                case NetworkInterfaceType.Ppp:
                case NetworkInterfaceType.Loopback:
                case NetworkInterfaceType.Ethernet3Megabit:
                case NetworkInterfaceType.Slip:
                case NetworkInterfaceType.Atm:
                case NetworkInterfaceType.GenericModem:
                case NetworkInterfaceType.FastEthernetT:
                case NetworkInterfaceType.Isdn:
                case NetworkInterfaceType.FastEthernetFx:
                case NetworkInterfaceType.AsymmetricDsl:
                case NetworkInterfaceType.RateAdaptDsl:
                case NetworkInterfaceType.SymmetricDsl:
                case NetworkInterfaceType.VeryHighSpeedDsl:
                case NetworkInterfaceType.IPOverAtm:
                case NetworkInterfaceType.GigabitEthernet:
                case NetworkInterfaceType.Tunnel:
                case NetworkInterfaceType.MultiRateSymmetricDsl:
                case NetworkInterfaceType.HighPerformanceSerialBus:
                default:
                    break;
            }

            switch (networkInterface.OperationalStatus)
            {
                case OperationalStatus.Up:
                    break;

                case OperationalStatus.Down:
                case OperationalStatus.Testing:
                case OperationalStatus.Unknown:
                case OperationalStatus.Dormant:
                case OperationalStatus.NotPresent:
                case OperationalStatus.LowerLayerDown:
                default:
                    isWiFiAvailable = false;
                    isCellularNetworkAvailable = false;
                    break;
            }
        }

        return new InternetConnectivityState(isNetworkAvailable, isWiFiAvailable, isCellularNetworkAvailable);
    }
}
