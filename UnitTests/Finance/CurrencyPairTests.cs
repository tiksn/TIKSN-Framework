using System;
using Xunit;

namespace TIKSN.Finance.Tests
{
    public class CurrencyPairTests
    {
        [Fact]
        public void CurrencyPair001()
        {
            System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            CurrencyInfo USD1 = new CurrencyInfo(USA);
            CurrencyInfo USD2 = new CurrencyInfo(USA);

            Assert.Throws<ArgumentException>(() => new CurrencyPair(USD1, USD2));
        }

        [Fact]
        public void Equals001()
        {
            System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            System.Globalization.RegionInfo GBR = new System.Globalization.RegionInfo("GB");
            System.Globalization.RegionInfo ITA = new System.Globalization.RegionInfo("IT");

            CurrencyInfo USD = new CurrencyInfo(USA);
            CurrencyInfo GBP = new CurrencyInfo(GBR);
            CurrencyInfo EUR = new CurrencyInfo(ITA);

            CurrencyPair pair1 = new CurrencyPair(GBP, USD);
            CurrencyPair pair2 = new CurrencyPair(GBP, USD);
            CurrencyPair pair3 = new CurrencyPair(GBP, EUR);
            CurrencyPair pair4 = new CurrencyPair(USD, EUR);
            CurrencyPair pair5 = new CurrencyPair(EUR, USD);

            Assert.True(pair1.Equals(pair1));
            Assert.True(pair1.GetHashCode() == pair1.GetHashCode());
            Assert.True(pair1 == pair1);
            Assert.False(pair1 != pair1);

            Assert.True(pair2.Equals(pair1));
            Assert.True(pair1.Equals(pair2));
            Assert.True(pair1.GetHashCode() == pair2.GetHashCode());
            Assert.True(pair2.GetHashCode() == pair1.GetHashCode());
            Assert.True(pair1 == pair2);
            Assert.True(pair2 == pair1);
            Assert.False(pair1 != pair2);
            Assert.False(pair2 != pair1);

            Assert.False(pair1.Equals(pair3));
            Assert.False(pair3.Equals(pair1));
            Assert.False(pair1.GetHashCode() == pair3.GetHashCode());
            Assert.False(pair3.GetHashCode() == pair1.GetHashCode());
            Assert.False(pair1 == pair3);
            Assert.False(pair3 == pair1);
            Assert.True(pair1 != pair3);
            Assert.True(pair3 != pair1);

            Assert.False(pair4.Equals(pair5));
            Assert.False(pair5.Equals(pair4));
            Assert.False(pair4.GetHashCode() == pair5.GetHashCode());
            Assert.False(pair5.GetHashCode() == pair4.GetHashCode());
            Assert.False(pair4 == pair5);
            Assert.False(pair5 == pair4);
            Assert.True(pair4 != pair5);
        }

        [Fact]
        public void Equals002()
        {
            System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            System.Globalization.RegionInfo GBR = new System.Globalization.RegionInfo("GB");

            CurrencyInfo USD = new CurrencyInfo(USA);
            CurrencyInfo GBP = new CurrencyInfo(GBR);

            CurrencyPair pair1 = new CurrencyPair(GBP, USD);
            CurrencyPair pair2 = pair1;
            object pair3 = pair1;
            object something = new object();
            object pair4 = new CurrencyPair(GBP, USD);

            Assert.True(pair1.Equals(pair2));
            Assert.True(pair1.Equals(pair3));
            Assert.True(pair1 == pair2);
            Assert.True(pair2 == pair1);
            Assert.False(pair1 != pair2);
            Assert.False(pair2 != pair1);

            Assert.False(pair1.Equals(something));

            Assert.True(pair1.Equals(pair4));
        }

        [Fact]
        public void Equals003()
        {
            System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            System.Globalization.RegionInfo GBR = new System.Globalization.RegionInfo("GB");

            CurrencyInfo USD = new CurrencyInfo(USA);
            CurrencyInfo GBP = new CurrencyInfo(GBR);

            CurrencyPair pair1 = new CurrencyPair(GBP, USD);
            CurrencyPair pair2 = null;
            object pair3 = null;
            CurrencyPair pair4 = null;

            Assert.False(pair1.Equals(pair2));
            Assert.False(pair1.Equals(pair3));
            Assert.False(pair1 == pair2);
            Assert.False(pair2 == pair1);
            Assert.True(pair2 == pair4);
            Assert.True(pair1 != pair2);
            Assert.True(pair2 != pair1);
            Assert.False(pair2 != pair4);
        }

        [Fact]
        public void ToString001()
        {
            System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            System.Globalization.RegionInfo GBR = new System.Globalization.RegionInfo("GB");

            CurrencyInfo USD = new CurrencyInfo(USA);
            CurrencyInfo GBP = new CurrencyInfo(GBR);

            CurrencyPair pair = new CurrencyPair(GBP, USD);

            Assert.Equal("GBP/USD", pair.ToString());
        }
    }
}