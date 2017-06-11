using System;
using System.Collections.Generic;

namespace TIKSN.Network
{
	public class InternetConnectivityState : IEqualityComparer<InternetConnectivityState>, IEquatable<InternetConnectivityState>
	{
		public InternetConnectivityState(bool isInternetAvailable, bool isWiFiAvailable, bool isCellularNetworkAvailable)
		{
			IsInternetAvailable = isInternetAvailable;
			IsWiFiAvailable = isWiFiAvailable;
			IsCellularNetworkAvailable = isCellularNetworkAvailable;
		}

		public bool IsCellularNetworkAvailable { get; private set; }
		public bool IsInternetAvailable { get; private set; }

		public bool IsWiFiAvailable { get; private set; }

		public bool Equals(InternetConnectivityState x, InternetConnectivityState y)
		{
			return ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
		}

		public bool Equals(InternetConnectivityState other)
		{
			if (ReferenceEquals(other, null))
				return false;

			return IsInternetAvailable == other.IsInternetAvailable &&
				IsWiFiAvailable == other.IsWiFiAvailable &&
				IsCellularNetworkAvailable == other.IsCellularNetworkAvailable;
		}

		public int GetHashCode(InternetConnectivityState obj)
		{
			return
				IsInternetAvailable.GetHashCode() ^
				IsWiFiAvailable.GetHashCode() ^
				IsCellularNetworkAvailable.GetHashCode();
		}
	}
}