namespace TIKSN.Finance.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class CurrencyInfoTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateArmenianDram001()
		{
			System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("AM");
			CurrencyInfo AMD = new CurrencyInfo(AM);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("AMD", AMD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("\u058F", AMD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateArmenianDram002()
		{
			System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("hy-AM");
			CurrencyInfo AMD = new CurrencyInfo(AM);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("AMD", AMD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("֏", AMD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateEuro001()
		{
			System.Globalization.RegionInfo DE = new System.Globalization.RegionInfo("DE");
			CurrencyInfo Euro = new CurrencyInfo(DE);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("EUR", Euro.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("€", Euro.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateEuro002()
		{
			System.Globalization.RegionInfo FR = new System.Globalization.RegionInfo("FR");
			CurrencyInfo Euro = new CurrencyInfo(FR);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("EUR", Euro.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("€", Euro.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateEuro003()
		{
			System.Globalization.RegionInfo DE = new System.Globalization.RegionInfo("de-DE");
			CurrencyInfo Euro = new CurrencyInfo(DE);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("EUR", Euro.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("€", Euro.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateUnitedStatesDollar001()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			CurrencyInfo USD = new CurrencyInfo(US);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", USD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$", USD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateUnitedStatesDollar002()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("en-US");
			CurrencyInfo USD = new CurrencyInfo(US);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", USD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$", USD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateUnitedStatesDollar003()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("es-US");
			CurrencyInfo USD = new CurrencyInfo(US);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", USD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$", USD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateUnitedStatesDollar004()
		{
			System.Globalization.RegionInfo SV = new System.Globalization.RegionInfo("SV");
			CurrencyInfo USD = new CurrencyInfo(SV);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", USD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$", USD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateUnitedStatesDollar005()
		{
			System.Globalization.RegionInfo SV = new System.Globalization.RegionInfo("es-SV");
			CurrencyInfo USD = new CurrencyInfo(SV);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", USD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$", USD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateUnitedStatesDollar006()
		{
			System.Globalization.RegionInfo EC = new System.Globalization.RegionInfo("EC");
			CurrencyInfo USD = new CurrencyInfo(EC);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", USD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$", USD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateUnitedStatesDollar007()
		{
			System.Globalization.RegionInfo EC = new System.Globalization.RegionInfo("es-EC");
			CurrencyInfo USD = new CurrencyInfo(EC);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", USD.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$", USD.CurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyComparison001()
		{
			System.Globalization.RegionInfo R1 = new System.Globalization.RegionInfo("en-US");
			System.Globalization.RegionInfo R2 = new System.Globalization.RegionInfo("es-US");
			System.Globalization.RegionInfo R3 = new System.Globalization.RegionInfo("US");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(R1 != R2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(R1 != R3);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(R3 != R2);

			CurrencyInfo C1 = new CurrencyInfo(R1);
			CurrencyInfo C2 = new CurrencyInfo(R2);
			CurrencyInfo C3 = new CurrencyInfo(R3);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C1.Equals(C1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C2.Equals(C2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C3.Equals(C3));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C1.Equals(C2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C2.Equals(C1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C1.Equals(C3));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C3.Equals(C1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C2.Equals(C3));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(C3.Equals(C2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyComparison002()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("AM");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo AMD = new CurrencyInfo(AM);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(AMD.Equals(USD));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(USD.Equals(AMD));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyComparison003()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo SV = new System.Globalization.RegionInfo("SV");

			CurrencyInfo USD1 = new CurrencyInfo(US);
			CurrencyInfo USD2 = new CurrencyInfo(SV);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(USD1 == USD2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(USD1 != USD2);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyComparison004()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo AM = new System.Globalization.RegionInfo("AM");

			CurrencyInfo USD = new CurrencyInfo(US);
			CurrencyInfo AMD = new CurrencyInfo(AM);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(AMD != USD);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(AMD == USD);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(USD1.Equals(USD1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(USD1.Equals(USD2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(USD1.Equals(SomeCurrency1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(USD1.Equals(Something1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(USD1.Equals(Something2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(USD1.Equals(USD3));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(USD1.Equals(USD4));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString001()
		{
			System.Globalization.RegionInfo US = new System.Globalization.RegionInfo("US");
			CurrencyInfo USD = new CurrencyInfo(US);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", USD.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyIsoSymbolAndNumber001()
		{
			var currency = new CurrencyInfo("eur");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("EUR", currency.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("EUR", currency.CurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("EUR", currency.ToString());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(false, currency.IsFund);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, currency.IsCurrent);
		}

		public void CurrencyIsoSymbolAndNumber002()
		{
			var currency = new CurrencyInfo("BOV");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("BOV", currency.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("BOV", currency.CurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("BOV", currency.ToString());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, currency.IsFund);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, currency.IsCurrent);
		}

		public void CurrencyIsoSymbolAndNumber003()
		{
			var currency = new CurrencyInfo("XRE");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("XRE", currency.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("XRE", currency.CurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("XRE", currency.ToString());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, currency.IsFund);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(false, currency.IsCurrent);
		}

		public void CurrencyIsoSymbolAndNumber004()
		{
			var currency = new CurrencyInfo(new System.Globalization.RegionInfo("en-US"));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("USD", currency.ISOCurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("$", currency.CurrencySymbol);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("USD", currency.ToString());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(false, currency.IsFund);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(true, currency.IsCurrent);
		}
	}
}