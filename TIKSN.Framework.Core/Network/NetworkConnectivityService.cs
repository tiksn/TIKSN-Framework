using System.Net.NetworkInformation;
using System.Reactive.Linq;

namespace TIKSN.Network;

public class NetworkConnectivityService : NetworkConnectivityServiceBase
{
    public NetworkConnectivityService() =>
        this.InternalInternetConnectivityState =
            Observable.FromEvent<NetworkAvailabilityChangedEventHandler, InternetConnectivityState>(
                _ => (s, e) => this.FetchState(),
                h => NetworkChange.NetworkAvailabilityChanged += h,
                h => NetworkChange.NetworkAvailabilityChanged -= h);

    protected override InternetConnectivityState FetchState()
    {
        var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
        var allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        var isWiFiAvailable = false;
        var isCellularNetworkAvailable = false;

        foreach (var networkInterface in allNetworkInterfaces)
        {
            var (isWiFi, isCellular) = FetchNetworkInfo(networkInterface);
            isWiFiAvailable |= isWiFi;
            isCellularNetworkAvailable |= isCellular;
        }

        return new InternetConnectivityState(isNetworkAvailable, isWiFiAvailable, isCellularNetworkAvailable);
    }

    private static (bool isWiFi, bool isCellular) FetchNetworkInfo(
        NetworkInterface network)
    {
        var isWiFiAvailable = false;
        var isCellularNetworkAvailable = false;

#pragma warning disable RCS1070 // Remove redundant default switch section
        switch (network.NetworkInterfaceType)
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
#pragma warning disable RCS1136 // Merge switch sections with equivalent content
                break;
#pragma warning restore RCS1136 // Merge switch sections with equivalent content
            default:
                break;
#pragma warning restore RCS1070 // Remove redundant default switch section
        }

        return network.OperationalStatus switch
        {
            OperationalStatus.Up => (isWiFiAvailable, isCellularNetworkAvailable),
            OperationalStatus.Down => (false, false),
            OperationalStatus.Testing => (false, false),
            OperationalStatus.Unknown => (false, false),
            OperationalStatus.Dormant => (false, false),
            OperationalStatus.NotPresent => (false, false),
            OperationalStatus.LowerLayerDown => (false, false),
            _ => (false, false),
        };
    }
}
