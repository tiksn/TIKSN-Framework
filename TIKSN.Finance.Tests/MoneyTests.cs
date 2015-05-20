namespace TIKSN.Finance.Tests
{
	[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
	public class MoneyTests
	{
		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ATMWithdraw001()
		{
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("hy-AM");
			CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);

			Money Account = new Money(ArmenianDram, 150600);
			Money Requested = new Money(ArmenianDram, 5600);

			Money Withdrawal = Requested - Requested % 1000;
			Account = Account - Withdrawal;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(145600m, Account.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ATMWithdraw002()
		{
			System.Globalization.RegionInfo Spain = new System.Globalization.RegionInfo("ES");
			CurrencyInfo Euro = new CurrencyInfo(Spain);

			Money Account = new Money(Euro, 2520.2m);
			Money Requested = new Money(Euro, 6.8m);

			Money Withdrawal = Requested - Requested % 0.5m;
			Account -= Withdrawal;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(2513.7m, Account.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void BuyAndSell001()
		{
			System.Globalization.RegionInfo Italy = new System.Globalization.RegionInfo("IT");
			CurrencyInfo Euro = new CurrencyInfo(Italy);

			Money BuyerWallet = new Money(Euro, 650m);
			Money SellerWallet = new Money(Euro, 1256.7m);
			Money Payment = new Money(Euro, 1.50m);

			BuyerWallet -= Payment;
			SellerWallet += Payment;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(648.5m, BuyerWallet.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(1258.2m, SellerWallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void BuyAndSell002()
		{
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("hy-AM");
			CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);

			Money BuyerWallet = new Money(ArmenianDram, 5000m);

			Money BreadPrice = new Money(ArmenianDram, 250);
			int BreadQuantity = 4;

			Money MilkPrice = new Money(ArmenianDram, 350);
			decimal MilkBottleVolume = 1.5m;
			int MilkQuantity = 2;

			Money ButterPrice = new Money(ArmenianDram, 2500);
			decimal ButterPart = 2.5m;

			BuyerWallet -= BreadPrice * BreadQuantity + MilkPrice * MilkBottleVolume * MilkQuantity + ButterPrice / ButterPart;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(1950m, BuyerWallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Commonwealth001()
		{
			System.Globalization.RegionInfo Russia = new System.Globalization.RegionInfo("RU");
			CurrencyInfo RussianRuble = new CurrencyInfo(Russia);

			Money Ivan = new Money(RussianRuble, 1250m);
			Money Olga = new Money(RussianRuble, 2360m);

			Money Commonwealth = (Ivan + Olga) / 2;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(1805m, Commonwealth.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Commonwealth002()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money BritainWealth = new Money(Pound, 356000000000m);

			Money EnglandWealth = BritainWealth * 45m / 100m;
			Money ScotlandWealth = BritainWealth * 36m / 100m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(160200000000m, EnglandWealth.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(128160000000m, ScotlandWealth.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CompareDifferentCurrency001()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Wallet1 = new Money(Franc, 12.6m);
			Money Wallet2 = new Money(Krone, 12.6m);

			try
			{
				Wallet1.CompareTo(Wallet2);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.InvalidOperationException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CompareDifferentCurrency002()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Wallet1 = new Money(Franc, 12.6m);
			Money Wallet2 = new Money(Krone, 12.6m);

			try
			{
				Wallet1.Equals(Wallet2);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.InvalidOperationException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CompareDifferentCurrency003()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Wallet1 = new Money(Franc, 12.6m);
			Money Wallet2 = new Money(Krone, 12.6m);

			try
			{
				bool result = Wallet1 > Wallet2;

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.InvalidOperationException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CompareDifferentCurrency004()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Wallet1 = new Money(Franc, 12.6m);
			Money Wallet2 = new Money(Krone, 12.6m);

			try
			{
				bool result = Wallet1 < Wallet2;

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.InvalidOperationException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CompareDifferentCurrency005()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Wallet1 = new Money(Franc, 12.6m);
			Money Wallet2 = new Money(Krone, 12.6m);

			try
			{
				bool result = Wallet1 >= Wallet2;

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.InvalidOperationException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CompareDifferentCurrency006()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Wallet1 = new Money(Franc, 12.6m);
			Money Wallet2 = new Money(Krone, 12.6m);

			try
			{
				bool result = Wallet1 <= Wallet2;

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.InvalidOperationException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CompareDifferentCurrency007()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Wallet1 = new Money(Franc, 12.6m);
			Money Wallet2 = new Money(Krone, 12.6m);

			try
			{
				bool result = Wallet1 == Wallet2;

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.InvalidOperationException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CompareDifferentCurrency008()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Wallet1 = new Money(Franc, 12.6m);
			Money Wallet2 = new Money(Krone, 12.6m);

			try
			{
				bool result = Wallet1 != Wallet2;

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.InvalidOperationException)
			{
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateMoney001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);

			Money payment = new Money(USDollar);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(decimal.Zero, payment.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(USDollar, payment.Currency);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("USD", payment.Currency.ISOCurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void CreateMoney002()
		{
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
			CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);

			Money payment = new Money(ArmenianDram, 1000m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(1000m, payment.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(ArmenianDram, payment.Currency);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("AMD", payment.Currency.ISOCurrencySymbol);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equality001()
		{
			System.Globalization.RegionInfo UnitedStates1 = new System.Globalization.RegionInfo("en-US");
			System.Globalization.RegionInfo UnitedStates2 = new System.Globalization.RegionInfo("es-US");

			CurrencyInfo Dollar1 = new CurrencyInfo(UnitedStates1);
			CurrencyInfo Dollar2 = new CurrencyInfo(UnitedStates2);

			Money Money1 = new Money(Dollar1, 125.6m);
			Money Money2 = new Money(Dollar2, 125.6m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Money1 == Money2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Money1 != Money2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Money1.Equals(Money2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Money2.Equals(Money1));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(0, Money1.CompareTo(Money2));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<int>(0, Money2.CompareTo(Money1));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("en-US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money M = new Money(Dollar, 145.6m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M.Equals(M));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals002()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("en-US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money M = new Money(Dollar, 145.6m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(M.Equals(null));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Equals003()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("en-US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money M1 = new Money(Dollar, 145.6m);
			object M2 = new Money(Dollar, 145.6m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M1.Equals(M2));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GreaterThan001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money Hundred = new Money(Dollar, 100m);
			Money Thousand = new Money(Dollar, 1000m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Thousand > Hundred);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Thousand < Hundred);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GreaterThanOrEqual001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money M1 = new Money(Dollar, 100m);
			Money M2 = new Money(Dollar, 1000m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(M1 >= M2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M2 >= M1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(M1.CompareTo(M2) >= 0);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M2.CompareTo(M1) >= 0);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void GreaterThanOrEqual002()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money M1 = new Money(Dollar, 100m);
			Money M2 = new Money(Dollar, 100m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M1 >= M2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M2 >= M1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M1.CompareTo(M2) >= 0);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M2.CompareTo(M1) >= 0);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void Inequality001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money Hundred = new Money(Dollar, 100m);
			Money Thousand = new Money(Dollar, 1000m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Hundred == Thousand);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Hundred != Thousand);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Hundred.Equals(Thousand));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Thousand.Equals(Hundred));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual<int>(0, Hundred.CompareTo(Thousand));
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual<int>(0, Thousand.CompareTo(Hundred));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void LessThan001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money Hundred = new Money(Dollar, 100m);
			Money Thousand = new Money(Dollar, 1000m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Hundred < Thousand);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Hundred > Thousand);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Hundred.CompareTo(Thousand) < 0);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Thousand.CompareTo(Hundred) > 0);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void LessThanOrEqual001()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money Hundred = new Money(Dollar, 100m);
			Money Thousand = new Money(Dollar, 1000m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Hundred <= Thousand);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(Thousand <= Hundred);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Hundred.CompareTo(Thousand) <= 0);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void LessThanOrEqual002()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money M1 = new Money(Dollar, 100m);
			Money M2 = new Money(Dollar, 100m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M1 <= M2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M2 <= M1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M1.CompareTo(M2) <= 0);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M2.CompareTo(M2) <= 0);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void LessThanOrEqual003()
		{
			System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

			Money M1 = new Money(Dollar, 100m);
			Money M2 = new Money(Dollar, 1000m);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M1 <= M2);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(M2 <= M1);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(M1.CompareTo(M2) <= 0);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(M2.CompareTo(M1) <= 0);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision001()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 1520m);

			sbyte part = 4;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(380m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision002()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 650m);

			byte part = 5;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(130m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision003()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 420m);

			short part = 6;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(70m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision004()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 712m);

			ushort part = 10;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(71.2m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision005()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 1260m);

			int part = 42;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(30.0m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision006()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 252m);

			uint part = 6;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(42m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision007()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 48m);

			long part = 12;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(4m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision008()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 43.2m);

			ulong part = 36;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(1.2m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision009()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 176m);

			decimal part = 88m;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(2m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision010()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 882m);

			decimal part = 42m;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(21m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorDivision011()
		{
			System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
			CurrencyInfo Franc = new CurrencyInfo(Switzerland);

			Money Budget = new Money(Franc, 744m);

			decimal part = 62m;

			Money Fund = Budget / part;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(12m, Fund.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus001()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 30m);

			sbyte whole = 12;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(6m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus002()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 62m);

			byte whole = 10;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(2m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus003()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 62m);

			short whole = 9;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(8m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus004()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 135m);

			ushort whole = 25;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(10m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus005()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 98m);

			int whole = 264;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(98m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus006()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 42.2m);

			uint whole = 26;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(16.2m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus007()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 6568.8m);

			long whole = 464;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(72.8m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus008()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 89.9m);

			ulong whole = 64;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(25.9m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus009()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 2500m);

			decimal whole = 64.7m;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(41.4m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < (decimal)whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorModulus010()
		{
			System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
			CurrencyInfo Krone = new CurrencyInfo(Denmark);

			Money Budget = new Money(Krone, 4526m);

			decimal whole = 24.4m;

			Money Truncated = Budget % whole;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(12m, Truncated.Amount);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(Truncated.Amount < whole);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply001()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 12.5m);

			sbyte m = 23;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(287.5m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply002()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 22.5m);

			byte m = 24;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(540m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply003()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 26m);

			short m = 240;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(6240m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply004()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 250m);

			ushort m = 667;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(166750m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply005()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 24.4m);

			uint m = 598;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(14591.2m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply006()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 4.44m);

			long m = 662;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(2939.28m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply007()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 88.2m);

			ulong m = 42;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(3704.4m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply008()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 242m);

			decimal m = 6.2m;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(1500.4m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply009()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 56m);

			decimal m = 8.4m;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(470.4m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void OperatorMultiply010()
		{
			System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(Britain);

			Money Payment = new Money(Pound, 69m);

			decimal m = 1.44m;

			Money Wallet = Payment * m;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(99.36m, Wallet.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString001()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(USA);

			Money Price = new Money(Dollar, 16.6m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-US");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("en-US", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$16.60", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString002()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(USA);

			Money Price = new Money(Dollar, 24.4m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-GB");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("en-GB", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$24.40", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString003()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(USA);

			Money Price = new Money(Dollar, 62.68m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("de-DE");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("de-DE", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("62,68 $", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString004()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(USA);

			Money Price = new Money(Dollar, 12.36m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ga-IE");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("ga-IE", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$12.36", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString005()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(USA);

			Money Price = new Money(Dollar, 5.6m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("nl-NL");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("nl-NL", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("$ 5,60", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString006()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(USA);

			Money Price = new Money(Dollar, 36.6m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("hy-AM");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("hy-AM", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("36.60 $", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString007()
		{
			System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
			CurrencyInfo Euro = new CurrencyInfo(Germany);

			Money Price = new Money(Euro, 58.7m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-US");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("en-US", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("€58.70", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString008()
		{
			System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
			CurrencyInfo Euro = new CurrencyInfo(Germany);

			Money Price = new Money(Euro, 12.4m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-GB");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("en-GB", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("€12.40", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString009()
		{
			System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
			CurrencyInfo Euro = new CurrencyInfo(Germany);

			Money Price = new Money(Euro, 14.24m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ga-IE");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("ga-IE", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("€14.24", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString010()
		{
			System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
			CurrencyInfo Euro = new CurrencyInfo(Germany);

			Money Price = new Money(Euro, 16.6m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("nl-NL");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("nl-NL", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("€ 16,60", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString011()
		{
			System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
			CurrencyInfo Euro = new CurrencyInfo(Germany);

			Money Price = new Money(Euro, 14.5m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("hy-AM");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("hy-AM", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("14.50 €", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString012()
		{
			System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
			CurrencyInfo Euro = new CurrencyInfo(Germany);

			Money Price = new Money(Euro, 16.5m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("de-DE");
			System.Threading.Thread.CurrentThread.CurrentCulture = CI;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("de-DE", System.Globalization.CultureInfo.CurrentCulture.Name);

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("16,50 €", Price.ToString());
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString013()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 56.42m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-US");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("£56.42", Price.ToString("S", CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString014()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 14.6m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-GB");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("£14.60", Price.ToString("S2", CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString015()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 25.96m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("de-DE");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("25,96000 £", Price.ToString("S5", CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString016()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 12.21m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ga-IE");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("£12.2", Price.ToString("S1", CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString017()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 16.36m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("nl-NL");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("£ 16,4", Price.ToString("S1", CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString018()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 66.326m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("hy-AM");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("66.3 £", Price.ToString("S1", CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString019()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 162.2m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("el-GR");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("162,20 £", Price.ToString("", CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString020()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 142.26m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("it-IT");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("£ 142,26", Price.ToString(string.Empty, CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString021()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 66.32m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("sv-SE");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("66,32 £", Price.ToString(null, CI));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString022()
		{
			System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
			CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

			Money Price = new Money(Pound, 66.32m);

			System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("pl-PL");

			try
			{
				string text = Price.ToString("K", CI);

				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
			catch (System.FormatException)
			{
			}
			catch (System.Exception)
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail();
			}
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString023()
		{
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Price = new Money(Dram, 2500m);

			System.Globalization.CultureInfo DispalyCulture = new System.Globalization.CultureInfo("en-US");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("AMD2,500.00", Price.ToString("I", DispalyCulture));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString024()
		{
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Price = new Money(Dram, 2500.2m);

			System.Globalization.CultureInfo DispalyCulture = new System.Globalization.CultureInfo("en-GB");

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("AMD2,500.200", Price.ToString("I3", DispalyCulture));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void ToString025()
		{
			System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
			CurrencyInfo Dram = new CurrencyInfo(Armenia);

			Money Price = new Money(Dram, 2500.24m);

			System.Globalization.CultureInfo DispalyCulture = new System.Globalization.CultureInfo("de-DE");
			System.Threading.Thread.CurrentThread.CurrentCulture = DispalyCulture;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("de-DE", System.Globalization.CultureInfo.CurrentCulture.Name);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<string>("2.500,2 AMD", Price.ToString("I1"));
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void UnaryNegation001()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(USA);

			Money M1 = new Money(Dollar, 235.6m);
			Money M2 = -M1;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(M1.Currency, M2.Currency);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(-235.6m, M2.Amount);
		}

		[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
		public void UnaryPlus001()
		{
			System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
			CurrencyInfo Dollar = new CurrencyInfo(USA);

			Money M1 = new Money(Dollar, 235.6m);
			Money M2 = +M1;

			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<CurrencyInfo>(M1.Currency, M2.Currency);
			Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<decimal>(235.6m, M2.Amount);
		}
	}
}