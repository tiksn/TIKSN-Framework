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
            x is null ? y is null : x.Equals(y);

        public int GetHashCode(InternetConnectivityState obj) => this.IsInternetAvailable.GetHashCode() ^
                                                                 this.IsWiFiAvailable.GetHashCode() ^
                                                                 this.IsCellularNetworkAvailable.GetHashCode();

        public bool Equals(InternetConnectivityState other)
        {
            if (other is null)
            {
                return false;
            }

            return this.IsInternetAvailable == other.IsInternetAvailable &&
                   this.IsWiFiAvailable == other.IsWiFiAvailable &&
                   this.IsCellularNetworkAvailable == other.IsCellularNetworkAvailable;
        }

        public override bool Equals(object obj) => this.Equals(obj as InternetConnectivityState);

        public override int GetHashCode() => throw new NotImplementedException();
    }
}
