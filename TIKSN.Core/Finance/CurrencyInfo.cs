using System;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace TIKSN.Finance
{
    public sealed class CurrencyInfo : IEquatable<CurrencyInfo>
    {
        public CurrencyInfo(RegionInfo regionInfo) =>
            this.InitializeCurrency(regionInfo.ISOCurrencySymbol, regionInfo.CurrencySymbol);

        public CurrencyInfo(string isoCurrencySymbol) => this.InitializeCurrency(isoCurrencySymbol, null);

        public string CurrencySymbol { get; private set; }

        public bool IsCurrent { get; private set; }

        public bool IsFund { get; private set; }

        public int? ISOCurrencyNumber { get; private set; }

        public string ISOCurrencySymbol { get; private set; }

        public bool Equals(CurrencyInfo that)
        {
            if (ReferenceEquals(that, null))
            {
                return false;
            }

            return string.CompareOrdinal(this.ISOCurrencySymbol, that.ISOCurrencySymbol) == 0;
        }

        public static bool operator !=(CurrencyInfo first, CurrencyInfo second) => !Equals(first, second);

        public static bool operator ==(CurrencyInfo first, CurrencyInfo second) => Equals(first, second);

        public override bool Equals(object that)
        {
            if (ReferenceEquals(this, that))
            {
                return true;
            }

            if (ReferenceEquals(that, null))
            {
                return false;
            }

            var That = that as CurrencyInfo;

            if (ReferenceEquals(That, null))
            {
                return false;
            }

            return this.Equals(That);
        }

        public override int GetHashCode() => this.ISOCurrencySymbol.GetHashCode();

        public override string ToString() => this.ISOCurrencySymbol;

        private static bool Equals(CurrencyInfo first, CurrencyInfo second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null))
            {
                return false;
            }

            if (ReferenceEquals(second, null))
            {
                return false;
            }

            return first.Equals(second);
        }

        private void InitializeCurrency(string isoSymbol, string symbol)
        {
            if (!this.TryExtractCurrencyInformation("TIKSN.Finance.Resources.TableA1.xml", isoSymbol, symbol, true,
                "CcyTbl", "CcyNtry"))
            {
                if (!this.TryExtractCurrencyInformation("TIKSN.Finance.Resources.TableA3.xml", isoSymbol, symbol, false,
                    "HstrcCcyTbl", "HstrcCcyNtry"))
                {
                    throw new CurrencyNotFoundException($"ISO symbol '{isoSymbol}' was not found in resources.");
                }
            }
        }

        private bool TryExtractCurrencyInformation(string tableResource, string isoSymbol, string symbol,
            bool lookingForCurrent, string tableElementName, string entityElementName)
        {
            using (var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(tableResource))
            {
                var tableXDoc = XDocument.Load(stream);

                foreach (var ccyNtryElement in tableXDoc.Element("ISO_4217").Element(tableElementName)
                    .Elements(entityElementName))
                {
                    var ccyElement = ccyNtryElement.Element("Ccy");

                    if (ccyElement != null)
                    {
                        if (string.Equals(ccyElement.Value, isoSymbol, StringComparison.OrdinalIgnoreCase))
                        {
                            this.IsCurrent = lookingForCurrent;
                            this.ISOCurrencySymbol = ccyElement.Value;
                            this.CurrencySymbol = string.IsNullOrEmpty(symbol) ? this.ISOCurrencySymbol : symbol;
                            this.ISOCurrencyNumber = int.Parse(ccyNtryElement.Element("CcyNbr").Value);

                            var ccyNmElement = ccyNtryElement.Element("CcyNm");
                            var isFundAttributeValue = ccyNmElement.Attribute("IsFund")?.Value;

                            if (isFundAttributeValue != null)
                            {
                                isFundAttributeValue = isFundAttributeValue.Trim();
                                if (string.Equals(isFundAttributeValue, "0", StringComparison.OrdinalIgnoreCase))
                                {
                                    isFundAttributeValue = false.ToString();
                                }
                            }

                            this.IsFund = !string.IsNullOrWhiteSpace(isFundAttributeValue) &&
                                          bool.Parse(isFundAttributeValue);

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
