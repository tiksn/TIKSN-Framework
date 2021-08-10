using System;
using System.Collections.Generic;

namespace TIKSN.Network
{
    public class InternetConnectivityState : IEqualityComparer<InternetConnectivityState>,
        IEquatable<InternetConnectivityState>
    {
        public InternetConnectivityState(bool isInternetAvailable, bool isWiFiAvailable,
            bool isCellularNetworkAvailable)
        {
            this.IsInternetAvailable = isInternetAvailable;
            this.IsWiFiAvailable = isWiFiAvailable;
            this.IsCellularNetworkAvailable = isCellularNetworkAvailable;
        }

        public bool IsCellularNetworkAvailable { get; }
        public bool IsInternetAvailable { get; }

        public bool IsWiFiAvailable { get; }

        public bool Equals(InternetConnectivityState x, InternetConnectivityState y) =>
            ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);

        public int GetHashCode(InternetConnectivityState obj) => this.IsInternetAvailable.GetHashCode() ^
                                                                 this.IsWiFiAvailable.GetHashCode() ^
                                                                 this.IsCellularNetworkAvailable.GetHashCode();

        public bool Equals(InternetConnectivityState other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return this.IsInternetAvailable == other.IsInternetAvailable &&
                   this.IsWiFiAvailable == other.IsWiFiAvailable &&
                   this.IsCellularNetworkAvailable == other.IsCellularNetworkAvailable;
        }
    }
}
