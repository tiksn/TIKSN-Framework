﻿namespace TIKSN.Finance
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
			this.InitializeCurrency(regionInfo.ISOCurrencySymbol, regionInfo.CurrencySymbol);
		}

		public CurrencyInfo(string isoCurrencySymbol)
		{
			this.InitializeCurrency(isoCurrencySymbol, null);
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
			return !Equals(first, second);
		}

		public static bool operator ==(CurrencyInfo first, CurrencyInfo second)
		{
			return Equals(first, second);
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

		private static bool Equals(CurrencyInfo first, CurrencyInfo second)
		{
			if (object.ReferenceEquals(first, second))
				return true;

			if (object.ReferenceEquals(first, null))
				return true;

			if (object.ReferenceEquals(second, null))
				return true;

			return first.Equals(second);
		}

		private void InitializeCurrency(string isoSymbol, string symbol)
		{
			if (!TryExtractCurrencyInformation(CurrencyResource.TableA1, isoSymbol, symbol, true, "CcyTbl", "CcyNtry"))
			{
				if (!TryExtractCurrencyInformation(CurrencyResource.TableA3, isoSymbol, symbol, false, "HstrcCcyTbl", "HstrcCcyNtry"))
				{
					throw new CurrencyNotFoundException();
				}
			}
		}

		private bool TryExtractCurrencyInformation(string tableResource, string isoSymbol, string symbol, bool lookingForCurrent, string tableElementName, string entityElementName)
		{
			using (var reader = new System.IO.StringReader(tableResource))
			{
				var tableXDoc = System.Xml.Linq.XDocument.Load(reader);

				foreach (var ccyNtryElement in tableXDoc.Element("ISO_4217").Element(tableElementName).Elements(entityElementName))
				{
					var ccyElement = ccyNtryElement.Element("Ccy");

					if (ccyElement != null)
					{
						if (string.Equals(ccyElement.Value, isoSymbol, System.StringComparison.OrdinalIgnoreCase))
						{
							this.isCurrent = lookingForCurrent;
							this.isoCurrencySymbol = ccyElement.Value;
							this.currencySymbol = string.IsNullOrEmpty(symbol) ? this.isoCurrencySymbol : symbol;
							this.isoCurrencyNumber = int.Parse(ccyNtryElement.Element("CcyNbr").Value);

							var ccyNmElement = ccyNtryElement.Element("CcyNm");
							string isFundAttributeValue = ccyNmElement.Attribute("IsFund")?.Value;

							this.isFund = !string.IsNullOrWhiteSpace(isFundAttributeValue) && bool.Parse(isFundAttributeValue);

							return true;
						}
					}
				}
			}

			return false;
		}
	}
}