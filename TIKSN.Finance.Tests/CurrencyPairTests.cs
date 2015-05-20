namespace TIKSN.Finance.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class CurrencyPairTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CurrencyPair001()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo USD1 = new CurrencyInfo(USA);
			CurrencyInfo USD2 = new CurrencyInfo(USA);

			try
			{
				CurrencyPair pair = new CurrencyPair(USD1, USD2);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.ArgumentException)
			{
			}
			catch
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.Equals(pair1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.GetHashCode() == pair1.GetHashCode());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1 == pair1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1 != pair1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair2.Equals(pair1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.Equals(pair2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.GetHashCode() == pair2.GetHashCode());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair2.GetHashCode() == pair1.GetHashCode());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1 == pair2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair2 == pair1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1 != pair2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair2 != pair1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1.Equals(pair3));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair3.Equals(pair1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1.GetHashCode() == pair3.GetHashCode());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair3.GetHashCode() == pair1.GetHashCode());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1 == pair3);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair3 == pair1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1 != pair3);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair3 != pair1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair4.Equals(pair5));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair5.Equals(pair4));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair4.GetHashCode() == pair5.GetHashCode());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair5.GetHashCode() == pair4.GetHashCode());
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair4 == pair5);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair5 == pair4);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair4 != pair5);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.Equals(pair2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.Equals(pair3));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1 == pair2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair2 == pair1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1 != pair2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair2 != pair1);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1.Equals(something));

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1.Equals(pair4));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
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

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1.Equals(pair2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1.Equals(pair3));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair1 == pair2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair2 == pair1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair2 == pair4);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair1 != pair2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(pair2 != pair1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(pair2 != pair4);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString001()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			System.Globalization.RegionInfo GBR = new System.Globalization.RegionInfo("GB");

			CurrencyInfo USD = new CurrencyInfo(USA);
			CurrencyInfo GBP = new CurrencyInfo(GBR);

			CurrencyPair pair = new CurrencyPair(GBP, USD);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("GBP/USD", pair.ToString());
		}
	}
}