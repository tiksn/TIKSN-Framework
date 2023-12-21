namespace TIKSN.Network;

public record InternetConnectivityState(
    bool IsInternetAvailable,
    bool IsWiFiAvailable,
    bool IsCellularNetworkAvailable);
