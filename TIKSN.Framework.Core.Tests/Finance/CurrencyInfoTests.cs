using FluentAssertions;
using Xunit;

namespace TIKSN.Finance.Tests
{
    public class CurrencyInfoTests
    {
        [Fact]
        public void CreateArmenianDram001()
        {
            var AM = new System.Globalization.RegionInfo("AM");
            var AMD = new CurrencyInfo(AM);

            AMD.ISOCurrencySymbol.Should().Be("AMD");
            AMD.CurrencySymbol.Should().Be("\u058F");
        }

        [Fact]
        public void CreateArmenianDram002()
        {
            var AM = new System.Globalization.RegionInfo("hy-AM");
            var AMD = new CurrencyInfo(AM);

            Assert.Equal("AMD", AMD.ISOCurrencySymbol);
            Assert.Equal("֏", AMD.CurrencySymbol);
        }

        [Fact]
        public void CreateEuro001()
        {
            var DE = new System.Globalization.RegionInfo("DE");
            var Euro = new CurrencyInfo(DE);

            Assert.Equal("EUR", Euro.ISOCurrencySymbol);
            Assert.Equal("€", Euro.CurrencySymbol);
        }

        [Fact]
        public void CreateEuro002()
        {
            var FR = new System.Globalization.RegionInfo("FR");
            var Euro = new CurrencyInfo(FR);

            Assert.Equal("EUR", Euro.ISOCurrencySymbol);
            Assert.Equal("€", Euro.CurrencySymbol);
        }

        [Fact]
        public void CreateEuro003()
        {
            var DE = new System.Globalization.RegionInfo("de-DE");
            var Euro = new CurrencyInfo(DE);

            Assert.Equal("EUR", Euro.ISOCurrencySymbol);
            Assert.Equal("€", Euro.CurrencySymbol);
        }

        [Fact]
        public void CreateUnitedStatesDollar001()
        {
            var US = new System.Globalization.RegionInfo("US");
            var USD = new CurrencyInfo(US);

            Assert.Equal("USD", USD.ISOCurrencySymbol);
            Assert.Equal("$", USD.CurrencySymbol);
        }

        [Fact]
        public void CreateUnitedStatesDollar002()
        {
            var US = new System.Globalization.RegionInfo("en-US");
            var USD = new CurrencyInfo(US);

            Assert.Equal("USD", USD.ISOCurrencySymbol);
            Assert.Equal("$", USD.CurrencySymbol);
        }

        [Fact]
        public void CreateUnitedStatesDollar003()
        {
            var US = new System.Globalization.RegionInfo("es-US");
            var USD = new CurrencyInfo(US);

            Assert.Equal("USD", USD.ISOCurrencySymbol);
            Assert.Equal("$", USD.CurrencySymbol);
        }

        [Fact]
        public void CreateUnitedStatesDollar004()
        {
            var SV = new System.Globalization.RegionInfo("SV");
            var USD = new CurrencyInfo(SV);

            Assert.Equal("USD", USD.ISOCurrencySymbol);
            Assert.Equal("$", USD.CurrencySymbol);
        }

        [Fact]
        public void CreateUnitedStatesDollar005()
        {
            var SV = new System.Globalization.RegionInfo("es-SV");
            var USD = new CurrencyInfo(SV);

            Assert.Equal("USD", USD.ISOCurrencySymbol);
            Assert.Equal("$", USD.CurrencySymbol);
        }

        [Fact]
        public void CreateUnitedStatesDollar006()
        {
            var EC = new System.Globalization.RegionInfo("EC");
            var USD = new CurrencyInfo(EC);

            Assert.Equal("USD", USD.ISOCurrencySymbol);
            Assert.Equal("$", USD.CurrencySymbol);
        }

        [Fact]
        public void CreateUnitedStatesDollar007()
        {
            var EC = new System.Globalization.RegionInfo("es-EC");
            var USD = new CurrencyInfo(EC);

            Assert.Equal("USD", USD.ISOCurrencySymbol);
            Assert.Equal("$", USD.CurrencySymbol);
        }

        [Fact]
        public void CurrencyComparison001()
        {
            var R1 = new System.Globalization.RegionInfo("en-US");
            var R2 = new System.Globalization.RegionInfo("es-US");
            var R3 = new System.Globalization.RegionInfo("US");

            Assert.True(R1 != R2);
            Assert.True(R1 != R3);
            Assert.True(R3 != R2);

            var C1 = new CurrencyInfo(R1);
            var C2 = new CurrencyInfo(R2);
            var C3 = new CurrencyInfo(R3);

            Assert.True(C1.Equals(C1));
            Assert.True(C2.Equals(C2));
            Assert.True(C3.Equals(C3));

            Assert.True(C1.Equals(C2));
            Assert.True(C2.Equals(C1));
            Assert.True(C1.Equals(C3));
            Assert.True(C3.Equals(C1));
            Assert.True(C2.Equals(C3));
            Assert.True(C3.Equals(C2));
        }

