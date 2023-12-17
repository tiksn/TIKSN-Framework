using FluentAssertions;
using TIKSN.Finance;
using Xunit;

namespace TIKSN.Tests.Finance;

public class CurrencyInfoTests
{
    [Fact]
    public void CreateArmenianDram001()
    {
        var armenia = new System.Globalization.RegionInfo("AM");
        var armenianDram = new CurrencyInfo(armenia);

        _ = armenianDram.ISOCurrencySymbol.Should().Be("AMD");
        _ = armenianDram.CurrencySymbol.Should().Be("\u058F");
    }

    [Fact]
    public void CreateArmenianDram002()
    {
        var armenia = new System.Globalization.RegionInfo("hy-AM");
        var armenianDram = new CurrencyInfo(armenia);

        _ = armenianDram.ISOCurrencySymbol.Should().Be("AMD");
        _ = armenianDram.CurrencySymbol.Should().Be("֏");
    }

    [Fact]
    public void CreateEuro001()
    {
        var germany = new System.Globalization.RegionInfo("DE");
        var euro = new CurrencyInfo(germany);

        _ = euro.ISOCurrencySymbol.Should().Be("EUR");
        _ = euro.CurrencySymbol.Should().Be("€");
    }

    [Fact]
    public void CreateEuro002()
    {
        var france = new System.Globalization.RegionInfo("FR");
        var euro = new CurrencyInfo(france);

        _ = euro.ISOCurrencySymbol.Should().Be("EUR");
        _ = euro.CurrencySymbol.Should().Be("€");
    }

    [Fact]
    public void CreateEuro003()
    {
        var germany = new System.Globalization.RegionInfo("de-DE");
        var euro = new CurrencyInfo(germany);

        _ = euro.ISOCurrencySymbol.Should().Be("EUR");
        _ = euro.CurrencySymbol.Should().Be("€");
    }

    [Fact]
    public void CreateUnitedStatesDollar001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var usDollar = new CurrencyInfo(unitedStates);

