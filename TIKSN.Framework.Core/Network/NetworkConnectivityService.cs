using System.Net.NetworkInformation;
using System.Reactive.Linq;

namespace TIKSN.Network;

public class NetworkConnectivityService : NetworkConnectivityServiceBase
{
    public NetworkConnectivityService() =>
        this.InternetConnectivityStateInternal =
            Observable.FromEvent<NetworkAvailabilityChangedEventHandler, InternetConnectivityState>(
                h => (s, e) => this.GetInternetConnectivityStateInternal(),
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
                    break;
                case NetworkInterfaceType.Ethernet:
                    break;
                case NetworkInterfaceType.TokenRing:
                    break;
                case NetworkInterfaceType.Fddi:
                    break;
                case NetworkInterfaceType.BasicIsdn:
                    break;
                case NetworkInterfaceType.PrimaryIsdn:
                    break;
                case NetworkInterfaceType.Ppp:
                    break;
                case NetworkInterfaceType.Loopback:
                    break;
                case NetworkInterfaceType.Ethernet3Megabit:
                    break;
                case NetworkInterfaceType.Slip:
                    break;
                case NetworkInterfaceType.Atm:
                    break;
                case NetworkInterfaceType.GenericModem:
                    break;
                case NetworkInterfaceType.FastEthernetT:
                    break;
                case NetworkInterfaceType.Isdn:
                    break;
                case NetworkInterfaceType.FastEthernetFx:
                    break;
                case NetworkInterfaceType.AsymmetricDsl:
                    break;
                case NetworkInterfaceType.RateAdaptDsl:
                    break;
                case NetworkInterfaceType.SymmetricDsl:
                    break;
                case NetworkInterfaceType.VeryHighSpeedDsl:
                    break;
                case NetworkInterfaceType.IPOverAtm:
                    break;
                case NetworkInterfaceType.GigabitEthernet:
                    break;
                case NetworkInterfaceType.Tunnel:
                    break;
                case NetworkInterfaceType.MultiRateSymmetricDsl:
                    break;
                case NetworkInterfaceType.HighPerformanceSerialBus:
                    break;
                default:
                    break;
            }

            switch (networkInterface.OperationalStatus)
            {
                case OperationalStatus.Up:
                    break;
                case OperationalStatus.Down:
                    break;
                case OperationalStatus.Testing:
                    break;
                case OperationalStatus.Unknown:
                    break;
                case OperationalStatus.Dormant:
                    break;
                case OperationalStatus.NotPresent:
                    break;
                case OperationalStatus.LowerLayerDown:
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
