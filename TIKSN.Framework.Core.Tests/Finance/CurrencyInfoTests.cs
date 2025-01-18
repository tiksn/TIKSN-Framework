using Shouldly;
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

        armenianDram.ISOCurrencySymbol.ShouldBe("AMD");
        armenianDram.CurrencySymbol.ShouldBe("\u058F");
    }

    [Fact]
    public void CreateArmenianDram002()
    {
        var armenia = new System.Globalization.RegionInfo("hy-AM");
        var armenianDram = new CurrencyInfo(armenia);

        armenianDram.ISOCurrencySymbol.ShouldBe("AMD");
        armenianDram.CurrencySymbol.ShouldBe("֏");
    }

    [Fact]
    public void CreateEuro001()
    {
        var germany = new System.Globalization.RegionInfo("DE");
        var euro = new CurrencyInfo(germany);

        euro.ISOCurrencySymbol.ShouldBe("EUR");
        euro.CurrencySymbol.ShouldBe("€");
    }

    [Fact]
    public void CreateEuro002()
    {
        var france = new System.Globalization.RegionInfo("FR");
        var euro = new CurrencyInfo(france);

        euro.ISOCurrencySymbol.ShouldBe("EUR");
        euro.CurrencySymbol.ShouldBe("€");
    }

    [Fact]
    public void CreateEuro003()
    {
        var germany = new System.Globalization.RegionInfo("de-DE");
        var euro = new CurrencyInfo(germany);

        euro.ISOCurrencySymbol.ShouldBe("EUR");
        euro.CurrencySymbol.ShouldBe("€");
    }

    [Fact]
    public void CreateUnitedStatesDollar001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var usDollar = new CurrencyInfo(unitedStates);

        usDollar.ISOCurrencySymbol.ShouldBe("USD");
        usDollar.CurrencySymbol.ShouldBe("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar002()
    {
        var unitedStates = new System.Globalization.RegionInfo("en-US");
        var usDollar = new CurrencyInfo(unitedStates);

        usDollar.ISOCurrencySymbol.ShouldBe("USD");
        usDollar.CurrencySymbol.ShouldBe("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar003()
    {
        var unitedStates = new System.Globalization.RegionInfo("es-US");
        var usDollar = new CurrencyInfo(unitedStates);

        usDollar.ISOCurrencySymbol.ShouldBe("USD");
        usDollar.CurrencySymbol.ShouldBe("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar004()
    {
        var elSalvador = new System.Globalization.RegionInfo("SV");
        var usDollar = new CurrencyInfo(elSalvador);

        usDollar.ISOCurrencySymbol.ShouldBe("USD");
        usDollar.CurrencySymbol.ShouldBe("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar005()
    {
        var elSalvador = new System.Globalization.RegionInfo("es-SV");
        var usDollar = new CurrencyInfo(elSalvador);

        usDollar.ISOCurrencySymbol.ShouldBe("USD");
        usDollar.CurrencySymbol.ShouldBe("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar006()
    {
        var ecuador = new System.Globalization.RegionInfo("EC");
        var usDollar = new CurrencyInfo(ecuador);

        usDollar.ISOCurrencySymbol.ShouldBe("USD");
        usDollar.CurrencySymbol.ShouldBe("$");
    }

    [Fact]
    public void CreateUnitedStatesDollar007()
    {
        var ecuador = new System.Globalization.RegionInfo("es-EC");
        var usDollar = new CurrencyInfo(ecuador);

        usDollar.ISOCurrencySymbol.ShouldBe("USD");
        usDollar.CurrencySymbol.ShouldBe("$");
    }

    [Fact]
    public void CurrencyComparison001()
    {
        var r1 = new System.Globalization.RegionInfo("en-US");
        var r2 = new System.Globalization.RegionInfo("es-US");
        var r3 = new System.Globalization.RegionInfo("US");

        (r1 != r2).ShouldBeTrue();
        (r1 != r3).ShouldBeTrue();
        (r3 != r2).ShouldBeTrue();

        var c1 = new CurrencyInfo(r1);
        var c2 = new CurrencyInfo(r2);
        var c3 = new CurrencyInfo(r3);

        c1.Equals(c1).ShouldBeTrue();
        c2.Equals(c2).ShouldBeTrue();
        c3.Equals(c3).ShouldBeTrue();

        c1.Equals(c2).ShouldBeTrue();
        c2.Equals(c1).ShouldBeTrue();
        c1.Equals(c3).ShouldBeTrue();
        c3.Equals(c1).ShouldBeTrue();
        c2.Equals(c3).ShouldBeTrue();
        c3.Equals(c2).ShouldBeTrue();
    }

    [Fact]
    public void CurrencyComparison002()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var armenia = new System.Globalization.RegionInfo("AM");

        var usDollar = new CurrencyInfo(unitedStates);
        var armenianDram = new CurrencyInfo(armenia);

        armenianDram.Equals(usDollar).ShouldBeFalse();
        usDollar.Equals(armenianDram).ShouldBeFalse();
    }

    [Fact]
    public void CurrencyComparison003()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var elSalvador = new System.Globalization.RegionInfo("SV");

        var usDollar1 = new CurrencyInfo(unitedStates);
        var usDollar2 = new CurrencyInfo(elSalvador);

        (usDollar1 == usDollar2).ShouldBeTrue();
        (usDollar1 != usDollar2).ShouldBeFalse();
    }

    [Fact]
    public void CurrencyComparison004()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var armenia = new System.Globalization.RegionInfo("AM");

        var usDollar = new CurrencyInfo(unitedStates);
        var armenianDram = new CurrencyInfo(armenia);

        (armenianDram != usDollar).ShouldBeTrue();
        (armenianDram == usDollar).ShouldBeFalse();
    }

    [Fact]
    public void CurrencyIsoSymbolAndNumber001()
    {
        var currency = new CurrencyInfo("eur");

        currency.ISOCurrencySymbol.ShouldBe("EUR");
        currency.CurrencySymbol.ShouldBe("EUR");
        currency.ToString().ShouldBe("EUR");
        currency.IsFund.ShouldBeFalse();
        currency.IsCurrent.ShouldBeTrue();
    }

    [Fact]
    public static void CurrencyIsoSymbolAndNumber002()
    {
        var currency = new CurrencyInfo("BOV");

        currency.ISOCurrencySymbol.ShouldBe("BOV");
        currency.CurrencySymbol.ShouldBe("BOV");
        currency.ToString().ShouldBe("BOV");
        currency.IsFund.ShouldBeTrue();
        currency.IsCurrent.ShouldBeTrue();
    }

    [Fact]
    public static void CurrencyIsoSymbolAndNumber003()
    {
        var currency = new CurrencyInfo("XRE");

        currency.ISOCurrencySymbol.ShouldBe("XRE");
        currency.CurrencySymbol.ShouldBe("XRE");
        currency.ToString().ShouldBe("XRE");
        currency.IsFund.ShouldBeTrue();
        currency.IsCurrent.ShouldBeFalse();
    }

    [Fact]
    public static void CurrencyIsoSymbolAndNumber004()
    {
        var currency = new CurrencyInfo(new System.Globalization.RegionInfo("en-US"));

        currency.ISOCurrencySymbol.ShouldBe("USD");
        currency.CurrencySymbol.ShouldBe("$");
        currency.ToString().ShouldBe("USD");
        currency.IsFund.ShouldBeFalse();
        currency.IsCurrent.ShouldBeTrue();
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

        usDollar1.Equals(usDollar1).ShouldBeTrue();
        usDollar1.Equals(usDollar2).ShouldBeTrue();
        usDollar1.Equals(someCurrency1).ShouldBeFalse();
        usDollar1.Equals(something1).ShouldBeFalse();
        usDollar1.Equals(something2).ShouldBeFalse();
        usDollar1.Equals(usDollar3).ShouldBeTrue();
        usDollar1.Equals(usDollar4).ShouldBeTrue();
    }

    [Fact]
    public void EqualsToNull()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var usDollar1 = new CurrencyInfo(unitedStates);
        CurrencyInfo usDollar2 = null;

        (usDollar1 != usDollar2).ShouldBeTrue();
        (usDollar1 == usDollar2).ShouldBeFalse();
    }

    [Fact]
    public void ToString001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var usDollar = new CurrencyInfo(unitedStates);

        usDollar.ToString().ShouldBe("USD");
    }
}
