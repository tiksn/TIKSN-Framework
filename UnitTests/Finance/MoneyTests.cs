using System;
using System.Globalization;
using System.Threading;
using Xunit;

namespace TIKSN.Finance.Tests
{
    public class MoneyTests
    {
        [Fact]
        public void ATMWithdraw001()
        {
            System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("hy-AM");
            CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);

            Money Account = new Money(ArmenianDram, 150600);
            Money Requested = new Money(ArmenianDram, 5600);

            Money Withdrawal = Requested - Requested % 1000;
            Account = Account - Withdrawal;

            Assert.Equal<decimal>(145600m, Account.Amount);
        }

        [Fact]
        public void ATMWithdraw002()
        {
            System.Globalization.RegionInfo Spain = new System.Globalization.RegionInfo("ES");
            CurrencyInfo Euro = new CurrencyInfo(Spain);

            Money Account = new Money(Euro, 2520.2m);
            Money Requested = new Money(Euro, 6.8m);

            Money Withdrawal = Requested - Requested % 0.5m;
            Account -= Withdrawal;

            Assert.Equal<decimal>(2513.7m, Account.Amount);
        }

        [Fact]
        public void BuyAndSell001()
        {
            System.Globalization.RegionInfo Italy = new System.Globalization.RegionInfo("IT");
            CurrencyInfo Euro = new CurrencyInfo(Italy);

            Money BuyerWallet = new Money(Euro, 650m);
            Money SellerWallet = new Money(Euro, 1256.7m);
            Money Payment = new Money(Euro, 1.50m);

            BuyerWallet -= Payment;
            SellerWallet += Payment;

            Assert.Equal<decimal>(648.5m, BuyerWallet.Amount);
            Assert.Equal<decimal>(1258.2m, SellerWallet.Amount);
        }

        [Fact]
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