        [Fact]
        public void CurrencyComparison002()
        {
            var US = new System.Globalization.RegionInfo("US");
            var AM = new System.Globalization.RegionInfo("AM");

            var USD = new CurrencyInfo(US);
            var AMD = new CurrencyInfo(AM);

            Assert.False(AMD.Equals(USD));
            Assert.False(USD.Equals(AMD));
        }

        [Fact]
        public void CurrencyComparison003()
        {
            var US = new System.Globalization.RegionInfo("US");
            var SV = new System.Globalization.RegionInfo("SV");

            var USD1 = new CurrencyInfo(US);
            var USD2 = new CurrencyInfo(SV);

            Assert.True(USD1 == USD2);
            Assert.False(USD1 != USD2);
        }

        [Fact]
        public void CurrencyComparison004()
        {
            var US = new System.Globalization.RegionInfo("US");
            var AM = new System.Globalization.RegionInfo("AM");

            var USD = new CurrencyInfo(US);
            var AMD = new CurrencyInfo(AM);

            Assert.True(AMD != USD);
            Assert.False(AMD == USD);
        }

        [Fact]
        public void CurrencyIsoSymbolAndNumber001()
        {
            var currency = new CurrencyInfo("eur");

            Assert.Equal("EUR", currency.ISOCurrencySymbol);
            Assert.Equal("EUR", currency.CurrencySymbol);
            Assert.Equal("EUR", currency.ToString());
            Assert.False(currency.IsFund);
            Assert.True(currency.IsCurrent);
        }

        [Fact]
        public static void CurrencyIsoSymbolAndNumber002()
        {
            var currency = new CurrencyInfo("BOV");

            Assert.Equal("BOV", currency.ISOCurrencySymbol);
            Assert.Equal("BOV", currency.CurrencySymbol);
            Assert.Equal("BOV", currency.ToString());
            Assert.True(currency.IsFund);
            Assert.True(currency.IsCurrent);
        }

        [Fact]
        public static void CurrencyIsoSymbolAndNumber003()
        {
            var currency = new CurrencyInfo("XRE");

            Assert.Equal("XRE", currency.ISOCurrencySymbol);
            Assert.Equal("XRE", currency.CurrencySymbol);
            Assert.Equal("XRE", currency.ToString());
            Assert.True(currency.IsFund);
            Assert.False(currency.IsCurrent);
        }

        [Fact]
        public static void CurrencyIsoSymbolAndNumber004()
        {
            var currency = new CurrencyInfo(new System.Globalization.RegionInfo("en-US"));

            Assert.Equal("USD", currency.ISOCurrencySymbol);
            Assert.Equal("$", currency.CurrencySymbol);
            Assert.Equal("USD", currency.ToString());
            Assert.False(currency.IsFund);
            Assert.True(currency.IsCurrent);
        }

        [Fact]
        public void Equals001()
        {
            var US = new System.Globalization.RegionInfo("US");
            var USD1 = new CurrencyInfo(US);
            var USD2 = USD1;
            CurrencyInfo SomeCurrency1 = null;
            var Something1 = new object();
            object Something2 = null;
            object USD3 = USD1;
            object USD4 = new CurrencyInfo(US);

            Assert.True(USD1.Equals(USD1));
            Assert.True(USD1.Equals(USD2));
            Assert.False(USD1.Equals(SomeCurrency1));
            Assert.False(USD1.Equals(Something1));
            Assert.False(USD1.Equals(Something2));
            Assert.True(USD1.Equals(USD3));
            Assert.True(USD1.Equals(USD4));
        }

        [Fact]
        public void EqualsToNull()
        {
            var US = new System.Globalization.RegionInfo("US");
            var USD1 = new CurrencyInfo(US);
            CurrencyInfo USD2 = null;

            Assert.True(USD1 != USD2);
            Assert.False(USD1 == USD2);
        }

        [Fact]
        public void ToString001()
        {
            var US = new System.Globalization.RegionInfo("US");
            var USD = new CurrencyInfo(US);

            Assert.Equal("USD", USD.ToString());
        }
    }
}
