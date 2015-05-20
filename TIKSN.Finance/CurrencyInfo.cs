namespace TIKSN.Finance
{
	public sealed class CurrencyInfo : System.IEquatable<CurrencyInfo>
	{
		private string currencySymbol;
		private bool isCurrent;
		private bool isFund;
		private int? isoCurrencyNumber;
		private string isoCurrencySymbol;

		public CurrencyInfo(System.Globalization.RegionInfo regionInfo)
		{
			this.isoCurrencySymbol = regionInfo.ISOCurrencySymbol;
			this.currencySymbol = regionInfo.CurrencySymbol;
			this.InitializeCurrency(regionInfo.ISOCurrencySymbol, regionInfo.CurrencySymbol);
		}

		public CurrencyInfo(string isoCurrencySymbol)
		{
			this.InitializeCurrency(this.isoCurrencySymbol, this.currencySymbol);
		}

		public string CurrencySymbol
		{
			get
			{
				return this.currencySymbol;
			}
		}

		public bool IsCurrent
		{
			get
			{
				return this.isCurrent;
			}
		}

		public bool IsFund
		{
			get
			{
				return this.isFund;
			}
		}

		public int? ISOCurrencyNumber
		{
			get
			{
				return this.isoCurrencyNumber;
			}
		}

		public string ISOCurrencySymbol
		{
			get
			{
				return this.isoCurrencySymbol;
			}
		}

		public static bool operator !=(CurrencyInfo first, CurrencyInfo second)
		{
			return !first.Equals(second);
		}

		public static bool operator ==(CurrencyInfo first, CurrencyInfo second)
		{
			return first.Equals(second);
		}

		public bool Equals(CurrencyInfo that)
		{
			if (object.ReferenceEquals(that, null))
				return false;

			return string.CompareOrdinal(this.isoCurrencySymbol, that.isoCurrencySymbol) == 0;
		}

		public override bool Equals(object that)
		{
			if (object.ReferenceEquals(this, that))
				return true;

			if (object.ReferenceEquals(that, null))
				return false;

			CurrencyInfo That = that as CurrencyInfo;

			if (object.ReferenceEquals(That, null))
				return false;

			return this.Equals(That);
		}

		public override int GetHashCode()
		{
			return this.isoCurrencySymbol.GetHashCode();
		}

		public override string ToString()
		{
			return this.isoCurrencySymbol;
		}

		private void InitializeCurrency(string isoSymbol, string symbol)
		{
			throw new System.NotImplementedException();
		}
	}
}