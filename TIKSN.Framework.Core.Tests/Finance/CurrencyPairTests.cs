using System;
using Shouldly;
using TIKSN.Finance;
using Xunit;

namespace TIKSN.Tests.Finance;

public class CurrencyPairTests
{
    [Fact]
    public void CurrencyPair001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var usDollar1 = new CurrencyInfo(unitedStates);
        var usDollar2 = new CurrencyInfo(unitedStates);

        _ = new Func<object>(() => new CurrencyPair(usDollar1, usDollar2)).ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void Equals001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var unitedKingdom = new System.Globalization.RegionInfo("GB");
        var italy = new System.Globalization.RegionInfo("IT");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);
        var euro = new CurrencyInfo(italy);

        var pair1 = new CurrencyPair(poundSterling, usDollar);
        var pair2 = new CurrencyPair(poundSterling, usDollar);
        var pair3 = new CurrencyPair(poundSterling, euro);
        var pair4 = new CurrencyPair(usDollar, euro);
        var pair5 = new CurrencyPair(euro, usDollar);

        pair1.Equals(pair1).ShouldBeTrue();
        (pair1.GetHashCode() == pair1.GetHashCode()).ShouldBeTrue();

#pragma warning disable CS1718 // Comparison made to same variable
        (pair1 == pair1).ShouldBeTrue();
        (pair1 != pair1).ShouldBeFalse();
#pragma warning restore CS1718 // Comparison made to same variable

        pair2.Equals(pair1).ShouldBeTrue();
        pair1.Equals(pair2).ShouldBeTrue();
        (pair1.GetHashCode() == pair2.GetHashCode()).ShouldBeTrue();
        (pair2.GetHashCode() == pair1.GetHashCode()).ShouldBeTrue();
        (pair1 == pair2).ShouldBeTrue();
        (pair2 == pair1).ShouldBeTrue();
        (pair1 != pair2).ShouldBeFalse();
        (pair2 != pair1).ShouldBeFalse();

        pair1.Equals(pair3).ShouldBeFalse();
        pair3.Equals(pair1).ShouldBeFalse();
        (pair1.GetHashCode() == pair3.GetHashCode()).ShouldBeFalse();
        (pair3.GetHashCode() == pair1.GetHashCode()).ShouldBeFalse();
        (pair1 == pair3).ShouldBeFalse();
        (pair3 == pair1).ShouldBeFalse();
        (pair1 != pair3).ShouldBeTrue();
        (pair3 != pair1).ShouldBeTrue();

        pair4.Equals(pair5).ShouldBeFalse();
        pair5.Equals(pair4).ShouldBeFalse();
        (pair4.GetHashCode() == pair5.GetHashCode()).ShouldBeFalse();
        (pair5.GetHashCode() == pair4.GetHashCode()).ShouldBeFalse();
        (pair4 == pair5).ShouldBeFalse();
        (pair5 == pair4).ShouldBeFalse();
        (pair4 != pair5).ShouldBeTrue();
    }

    [Fact]
    public void Equals002()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var unitedKingdom = new System.Globalization.RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair1 = new CurrencyPair(poundSterling, usDollar);
        var pair2 = pair1;
        object pair3 = pair1;
        var something = new object();
        object pair4 = new CurrencyPair(poundSterling, usDollar);

        pair1.Equals(pair2).ShouldBeTrue();
        pair1.Equals(pair3).ShouldBeTrue();
        (pair1 == pair2).ShouldBeTrue();
        (pair2 == pair1).ShouldBeTrue();
        (pair1 != pair2).ShouldBeFalse();
        (pair2 != pair1).ShouldBeFalse();

        pair1.Equals(something).ShouldBeFalse();

        pair1.Equals(pair4).ShouldBeTrue();
    }

    [Fact]
    public void Equals003()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var unitedKingdom = new System.Globalization.RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair1 = new CurrencyPair(poundSterling, usDollar);
        CurrencyPair pair2 = null;
        object pair3 = null;
        CurrencyPair pair4 = null;

        pair1.Equals(pair2).ShouldBeFalse();
        pair1.Equals(pair3).ShouldBeFalse();
        (pair1 == pair2).ShouldBeFalse();
        (pair2 == pair1).ShouldBeFalse();
        (pair2 == pair4).ShouldBeTrue();
        (pair1 != pair2).ShouldBeTrue();
        (pair2 != pair1).ShouldBeTrue();
        (pair2 != pair4).ShouldBeFalse();
    }

    [Fact]
    public void ToString001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var unitedKingdom = new System.Globalization.RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair = new CurrencyPair(poundSterling, usDollar);

        pair.ToString().ShouldBe("GBP/USD");
    }
}
