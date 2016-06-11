using Xunit;

namespace TIKSN.Finance.Tests
{
	public class CurrencyInfoTests
	{
		[Fact]
		public void CreateArmenianDram001()
		{
			System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("AM");
			CurrencyInfo AMD = new CurrencyInfo(AM);

			Assert.Equal("AMD", AMD.ISOCurrencySymbol);
			Assert.Equal("\u058F", AMD.CurrencySymbol);
		}

		[Fact]
		public void CreateArmenianDram002()
		{
			System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("hy-AM");
			CurrencyInfo AMD = new CurrencyInfo(AM);

			Assert.Equal("AMD", AMD.ISOCurrencySymbol);
			Assert.Equal("֏", AMD.CurrencySymbol);
		}

		[Fact]
		public void CreateEuro001()
		{
			System.Globalization.RegionInfo DE = new System.Globalization.RegionInfo("DE");
			CurrencyInfo Euro = new CurrencyInfo(DE);

			Assert.Equal("EUR", Euro.ISOCurrencySymbol);
			Assert.Equal("€", Euro.CurrencySymbol);
		}

		[Fact]
		public void CreateEuro002()
		{
			System.Globalization.RegionInfo FR = new System.Globalization.RegionInfo("FR");
			CurrencyInfo Euro = new CurrencyInfo(FR);

			Assert.Equal("EUR", Euro.ISOCurrencySymbol);
			Assert.Equal("€", Euro.CurrencySymbol);
		}

		[Fact]
		public void CreateEuro003()
		{
			System.Globalization.RegionInfo DE = new System.Globalization.RegionInfo("de-DE");
			CurrencyInfo Euro = new CurrencyInfo(DE);

			Assert.Equal("EUR", Euro.ISOCurrencySymbol);
			Assert.Equal("€", Euro.CurrencySymbol);
		}

		[Fact]
		public void CreateUnitedStatesDollar001()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			CurrencyInfo USD = new CurrencyInfo(US);

			Assert.Equal("USD", USD.ISOCurrencySymbol);
			Assert.Equal("$", USD.CurrencySymbol);
		}

		[Fact]
		public void CreateUnitedStatesDollar002()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("en-US");
			CurrencyInfo USD = new CurrencyInfo(US);

			Assert.Equal("USD", USD.ISOCurrencySymbol);
			Assert.Equal("$", USD.CurrencySymbol);
		}

		[Fact]
		public void CreateUnitedStatesDollar003()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("es-US");
			CurrencyInfo USD = new CurrencyInfo(US);

			Assert.Equal("USD", USD.ISOCurrencySymbol);
			Assert.Equal("$", USD.CurrencySymbol);
		}

		[Fact]
		public void CreateUnitedStatesDollar004()
		{
			System.Globalization.RegionInfo SV = new System.Globalization.RegionInfo("SV");
			CurrencyInfo USD = new CurrencyInfo(SV);

			Assert.Equal("USD", USD.ISOCurrencySymbol);
			Assert.Equal("$", USD.CurrencySymbol);
		}

		[Fact]
		public void CreateUnitedStatesDollar005()
		{
			System.Globalization.RegionInfo SV = new System.Globalization.RegionInfo("es-SV");
			CurrencyInfo USD = new CurrencyInfo(SV);

			Assert.Equal("USD", USD.ISOCurrencySymbol);
			Assert.Equal("$", USD.CurrencySymbol);
		}

		[Fact]
		public void CreateUnitedStatesDollar006()
		{
			System.Globalization.RegionInfo EC = new System.Globalization.RegionInfo("EC");
			CurrencyInfo USD = new CurrencyInfo(EC);

			Assert.Equal("USD", USD.ISOCurrencySymbol);
			Assert.Equal("$", USD.CurrencySymbol);
		}

		[Fact]
		public void CreateUnitedStatesDollar007()
		{
			System.Globalization.RegionInfo EC = new System.Globalization.RegionInfo("es-EC");
			CurrencyInfo USD = new CurrencyInfo(EC);

			Assert.Equal("USD", USD.ISOCurrencySymbol);
			Assert.Equal("$", USD.CurrencySymbol);
		}

		[Fact]
		public void CurrencyComparison001()
		{
			System.Globalization.RegionInfo R1 = new System.Globalization.RegionInfo("en-US");
			System.Globalization.RegionInfo R2 = new System.Globalization.RegionInfo("es-US");
			System.Globalization.RegionInfo R3 = new System.Globalization.RegionInfo("US");

			Assert.True(R1 != R2);
			Assert.True(R1 != R3);
			Assert.True(R3 != R2);

			CurrencyInfo C1 = new CurrencyInfo(R1);
			CurrencyInfo C2 = new CurrencyInfo(R2);
			CurrencyInfo C3 = new CurrencyInfo(R3);

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
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("AM");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo AMD = new CurrencyInfo(AM);

			Assert.False(AMD.Equals(USD));
			Assert.False(USD.Equals(AMD));
		}

		[Fact]
		public void CurrencyComparison003()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo SV = new System.Globalization.RegionInfo("SV");

			CurrencyInfo USD1 = new CurrencyInfo(US);
			CurrencyInfo USD2 = new CurrencyInfo(SV);

			Assert.True(USD1 == USD2);
			Assert.False(USD1 != USD2);
		}

		[Fact]
		public void CurrencyComparison004()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("AM");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo AMD = new CurrencyInfo(AM);

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
			Assert.Equal(false, currency.IsFund);
			Assert.Equal(true, currency.IsCurrent);
		}

		public void CurrencyIsoSymbolAndNumber002()
		{
			var currency = new CurrencyInfo("BOV");

			Assert.Equal("BOV", currency.ISOCurrencySymbol);
			Assert.Equal("BOV", currency.CurrencySymbol);
			Assert.Equal("BOV", currency.ToString());
			Assert.Equal(true, currency.IsFund);
			Assert.Equal(true, currency.IsCurrent);
		}

		public void CurrencyIsoSymbolAndNumber003()
		{
			var currency = new CurrencyInfo("XRE");

			Assert.Equal("XRE", currency.ISOCurrencySymbol);
			Assert.Equal("XRE", currency.CurrencySymbol);
			Assert.Equal("XRE", currency.ToString());
			Assert.Equal(true, currency.IsFund);
			Assert.Equal(false, currency.IsCurrent);
		}

		public void CurrencyIsoSymbolAndNumber004()
		{
			var currency = new CurrencyInfo(new System.Globalization.RegionInfo("en-US"));

			Assert.Equal("USD", currency.ISOCurrencySymbol);
			Assert.Equal("$", currency.CurrencySymbol);
			Assert.Equal("USD", currency.ToString());
			Assert.Equal(false, currency.IsFund);
			Assert.Equal(true, currency.IsCurrent);
		}

		[Fact]
		public void Equals001()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			CurrencyInfo USD1 = new CurrencyInfo(US);
			CurrencyInfo USD2 = USD1;
			CurrencyInfo SomeCurrency1 = null;
			object Something1 = new object();
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
		public void ToString001()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			CurrencyInfo USD = new CurrencyInfo(US);

			Assert.Equal("USD", USD.ToString());
		}
	}
}