            Assert.Equal<decimal>(1950m, BuyerWallet.Amount);
        }

        [Fact]
        public void Commonwealth001()
        {
            System.Globalization.RegionInfo Russia = new System.Globalization.RegionInfo("RU");
            CurrencyInfo RussianRuble = new CurrencyInfo(Russia);

            Money Ivan = new Money(RussianRuble, 1250m);
            Money Olga = new Money(RussianRuble, 2360m);

            Money Commonwealth = (Ivan + Olga) / 2;

            Assert.Equal<decimal>(1805m, Commonwealth.Amount);
        }

        [Fact]
        public void Commonwealth002()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money BritainWealth = new Money(Pound, 356000000000m);

            Money EnglandWealth = BritainWealth * 45m / 100m;
            Money ScotlandWealth = BritainWealth * 36m / 100m;

            Assert.Equal<decimal>(160200000000m, EnglandWealth.Amount);
            Assert.Equal<decimal>(128160000000m, ScotlandWealth.Amount);
        }

        [Fact]
        public void CompareDifferentCurrency001()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Wallet1 = new Money(Franc, 12.6m);
            Money Wallet2 = new Money(Krone, 12.6m);

            Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1.CompareTo(Wallet2));
        }

        [Fact]
        public void CompareDifferentCurrency002()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Wallet1 = new Money(Franc, 12.6m);
            Money Wallet2 = new Money(Krone, 12.6m);

            Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1.Equals(Wallet2));
        }

        [Fact]
        public void CompareDifferentCurrency003()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Wallet1 = new Money(Franc, 12.6m);
            Money Wallet2 = new Money(Krone, 12.6m);

            Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 > Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency004()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Wallet1 = new Money(Franc, 12.6m);
            Money Wallet2 = new Money(Krone, 12.6m);

            Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 < Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency005()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Wallet1 = new Money(Franc, 12.6m);
            Money Wallet2 = new Money(Krone, 12.6m);

            Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 >= Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency006()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Wallet1 = new Money(Franc, 12.6m);
            Money Wallet2 = new Money(Krone, 12.6m);

            Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 <= Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency007()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Wallet1 = new Money(Franc, 12.6m);
            Money Wallet2 = new Money(Krone, 12.6m);

            Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 == Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency008()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Wallet1 = new Money(Franc, 12.6m);
            Money Wallet2 = new Money(Krone, 12.6m);

            Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 != Wallet2);
        }

        [Fact]
        public void CreateMoney001()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo USDollar = new CurrencyInfo(UnitedStates);

            Money payment = new Money(USDollar);

            Assert.Equal<decimal>(decimal.Zero, payment.Amount);
            Assert.Equal<CurrencyInfo>(USDollar, payment.Currency);
            Assert.Equal("USD", payment.Currency.ISOCurrencySymbol);
        }

        [Fact]
        public void CreateMoney002()
        {
            System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
            CurrencyInfo ArmenianDram = new CurrencyInfo(Armenia);

            Money payment = new Money(ArmenianDram, 1000m);

            Assert.Equal<decimal>(1000m, payment.Amount);
            Assert.Equal<CurrencyInfo>(ArmenianDram, payment.Currency);
            Assert.Equal("AMD", payment.Currency.ISOCurrencySymbol);
        }

        [Fact]
        public void Equality001()
        {
            System.Globalization.RegionInfo UnitedStates1 = new System.Globalization.RegionInfo("en-US");
            System.Globalization.RegionInfo UnitedStates2 = new System.Globalization.RegionInfo("es-US");

            CurrencyInfo Dollar1 = new CurrencyInfo(UnitedStates1);
            CurrencyInfo Dollar2 = new CurrencyInfo(UnitedStates2);

            Money Money1 = new Money(Dollar1, 125.6m);
            Money Money2 = new Money(Dollar2, 125.6m);

            Assert.True(Money1 == Money2);
            Assert.False(Money1 != Money2);
            Assert.True(Money1.Equals(Money2));
            Assert.True(Money2.Equals(Money1));
            Assert.Equal(0, Money1.CompareTo(Money2));
            Assert.Equal(0, Money2.CompareTo(Money1));
        }

        [Fact]
        public void Equals001()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("en-US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money M = new Money(Dollar, 145.6m);

            Assert.True(M.Equals(M));
        }

        [Fact]
        public void Equals002()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("en-US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money M = new Money(Dollar, 145.6m);

            Assert.False(M.Equals(null));
        }

        [Fact]
        public void Equals003()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("en-US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money M1 = new Money(Dollar, 145.6m);
            object M2 = new Money(Dollar, 145.6m);

            Assert.True(M1.Equals(M2));
        }

        [Fact]
        public void GreaterThan001()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money Hundred = new Money(Dollar, 100m);
            Money Thousand = new Money(Dollar, 1000m);

            Assert.True(Thousand > Hundred);
            Assert.False(Thousand < Hundred);
        }

        [Fact]
        public void GreaterThanOrEqual001()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money M1 = new Money(Dollar, 100m);
            Money M2 = new Money(Dollar, 1000m);

            Assert.False(M1 >= M2);
            Assert.True(M2 >= M1);
            Assert.False(M1.CompareTo(M2) >= 0);
            Assert.True(M2.CompareTo(M1) >= 0);
        }

        [Fact]
        public void GreaterThanOrEqual002()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money M1 = new Money(Dollar, 100m);
            Money M2 = new Money(Dollar, 100m);

            Assert.True(M1 >= M2);
            Assert.True(M2 >= M1);
            Assert.True(M1.CompareTo(M2) >= 0);
            Assert.True(M2.CompareTo(M1) >= 0);
        }

        [Fact]
        public void Inequality001()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money Hundred = new Money(Dollar, 100m);
            Money Thousand = new Money(Dollar, 1000m);

            Assert.False(Hundred == Thousand);
            Assert.True(Hundred != Thousand);
            Assert.False(Hundred.Equals(Thousand));
            Assert.False(Thousand.Equals(Hundred));
            Assert.NotEqual(0, Hundred.CompareTo(Thousand));
            Assert.NotEqual(0, Thousand.CompareTo(Hundred));
        }

        [Fact]
        public void LessThan001()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money Hundred = new Money(Dollar, 100m);
            Money Thousand = new Money(Dollar, 1000m);

            Assert.True(Hundred < Thousand);
            Assert.False(Hundred > Thousand);
            Assert.True(Hundred.CompareTo(Thousand) < 0);
            Assert.True(Thousand.CompareTo(Hundred) > 0);
        }

        [Fact]
        public void LessThanOrEqual001()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money Hundred = new Money(Dollar, 100m);
            Money Thousand = new Money(Dollar, 1000m);

            Assert.True(Hundred <= Thousand);
            Assert.False(Thousand <= Hundred);
            Assert.True(Hundred.CompareTo(Thousand) <= 0);
        }

        [Fact]
        public void LessThanOrEqual002()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money M1 = new Money(Dollar, 100m);
            Money M2 = new Money(Dollar, 100m);

            Assert.True(M1 <= M2);
            Assert.True(M2 <= M1);
            Assert.True(M1.CompareTo(M2) <= 0);
            Assert.True(M2.CompareTo(M2) <= 0);
        }

        [Fact]
        public void LessThanOrEqual003()
        {
            System.Globalization.RegionInfo UnitedStates = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(UnitedStates);

            Money M1 = new Money(Dollar, 100m);
            Money M2 = new Money(Dollar, 1000m);

            Assert.True(M1 <= M2);
            Assert.False(M2 <= M1);
            Assert.True(M1.CompareTo(M2) <= 0);
            Assert.False(M2.CompareTo(M1) <= 0);
        }

        [Fact]
        public void OperatorDivision001()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 1520m);

            sbyte part = 4;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(380m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision002()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 650m);

            byte part = 5;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(130m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision003()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 420m);

            short part = 6;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(70m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision004()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 712m);

            ushort part = 10;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(71.2m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision005()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 1260m);

            int part = 42;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(30.0m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision006()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 252m);

            uint part = 6;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(42m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision007()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 48m);

            long part = 12;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(4m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision008()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 43.2m);

            ulong part = 36;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(1.2m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision009()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 176m);

            decimal part = 88m;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(2m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision010()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 882m);

            decimal part = 42m;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(21m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision011()
        {
            System.Globalization.RegionInfo Switzerland = new System.Globalization.RegionInfo("CH");
            CurrencyInfo Franc = new CurrencyInfo(Switzerland);

            Money Budget = new Money(Franc, 744m);

            decimal part = 62m;

            Money Fund = Budget / part;

            Assert.Equal<decimal>(12m, Fund.Amount);
        }

        [Fact]
        public void OperatorModulus001()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 30m);

            sbyte whole = 12;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(6m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus002()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 62m);

            byte whole = 10;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(2m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus003()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 62m);

            short whole = 9;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(8m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus004()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 135m);

            ushort whole = 25;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(10m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus005()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 98m);

            int whole = 264;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(98m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus006()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 42.2m);

            uint whole = 26;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(16.2m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus007()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 6568.8m);

            long whole = 464;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(72.8m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus008()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 89.9m);

            ulong whole = 64;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(25.9m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus009()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 2500m);

            decimal whole = 64.7m;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(41.4m, Truncated.Amount);
            Assert.True(Truncated.Amount < (decimal)whole);
        }

        [Fact]
        public void OperatorModulus010()
        {
            System.Globalization.RegionInfo Denmark = new System.Globalization.RegionInfo("DK");
            CurrencyInfo Krone = new CurrencyInfo(Denmark);

            Money Budget = new Money(Krone, 4526m);

            decimal whole = 24.4m;

            Money Truncated = Budget % whole;

            Assert.Equal<decimal>(12m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorMultiply001()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 12.5m);

            sbyte m = 23;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(287.5m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply002()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 22.5m);

            byte m = 24;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(540m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply003()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 26m);

            short m = 240;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(6240m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply004()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 250m);

            ushort m = 667;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(166750m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply005()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 24.4m);

            uint m = 598;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(14591.2m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply006()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 4.44m);

            long m = 662;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(2939.28m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply007()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 88.2m);

            ulong m = 42;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(3704.4m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply008()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 242m);

            decimal m = 6.2m;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(1500.4m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply009()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 56m);

            decimal m = 8.4m;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(470.4m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply010()
        {
            System.Globalization.RegionInfo Britain = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(Britain);

            Money Payment = new Money(Pound, 69m);

            decimal m = 1.44m;

            Money Wallet = Payment * m;

            Assert.Equal<decimal>(99.36m, Wallet.Amount);
        }

        [Fact]
        public void ToString001()
        {
            var stringValue = string.Empty;

            var thread = new Thread(() =>
            {
                var USA = new RegionInfo("US");
                var Dollar = new CurrencyInfo(USA);

                var Price = new Money(Dollar, 16.6m);

                var CI = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = CI;
                stringValue = Price.ToString();
            });

            thread.Start();
            thread.Join();

            Assert.Equal("$16.60", stringValue);
        }

        [Fact]
        public void ToString002()
        {
            throw new NotImplementedException();
            //         System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            //CurrencyInfo Dollar = new CurrencyInfo(USA);

            //Money Price = new Money(Dollar, 24.4m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-GB");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("en-GB", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("$24.40", Price.ToString());
        }

        [Fact]
        public void ToString003()
        {
            throw new NotImplementedException();
            //         System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            //CurrencyInfo Dollar = new CurrencyInfo(USA);

            //Money Price = new Money(Dollar, 62.68m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("de-DE");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("de-DE", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("62,68 $", Price.ToString());
        }

        [Fact]
        public void ToString004()
        {
            throw new NotImplementedException();
            //         System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            //CurrencyInfo Dollar = new CurrencyInfo(USA);

            //Money Price = new Money(Dollar, 12.36m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ga-IE");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("ga-IE", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("$12.36", Price.ToString());
        }

        [Fact]
        public void ToString005()
        {
            throw new NotImplementedException();

            //         System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            //CurrencyInfo Dollar = new CurrencyInfo(USA);

            //Money Price = new Money(Dollar, 5.6m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("nl-NL");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("nl-NL", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("$ 5,60", Price.ToString());
        }

        [Fact]
        public void ToString006()
        {
            throw new NotImplementedException();
            //         System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            //CurrencyInfo Dollar = new CurrencyInfo(USA);

            //Money Price = new Money(Dollar, 36.6m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("hy-AM");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("hy-AM", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("36.60 $", Price.ToString());
        }

        [Fact]
        public void ToString007()
        {
            throw new NotImplementedException();

            //         System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
            //CurrencyInfo Euro = new CurrencyInfo(Germany);

            //Money Price = new Money(Euro, 58.7m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("en-US", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("€58.70", Price.ToString());
        }

        [Fact]
        public void ToString008()
        {
            throw new NotImplementedException();

            //         System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
            //CurrencyInfo Euro = new CurrencyInfo(Germany);

            //Money Price = new Money(Euro, 12.4m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-GB");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("en-GB", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("€12.40", Price.ToString());
        }

        [Fact]
        public void ToString009()
        {
            throw new NotImplementedException();

            //         System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
            //CurrencyInfo Euro = new CurrencyInfo(Germany);

            //Money Price = new Money(Euro, 14.24m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ga-IE");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("ga-IE", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("€14.24", Price.ToString());
        }

        [Fact]
        public void ToString010()
        {
            throw new NotImplementedException();

            //         System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
            //CurrencyInfo Euro = new CurrencyInfo(Germany);

            //Money Price = new Money(Euro, 16.6m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("nl-NL");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("nl-NL", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("€ 16,60", Price.ToString());
        }

        [Fact]
        public void ToString011()
        {
            throw new NotImplementedException();

            //         System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
            //CurrencyInfo Euro = new CurrencyInfo(Germany);

            //Money Price = new Money(Euro, 14.5m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("hy-AM");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("hy-AM", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("14.50 €", Price.ToString());
        }

        [Fact]
        public void ToString012()
        {
            throw new NotImplementedException();

            //         System.Globalization.RegionInfo Germany = new System.Globalization.RegionInfo("DE");
            //CurrencyInfo Euro = new CurrencyInfo(Germany);

            //Money Price = new Money(Euro, 16.5m);

            //System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("de-DE");
            //System.Threading.Thread.CurrentThread.CurrentCulture = CI;

            //Assert.Equal("de-DE", System.Globalization.CultureInfo.CurrentCulture.Name);

            //Assert.Equal("16,50 €", Price.ToString());
        }

        [Fact]
        public void ToString013()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 56.42m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-US");

            Assert.Equal("£56.42", Price.ToString("S", CI));
        }

        [Fact]
        public void ToString014()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 14.6m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("en-GB");

            Assert.Equal("£14.60", Price.ToString("S2", CI));
        }

        [Fact]
        public void ToString015()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 25.96m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("de-DE");

            Assert.Equal("25,96000 £", Price.ToString("S5", CI));
        }

        [Fact]
        public void ToString016()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 12.21m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("ga-IE");

            Assert.Equal("£12.2", Price.ToString("S1", CI));
        }

        [Fact]
        public void ToString017()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 16.36m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("nl-NL");

            Assert.Equal("£ 16,4", Price.ToString("S1", CI));
        }

        [Fact]
        public void ToString018()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 66.326m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("hy-AM");

            Assert.Equal("66.3 £", Price.ToString("S1", CI));
        }

        [Fact]
        public void ToString019()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 162.2m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("el-GR");

            Assert.Equal("162,20 £", Price.ToString("", CI));
        }

        [Fact]
        public void ToString020()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 142.26m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("it-IT");

            Assert.Equal("£ 142,26", Price.ToString(string.Empty, CI));
        }

        [Fact]
        public void ToString021()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 66.32m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("sv-SE");

            Assert.Equal("66,32 £", Price.ToString(null, CI));
        }

        [Fact]
        public void ToString022()
        {
            System.Globalization.RegionInfo UnitedKingdom = new System.Globalization.RegionInfo("GB");
            CurrencyInfo Pound = new CurrencyInfo(UnitedKingdom);

            Money Price = new Money(Pound, 66.32m);

            System.Globalization.CultureInfo CI = new System.Globalization.CultureInfo("pl-PL");

            Assert.Throws<FormatException>(
                   () =>
                        Price.ToString("K", CI));
        }

        [Fact]
        public void ToString023()
        {
            System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
            CurrencyInfo Dram = new CurrencyInfo(Armenia);

            Money Price = new Money(Dram, 2500m);

            System.Globalization.CultureInfo DispalyCulture = new System.Globalization.CultureInfo("en-US");

            Assert.Equal("AMD2,500.00", Price.ToString("I", DispalyCulture));
        }

        [Fact]
        public void ToString024()
        {
            System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
            CurrencyInfo Dram = new CurrencyInfo(Armenia);

            Money Price = new Money(Dram, 2500.2m);

            System.Globalization.CultureInfo DispalyCulture = new System.Globalization.CultureInfo("en-GB");

            Assert.Equal("AMD2,500.200", Price.ToString("I3", DispalyCulture));
        }

        [Fact]
        public void ToString025()
        {
            throw new NotImplementedException();

            //         System.Globalization.RegionInfo Armenia = new System.Globalization.RegionInfo("AM");
            //CurrencyInfo Dram = new CurrencyInfo(Armenia);

            //Money Price = new Money(Dram, 2500.24m);

            //System.Globalization.CultureInfo DispalyCulture = new System.Globalization.CultureInfo("de-DE");
            //System.Threading.Thread.CurrentThread.CurrentCulture = DispalyCulture;

            //Assert.Equal("de-DE", System.Globalization.CultureInfo.CurrentCulture.Name);
            //Assert.Equal("2.500,2 AMD", Price.ToString("I1"));
        }

        [Fact]
        public void UnaryNegation001()
        {
            System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(USA);

            Money M1 = new Money(Dollar, 235.6m);
            Money M2 = -M1;

            Assert.Equal<CurrencyInfo>(M1.Currency, M2.Currency);
            Assert.Equal<decimal>(-235.6m, M2.Amount);
        }

        [Fact]
        public void UnaryPlus001()
        {
            System.Globalization.RegionInfo USA = new System.Globalization.RegionInfo("US");
            CurrencyInfo Dollar = new CurrencyInfo(USA);

            Money M1 = new Money(Dollar, 235.6m);
            Money M2 = +M1;

            Assert.Equal<CurrencyInfo>(M1.Currency, M2.Currency);
            Assert.Equal<decimal>(235.6m, M2.Amount);
        }
    }
}