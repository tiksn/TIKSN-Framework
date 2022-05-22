using System;
using Xunit;

namespace TIKSN.Finance.Tests
{
    public class CurrencyPairTests
    {
        [Fact]
        public void CurrencyPair001()
        {
            var USA = new System.Globalization.RegionInfo("US");
            var USD1 = new CurrencyInfo(USA);
            var USD2 = new CurrencyInfo(USA);

            _ = Assert.Throws<ArgumentException>(() => new CurrencyPair(USD1, USD2));
        }

        [Fact]
        public void Equals001()
        {
            var USA = new System.Globalization.RegionInfo("US");
            var GBR = new System.Globalization.RegionInfo("GB");
            var ITA = new System.Globalization.RegionInfo("IT");

            var USD = new CurrencyInfo(USA);
            var GBP = new CurrencyInfo(GBR);
            var EUR = new CurrencyInfo(ITA);

            var pair1 = new CurrencyPair(GBP, USD);
            var pair2 = new CurrencyPair(GBP, USD);
            var pair3 = new CurrencyPair(GBP, EUR);
            var pair4 = new CurrencyPair(USD, EUR);
            var pair5 = new CurrencyPair(EUR, USD);

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
            var USA = new System.Globalization.RegionInfo("US");
            var GBR = new System.Globalization.RegionInfo("GB");

            var USD = new CurrencyInfo(USA);
            var GBP = new CurrencyInfo(GBR);

            var pair1 = new CurrencyPair(GBP, USD);
            var pair2 = pair1;
            object pair3 = pair1;
            var something = new object();
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
            var USA = new System.Globalization.RegionInfo("US");
            var GBR = new System.Globalization.RegionInfo("GB");

            var USD = new CurrencyInfo(USA);
            var GBP = new CurrencyInfo(GBR);

            var pair1 = new CurrencyPair(GBP, USD);
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
            var USA = new System.Globalization.RegionInfo("US");
            var GBR = new System.Globalization.RegionInfo("GB");

            var USD = new CurrencyInfo(USA);
            var GBP = new CurrencyInfo(GBR);

            var pair = new CurrencyPair(GBP, USD);

            Assert.Equal("GBP/USD", pair.ToString());
        }
    }
}