        _ = usDollar.ISOCurrencySymbol.Should().Be("USD");
        _ = usDollar.CurrencySymbol.Should().Be("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar002()
    {
        var unitedStates = new System.Globalization.RegionInfo("en-US");
        var usDollar = new CurrencyInfo(unitedStates);

        _ = usDollar.ISOCurrencySymbol.Should().Be("USD");
        _ = usDollar.CurrencySymbol.Should().Be("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar003()
    {
        var unitedStates = new System.Globalization.RegionInfo("es-US");
        var usDollar = new CurrencyInfo(unitedStates);

        _ = usDollar.ISOCurrencySymbol.Should().Be("USD");
        _ = usDollar.CurrencySymbol.Should().Be("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar004()
    {
        var elSalvador = new System.Globalization.RegionInfo("SV");
        var usDollar = new CurrencyInfo(elSalvador);

        _ = usDollar.ISOCurrencySymbol.Should().Be("USD");
        _ = usDollar.CurrencySymbol.Should().Be("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar005()
    {
        var elSalvador = new System.Globalization.RegionInfo("es-SV");
        var usDollar = new CurrencyInfo(elSalvador);

        _ = usDollar.ISOCurrencySymbol.Should().Be("USD");
        _ = usDollar.CurrencySymbol.Should().Be("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar006()
    {
        var ecuador = new System.Globalization.RegionInfo("EC");
        var usDollar = new CurrencyInfo(ecuador);

        _ = usDollar.ISOCurrencySymbol.Should().Be("USD");
        _ = usDollar.CurrencySymbol.Should().Be("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar007()
    {
        var ecuador = new System.Globalization.RegionInfo("es-EC");
        var usDollar = new CurrencyInfo(ecuador);

        _ = usDollar.ISOCurrencySymbol.Should().Be("USD");
        _ = usDollar.CurrencySymbol.Should().Be("$");
    }

    [Fact]
    public void CurrencyComparison001()
    {
        var r1 = new System.Globalization.RegionInfo("en-US");
        var r2 = new System.Globalization.RegionInfo("es-US");
        var r3 = new System.Globalization.RegionInfo("US");

        _ = (r1 != r2).Should().BeTrue();
        _ = (r1 != r3).Should().BeTrue();
        _ = (r3 != r2).Should().BeTrue();

        var c1 = new CurrencyInfo(r1);
        var c2 = new CurrencyInfo(r2);
        var c3 = new CurrencyInfo(r3);

        _ = c1.Equals(c1).Should().BeTrue();
        _ = c2.Equals(c2).Should().BeTrue();
        _ = c3.Equals(c3).Should().BeTrue();

        _ = c1.Equals(c2).Should().BeTrue();
        _ = c2.Equals(c1).Should().BeTrue();
        _ = c1.Equals(c3).Should().BeTrue();
        _ = c3.Equals(c1).Should().BeTrue();
        _ = c2.Equals(c3).Should().BeTrue();
        _ = c3.Equals(c2).Should().BeTrue();
    }

    [Fact]
    public void CurrencyComparison002()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var armenia = new System.Globalization.RegionInfo("AM");

        var usDollar = new CurrencyInfo(unitedStates);
        var armenianDram = new CurrencyInfo(armenia);

        _ = armenianDram.Equals(usDollar).Should().BeFalse();
        _ = usDollar.Equals(armenianDram).Should().BeFalse();
    }

    [Fact]
    public void CurrencyComparison003()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var elSalvador = new System.Globalization.RegionInfo("SV");

        var usDollar1 = new CurrencyInfo(unitedStates);
        var usDollar2 = new CurrencyInfo(elSalvador);

        _ = (usDollar1 == usDollar2).Should().BeTrue();
        _ = (usDollar1 != usDollar2).Should().BeFalse();
    }

    [Fact]
    public void CurrencyComparison004()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var armenia = new System.Globalization.RegionInfo("AM");

        var usDollar = new CurrencyInfo(unitedStates);
        var armenianDram = new CurrencyInfo(armenia);

        _ = (armenianDram != usDollar).Should().BeTrue();
        _ = (armenianDram == usDollar).Should().BeFalse();
    }

    [Fact]
    public void CurrencyIsoSymbolAndNumber001()
    {
        var currency = new CurrencyInfo("eur");

        _ = currency.ISOCurrencySymbol.Should().Be("EUR");
        _ = currency.CurrencySymbol.Should().Be("EUR");
        _ = currency.ToString().Should().Be("EUR");
        _ = currency.IsFund.Should().BeFalse();
        _ = currency.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public static void CurrencyIsoSymbolAndNumber002()
    {
        var currency = new CurrencyInfo("BOV");

        _ = currency.ISOCurrencySymbol.Should().Be("BOV");
        _ = currency.CurrencySymbol.Should().Be("BOV");
        _ = currency.ToString().Should().Be("BOV");
        _ = currency.IsFund.Should().BeTrue();
        _ = currency.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public static void CurrencyIsoSymbolAndNumber003()
    {
        var currency = new CurrencyInfo("XRE");

        _ = currency.ISOCurrencySymbol.Should().Be("XRE");
        _ = currency.CurrencySymbol.Should().Be("XRE");
        _ = currency.ToString().Should().Be("XRE");
        _ = currency.IsFund.Should().BeTrue();
        _ = currency.IsCurrent.Should().BeFalse();
    }

    [Fact]
    public static void CurrencyIsoSymbolAndNumber004()
    {
        var currency = new CurrencyInfo(new System.Globalization.RegionInfo("en-US"));

        _ = currency.ISOCurrencySymbol.Should().Be("USD");
        _ = currency.CurrencySymbol.Should().Be("$");
        _ = currency.ToString().Should().Be("USD");
        _ = currency.IsFund.Should().BeFalse();
        _ = currency.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public void Equals001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var usDollar1 = new CurrencyInfo(unitedStates);
        var usDollar2 = usDollar1;
        CurrencyInfo someCurrency1 = null;
        var something1 = new object();
        object something2 = null;
        object usDollar3 = usDollar1;
        object usDollar4 = new CurrencyInfo(unitedStates);

        _ = usDollar1.Equals(usDollar1).Should().BeTrue();
        _ = usDollar1.Equals(usDollar2).Should().BeTrue();
        _ = usDollar1.Equals(someCurrency1).Should().BeFalse();
        _ = usDollar1.Equals(something1).Should().BeFalse();
        _ = usDollar1.Equals(something2).Should().BeFalse();
        _ = usDollar1.Equals(usDollar3).Should().BeTrue();
        _ = usDollar1.Equals(usDollar4).Should().BeTrue();
    }

    [Fact]
    public void EqualsToNull()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var usDollar1 = new CurrencyInfo(unitedStates);
        CurrencyInfo usDollar2 = null;

        _ = (usDollar1 != usDollar2).Should().BeTrue();
        _ = (usDollar1 == usDollar2).Should().BeFalse();
    }

    [Fact]
    public void ToString001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var usDollar = new CurrencyInfo(unitedStates);

        _ = usDollar.ToString().Should().Be("USD");
    }
}
