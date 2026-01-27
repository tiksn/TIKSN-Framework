using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Finance;

public sealed class CurrencyInfo : IEquatable<CurrencyInfo>
{
    public CurrencyInfo(RegionInfo regionInfo)
    {
        ArgumentNullException.ThrowIfNull(regionInfo);

        (this.IsCurrent, this.ISOCurrencySymbol, this.CurrencySymbol, this.ISOCurrencyNumber, this.IsFund) =
            this.InitializeCurrency(regionInfo.ISOCurrencySymbol, regionInfo.CurrencySymbol);
    }

    public CurrencyInfo(string isoCurrencySymbol)
    {
        ArgumentNullException.ThrowIfNull(isoCurrencySymbol);

        (this.IsCurrent, this.ISOCurrencySymbol, this.CurrencySymbol, this.ISOCurrencyNumber, this.IsFund) =
            this.InitializeCurrency(isoCurrencySymbol, symbol: null);
    }

    public string CurrencySymbol { get; }

    public bool IsCurrent { get; }

    public bool IsFund { get; }

    public int? ISOCurrencyNumber { get; }

    public string ISOCurrencySymbol { get; }

    public static bool operator !=(CurrencyInfo first, CurrencyInfo second) => !Equals(first, second);

    public static bool operator ==(CurrencyInfo first, CurrencyInfo second) => Equals(first, second);

    public bool Equals(CurrencyInfo? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(this.ISOCurrencySymbol, other.ISOCurrencySymbol, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        if (obj is not CurrencyInfo that)
        {
            return false;
        }

        return this.Equals(that);
    }

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(this.ISOCurrencySymbol);

    public override string ToString() => this.ISOCurrencySymbol;

    private static bool Equals(CurrencyInfo first, CurrencyInfo second)
    {
        if (ReferenceEquals(first, second))
        {
            return true;
        }

        if (first is null)
        {
            return false;
        }

        if (second is null)
        {
            return false;
        }

        return first.Equals(second);
    }

    private (bool isCurrent, string isoCurrencySymbol, string currencySymbol, int? isoCurrencyNumber, bool isFund) InitializeCurrency(string isoSymbol, string? symbol)
        => this.TryExtractCurrencyInformation("TIKSN.Finance.Resources.TableA1.xml", isoSymbol, symbol, lookingForCurrent: true, "CcyTbl", "CcyNtry")
            .Match(s => Some(s), () => this.TryExtractCurrencyInformation("TIKSN.Finance.Resources.TableA3.xml", isoSymbol, symbol, lookingForCurrent: false, "HstrcCcyTbl", "HstrcCcyNtry"))
            .MatchUnsafe(s => s, () => throw new CurrencyNotFoundException($"ISO symbol '{isoSymbol}' was not found in resources."));

    private Option<(bool isCurrent, string isoCurrencySymbol, string currencySymbol, int? isoCurrencyNumber, bool isFund)> TryExtractCurrencyInformation(
        string tableResource,
        string isoSymbol,
        string? symbol,
        bool lookingForCurrent,
        string tableElementName,
        string entityElementName)
    {
        using var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(tableResource);
        if (stream is null)
        {
            return None;
        }

        var tableXDoc = XDocument.Load(stream);

        foreach (var ccyNtryElement in tableXDoc
            ?.Element("ISO_4217")
            ?.Element(tableElementName)
            ?.Elements(entityElementName) ?? [])
        {
            var ccyElement = ccyNtryElement.Element("Ccy");

            if (ccyElement != null && string.Equals(ccyElement.Value, isoSymbol, StringComparison.OrdinalIgnoreCase))
            {
                var isCurrent = lookingForCurrent;
                var isoCurrencySymbol = ccyElement.Value;
                var currencySymbol = string.IsNullOrEmpty(symbol) ? isoCurrencySymbol : symbol;
                var ccyNbrElement = ccyNtryElement.Element("CcyNbr");
                int? isoCurrencyNumber = ccyNbrElement is null ? null : int.Parse(ccyNbrElement.Value, CultureInfo.InvariantCulture);

                var ccyNmElement = ccyNtryElement.Element("CcyNm");
                var isFundAttributeValue = ccyNmElement?.Attribute("IsFund")?.Value;

                if (isFundAttributeValue != null)
                {
                    isFundAttributeValue = isFundAttributeValue.Trim();
                    if (string.Equals(isFundAttributeValue, "0", StringComparison.OrdinalIgnoreCase))
                    {
                        isFundAttributeValue = false.ToString();
                    }
                }

                var isFund = !string.IsNullOrWhiteSpace(isFundAttributeValue) &&
                    (string.Equals(isFundAttributeValue, "WAHR", StringComparison.OrdinalIgnoreCase) ||
                    bool.Parse(isFundAttributeValue));

                return (isCurrent, isoCurrencySymbol, currencySymbol, isoCurrencyNumber, isFund);
            }
        }

        return None;
    }
}
