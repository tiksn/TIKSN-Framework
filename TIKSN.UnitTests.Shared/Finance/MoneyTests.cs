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
            var Armenia = new RegionInfo("hy-AM");
            var ArmenianDram = new CurrencyInfo(Armenia);

            var Account = new Money(ArmenianDram, 150600);
            var Requested = new Money(ArmenianDram, 5600);

            var Withdrawal = Requested - (Requested % 1000);
            Account -= Withdrawal;

            Assert.Equal(145600m, Account.Amount);
        }

        [Fact]
        public void ATMWithdraw002()
        {
            var Spain = new RegionInfo("ES");
            var Euro = new CurrencyInfo(Spain);

            var Account = new Money(Euro, 2520.2m);
            var Requested = new Money(Euro, 6.8m);

            var Withdrawal = Requested - (Requested % 0.5m);
            Account -= Withdrawal;

            Assert.Equal(2513.7m, Account.Amount);
        }

        [Fact]
        public void BuyAndSell001()
        {
            var Italy = new RegionInfo("IT");
            var Euro = new CurrencyInfo(Italy);

            var BuyerWallet = new Money(Euro, 650m);
            var SellerWallet = new Money(Euro, 1256.7m);
            var Payment = new Money(Euro, 1.50m);

            BuyerWallet -= Payment;
            SellerWallet += Payment;

            Assert.Equal(648.5m, BuyerWallet.Amount);
            Assert.Equal(1258.2m, SellerWallet.Amount);
        }

        [Fact]
        public void BuyAndSell002()
        {
            var Armenia = new RegionInfo("hy-AM");
            var ArmenianDram = new CurrencyInfo(Armenia);

            var BuyerWallet = new Money(ArmenianDram, 5000m);

            var BreadPrice = new Money(ArmenianDram, 250);
            var BreadQuantity = 4;

            var MilkPrice = new Money(ArmenianDram, 350);
            var MilkBottleVolume = 1.5m;
            var MilkQuantity = 2;

            var ButterPrice = new Money(ArmenianDram, 2500);
            var ButterPart = 2.5m;

            BuyerWallet -= (BreadPrice * BreadQuantity) + (MilkPrice * MilkBottleVolume * MilkQuantity) + (ButterPrice / ButterPart);

            Assert.Equal(1950m, BuyerWallet.Amount);
        }

        [Fact]
        public void Commonwealth001()
        {
            var Russia = new RegionInfo("RU");
            var RussianRuble = new CurrencyInfo(Russia);

            var Ivan = new Money(RussianRuble, 1250m);
            var Olga = new Money(RussianRuble, 2360m);

            var Commonwealth = (Ivan + Olga) / 2;

            Assert.Equal(1805m, Commonwealth.Amount);
        }

        [Fact]
        public void Commonwealth002()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var BritainWealth = new Money(Pound, 356000000000m);

            var EnglandWealth = BritainWealth * 45m / 100m;
            var ScotlandWealth = BritainWealth * 36m / 100m;

            Assert.Equal(160200000000m, EnglandWealth.Amount);
            Assert.Equal(128160000000m, ScotlandWealth.Amount);
        }

        [Fact]
        public void CompareDifferentCurrency001()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Wallet1 = new Money(Franc, 12.6m);
            var Wallet2 = new Money(Krone, 12.6m);

            _ = Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1.CompareTo(Wallet2));
        }

        [Fact]
        public void CompareDifferentCurrency002()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Wallet1 = new Money(Franc, 12.6m);
            var Wallet2 = new Money(Krone, 12.6m);

            _ = Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1.Equals(Wallet2));
        }

        [Fact]
        public void CompareDifferentCurrency003()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Wallet1 = new Money(Franc, 12.6m);
            var Wallet2 = new Money(Krone, 12.6m);

            _ = Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 > Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency004()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Wallet1 = new Money(Franc, 12.6m);
            var Wallet2 = new Money(Krone, 12.6m);

            _ = Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 < Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency005()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Wallet1 = new Money(Franc, 12.6m);
            var Wallet2 = new Money(Krone, 12.6m);

            _ = Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 >= Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency006()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Wallet1 = new Money(Franc, 12.6m);
            var Wallet2 = new Money(Krone, 12.6m);

            _ = Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 <= Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency007()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Wallet1 = new Money(Franc, 12.6m);
            var Wallet2 = new Money(Krone, 12.6m);

            _ = Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 == Wallet2);
        }

        [Fact]
        public void CompareDifferentCurrency008()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Wallet1 = new Money(Franc, 12.6m);
            var Wallet2 = new Money(Krone, 12.6m);

            _ = Assert.Throws<InvalidOperationException>(
                () =>
                    Wallet1 != Wallet2);
        }

        [Fact]
        public void CreateMoney001()
        {
            var UnitedStates = new RegionInfo("US");
            var USDollar = new CurrencyInfo(UnitedStates);

            var payment = new Money(USDollar);

            Assert.Equal(decimal.Zero, payment.Amount);
            Assert.Equal(USDollar, payment.Currency);
            Assert.Equal("USD", payment.Currency.ISOCurrencySymbol);
        }

        [Fact]
        public void CreateMoney002()
        {
            var Armenia = new RegionInfo("AM");
            var ArmenianDram = new CurrencyInfo(Armenia);

            var payment = new Money(ArmenianDram, 1000m);

            Assert.Equal(1000m, payment.Amount);
            Assert.Equal(ArmenianDram, payment.Currency);
            Assert.Equal("AMD", payment.Currency.ISOCurrencySymbol);
        }

        [Fact]
        public void Equality001()
        {
            var UnitedStates1 = new RegionInfo("en-US");
            var UnitedStates2 = new RegionInfo("es-US");

            var Dollar1 = new CurrencyInfo(UnitedStates1);
            var Dollar2 = new CurrencyInfo(UnitedStates2);

            var Money1 = new Money(Dollar1, 125.6m);
            var Money2 = new Money(Dollar2, 125.6m);

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
            var UnitedStates = new RegionInfo("en-US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var M = new Money(Dollar, 145.6m);

            Assert.True(M.Equals(M));
        }

        [Fact]
        public void Equals002()
        {
            var UnitedStates = new RegionInfo("en-US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var M = new Money(Dollar, 145.6m);

            Assert.False(M.Equals(null));
        }

        [Fact]
        public void Equals003()
        {
            var UnitedStates = new RegionInfo("en-US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var M1 = new Money(Dollar, 145.6m);
            object M2 = new Money(Dollar, 145.6m);

            Assert.True(M1.Equals(M2));
        }

        [Fact]
        public void GreaterThan001()
        {
            var UnitedStates = new RegionInfo("US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var Hundred = new Money(Dollar, 100m);
            var Thousand = new Money(Dollar, 1000m);

            Assert.True(Thousand > Hundred);
            Assert.False(Thousand < Hundred);
        }

        [Fact]
        public void GreaterThanOrEqual001()
        {
            var UnitedStates = new RegionInfo("US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var M1 = new Money(Dollar, 100m);
            var M2 = new Money(Dollar, 1000m);

            Assert.False(M1 >= M2);
            Assert.True(M2 >= M1);
            Assert.False(M1.CompareTo(M2) >= 0);
            Assert.True(M2.CompareTo(M1) >= 0);
        }

        [Fact]
        public void GreaterThanOrEqual002()
        {
            var UnitedStates = new RegionInfo("US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var M1 = new Money(Dollar, 100m);
            var M2 = new Money(Dollar, 100m);

            Assert.True(M1 >= M2);
            Assert.True(M2 >= M1);
            Assert.True(M1.CompareTo(M2) >= 0);
            Assert.True(M2.CompareTo(M1) >= 0);
        }

        [Fact]
        public void Inequality001()
        {
            var UnitedStates = new RegionInfo("US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var Hundred = new Money(Dollar, 100m);
            var Thousand = new Money(Dollar, 1000m);

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
            var UnitedStates = new RegionInfo("US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var Hundred = new Money(Dollar, 100m);
            var Thousand = new Money(Dollar, 1000m);

            Assert.True(Hundred < Thousand);
            Assert.False(Hundred > Thousand);
            Assert.True(Hundred.CompareTo(Thousand) < 0);
            Assert.True(Thousand.CompareTo(Hundred) > 0);
        }

        [Fact]
        public void LessThanOrEqual001()
        {
            var UnitedStates = new RegionInfo("US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var Hundred = new Money(Dollar, 100m);
            var Thousand = new Money(Dollar, 1000m);

            Assert.True(Hundred <= Thousand);
            Assert.False(Thousand <= Hundred);
            Assert.True(Hundred.CompareTo(Thousand) <= 0);
        }

        [Fact]
        public void LessThanOrEqual002()
        {
            var UnitedStates = new RegionInfo("US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var M1 = new Money(Dollar, 100m);
            var M2 = new Money(Dollar, 100m);

            Assert.True(M1 <= M2);
            Assert.True(M2 <= M1);
            Assert.True(M1.CompareTo(M2) <= 0);
            Assert.True(M2.CompareTo(M2) <= 0);
        }

        [Fact]
        public void LessThanOrEqual003()
        {
            var UnitedStates = new RegionInfo("US");
            var Dollar = new CurrencyInfo(UnitedStates);

            var M1 = new Money(Dollar, 100m);
            var M2 = new Money(Dollar, 1000m);

            Assert.True(M1 <= M2);
            Assert.False(M2 <= M1);
            Assert.True(M1.CompareTo(M2) <= 0);
            Assert.False(M2.CompareTo(M1) <= 0);
        }

        [Fact]
        public void OperatorDivision001()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 1520m);

            sbyte part = 4;

            var Fund = Budget / part;

            Assert.Equal(380m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision002()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 650m);

            byte part = 5;

            var Fund = Budget / part;

            Assert.Equal(130m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision003()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 420m);

            short part = 6;

            var Fund = Budget / part;

            Assert.Equal(70m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision004()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 712m);

            ushort part = 10;

            var Fund = Budget / part;

            Assert.Equal(71.2m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision005()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 1260m);

            var part = 42;

            var Fund = Budget / part;

            Assert.Equal(30.0m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision006()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 252m);

            uint part = 6;

            var Fund = Budget / part;

            Assert.Equal(42m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision007()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 48m);

            long part = 12;

            var Fund = Budget / part;

            Assert.Equal(4m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision008()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 43.2m);

            ulong part = 36;

            var Fund = Budget / part;

            Assert.Equal(1.2m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision009()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 176m);

            var part = 88m;

            var Fund = Budget / part;

            Assert.Equal(2m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision010()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 882m);

            var part = 42m;

            var Fund = Budget / part;

            Assert.Equal(21m, Fund.Amount);
        }

        [Fact]
        public void OperatorDivision011()
        {
            var Switzerland = new RegionInfo("CH");
            var Franc = new CurrencyInfo(Switzerland);

            var Budget = new Money(Franc, 744m);

            var part = 62m;

            var Fund = Budget / part;

            Assert.Equal(12m, Fund.Amount);
        }

        [Fact]
        public void OperatorModulus001()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 30m);

            sbyte whole = 12;

            var Truncated = Budget % whole;

            Assert.Equal(6m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus002()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 62m);

            byte whole = 10;

            var Truncated = Budget % whole;

            Assert.Equal(2m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus003()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 62m);

            short whole = 9;

            var Truncated = Budget % whole;

            Assert.Equal(8m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus004()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 135m);

            ushort whole = 25;

            var Truncated = Budget % whole;

            Assert.Equal(10m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus005()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 98m);

            var whole = 264;

            var Truncated = Budget % whole;

            Assert.Equal(98m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus006()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 42.2m);

            uint whole = 26;

            var Truncated = Budget % whole;

            Assert.Equal(16.2m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus007()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 6568.8m);

            long whole = 464;

            var Truncated = Budget % whole;

            Assert.Equal(72.8m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus008()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 89.9m);

            ulong whole = 64;

            var Truncated = Budget % whole;

            Assert.Equal(25.9m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus009()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 2500m);

            var whole = 64.7m;

            var Truncated = Budget % whole;

            Assert.Equal(41.4m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorModulus010()
        {
            var Denmark = new RegionInfo("DK");
            var Krone = new CurrencyInfo(Denmark);

            var Budget = new Money(Krone, 4526m);

            var whole = 24.4m;

            var Truncated = Budget % whole;

            Assert.Equal(12m, Truncated.Amount);
            Assert.True(Truncated.Amount < whole);
        }

        [Fact]
        public void OperatorMultiply001()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 12.5m);

            sbyte m = 23;

            var Wallet = Payment * m;

            Assert.Equal(287.5m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply002()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 22.5m);

            byte m = 24;

            var Wallet = Payment * m;

            Assert.Equal(540m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply003()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 26m);

            short m = 240;

            var Wallet = Payment * m;

            Assert.Equal(6240m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply004()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 250m);

            ushort m = 667;

            var Wallet = Payment * m;

            Assert.Equal(166750m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply005()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 24.4m);

            uint m = 598;

            var Wallet = Payment * m;

            Assert.Equal(14591.2m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply006()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 4.44m);

            long m = 662;

            var Wallet = Payment * m;

            Assert.Equal(2939.28m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply007()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 88.2m);

            ulong m = 42;

            var Wallet = Payment * m;

            Assert.Equal(3704.4m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply008()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 242m);

            var m = 6.2m;

            var Wallet = Payment * m;

            Assert.Equal(1500.4m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply009()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 56m);

            var m = 8.4m;

            var Wallet = Payment * m;

            Assert.Equal(470.4m, Wallet.Amount);
        }

        [Fact]
        public void OperatorMultiply010()
        {
            var Britain = new RegionInfo("GB");
            var Pound = new CurrencyInfo(Britain);

            var Payment = new Money(Pound, 69m);

            var m = 1.44m;

            var Wallet = Payment * m;

            Assert.Equal(99.36m, Wallet.Amount);
        }

        [Theory]
        [InlineData("US", "16.6", "en-US", "$16.60")]
        [InlineData("US", "24.4", "en-GB", "$24.40")]
        [InlineData("US", "62.68", "de-DE", "62,68 $")]
        [InlineData("US", "12.36", "ga-IE", "$12.36")]
        [InlineData("US", "5.6", "nl-NL", "$ 5,60")]
        [InlineData("US", "36.6", "hy-AM", "36,60 $")]
        [InlineData("US", "36.6", "uk-UA", "36,60 $")]
        [InlineData("DE", "58.7", "en-US", "€58.70")]
        [InlineData("DE", "12.4", "en-GB", "€12.40")]
        [InlineData("DE", "14.24", "ga-IE", "€14.24")]
        [InlineData("DE", "16.6", "nl-NL", "€ 16,60")]
        [InlineData("DE", "14.5", "hy-AM", "14,50 €")]
        [InlineData("DE", "14.5", "uk-UA", "14,50 €")]
        [InlineData("DE", "16.5", "de-DE", "16,50 €")]
        [InlineData("AM", "2500.24", "de-DE", "2.500,24 ֏")]
        public void ToStringInDifferentCultures(string countryCode, string amount, string culture, string expected)
        {
            var stringValue = string.Empty;

            var thread = new Thread(() =>
            {
                var country = new RegionInfo(countryCode);
                var currency = new CurrencyInfo(country);

                var price = new Money(currency, decimal.Parse(amount, CultureInfo.InvariantCulture));

                var ci = new CultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = ci;
                stringValue = price.ToString();
            });

            thread.Start();
            thread.Join();

            Assert.Equal(expected, stringValue);
        }

        [Theory]
        [InlineData("GB", "56.42", "en-US", "S", "£56.42")]
        [InlineData("GB", "14.6", "en-GB", "S2", "£14.60")]
        [InlineData("GB", "25.96", "de-DE", "S5", "25,96000 £")]
        [InlineData("GB", "12.21", "ga-IE", "S1", "£12.2")]
        [InlineData("GB", "16.36", "nl-NL", "S1", "£ 16,4")]
        [InlineData("GB", "66.326", "hy-AM", "S1", "66,3 £")]
        [InlineData("GB", "162.2", "el-GR", "", "162,20 £")]
        [InlineData("GB", "142.26", "it-IT", "", "142,26 £")]
        [InlineData("GB", "66.32", "sv-SE", null, "66,32 £")]
        [InlineData("AM", "2500", "en-US", "I", "AMD2,500.00")]
        [InlineData("AM", "2500.2", "en-GB", "I3", "AMD2,500.200")]
        public void ToStringWithFormat(string countryCode, string amount, string culture, string format, string expected)
        {
            var country = new RegionInfo(countryCode);
            var currency = new CurrencyInfo(country);

            var price = new Money(currency, decimal.Parse(amount, CultureInfo.InvariantCulture));

            var ci = new CultureInfo(culture);

            Assert.Equal(expected, price.ToString(format, ci));
        }

        [Fact]
        public void ToString022()
        {
            var UnitedKingdom = new RegionInfo("GB");
            var Pound = new CurrencyInfo(UnitedKingdom);

            var Price = new Money(Pound, 66.32m);

            var CI = new CultureInfo("pl-PL");

            _ = Assert.Throws<FormatException>(
                   () =>
                        Price.ToString("K", CI));
        }

        [Fact]
        public void UnaryNegation001()
        {
            var USA = new RegionInfo("US");
            var Dollar = new CurrencyInfo(USA);

            var M1 = new Money(Dollar, 235.6m);
            var M2 = -M1;

            Assert.Equal(M1.Currency, M2.Currency);
            Assert.Equal(-235.6m, M2.Amount);
        }

        [Fact]
        public void UnaryPlus001()
        {
            var USA = new RegionInfo("US");
            var Dollar = new CurrencyInfo(USA);

            var M1 = new Money(Dollar, 235.6m);
            var M2 = +M1;

            Assert.Equal(M1.Currency, M2.Currency);
            Assert.Equal(235.6m, M2.Amount);
        }
    }
}
