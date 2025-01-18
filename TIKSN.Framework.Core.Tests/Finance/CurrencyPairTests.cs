using System;
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

        _ = new Func<object>(() => new CurrencyPair(usDollar1, usDollar2)).Should().ThrowExactly<ArgumentException>();
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

        _ = pair1.Equals(pair1).Should().BeTrue();
        _ = (pair1.GetHashCode() == pair1.GetHashCode()).Should().BeTrue();

#pragma warning disable CS1718 // Comparison made to same variable
        _ = (pair1 == pair1).Should().BeTrue();
        _ = (pair1 != pair1).Should().BeFalse();
#pragma warning restore CS1718 // Comparison made to same variable

        _ = pair2.Equals(pair1).Should().BeTrue();
        _ = pair1.Equals(pair2).Should().BeTrue();
        _ = (pair1.GetHashCode() == pair2.GetHashCode()).Should().BeTrue();
        _ = (pair2.GetHashCode() == pair1.GetHashCode()).Should().BeTrue();
        _ = (pair1 == pair2).Should().BeTrue();
        _ = (pair2 == pair1).Should().BeTrue();
        _ = (pair1 != pair2).Should().BeFalse();
        _ = (pair2 != pair1).Should().BeFalse();

        _ = pair1.Equals(pair3).Should().BeFalse();
        _ = pair3.Equals(pair1).Should().BeFalse();
        _ = (pair1.GetHashCode() == pair3.GetHashCode()).Should().BeFalse();
        _ = (pair3.GetHashCode() == pair1.GetHashCode()).Should().BeFalse();
        _ = (pair1 == pair3).Should().BeFalse();
        _ = (pair3 == pair1).Should().BeFalse();
        _ = (pair1 != pair3).Should().BeTrue();
        _ = (pair3 != pair1).Should().BeTrue();

        _ = pair4.Equals(pair5).Should().BeFalse();
        _ = pair5.Equals(pair4).Should().BeFalse();
        _ = (pair4.GetHashCode() == pair5.GetHashCode()).Should().BeFalse();
        _ = (pair5.GetHashCode() == pair4.GetHashCode()).Should().BeFalse();
        _ = (pair4 == pair5).Should().BeFalse();
        _ = (pair5 == pair4).Should().BeFalse();
        _ = (pair4 != pair5).Should().BeTrue();
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

        _ = pair1.Equals(pair2).Should().BeTrue();
        _ = pair1.Equals(pair3).Should().BeTrue();
        _ = (pair1 == pair2).Should().BeTrue();
        _ = (pair2 == pair1).Should().BeTrue();
        _ = (pair1 != pair2).Should().BeFalse();
        _ = (pair2 != pair1).Should().BeFalse();

        _ = pair1.Equals(something).Should().BeFalse();

        _ = pair1.Equals(pair4).Should().BeTrue();
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

        _ = pair1.Equals(pair2).Should().BeFalse();
        _ = pair1.Equals(pair3).Should().BeFalse();
        _ = (pair1 == pair2).Should().BeFalse();
        _ = (pair2 == pair1).Should().BeFalse();
        _ = (pair2 == pair4).Should().BeTrue();
        _ = (pair1 != pair2).Should().BeTrue();
        _ = (pair2 != pair1).Should().BeTrue();
        _ = (pair2 != pair4).Should().BeFalse();
    }

    [Fact]
    public void ToString001()
    {
        var unitedStates = new System.Globalization.RegionInfo("US");
        var unitedKingdom = new System.Globalization.RegionInfo("GB");

        var usDollar = new CurrencyInfo(unitedStates);
        var poundSterling = new CurrencyInfo(unitedKingdom);

        var pair = new CurrencyPair(poundSterling, usDollar);

        _ = pair.ToString().Should().Be("GBP/USD");
    }
}
