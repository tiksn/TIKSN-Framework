using System;
using System.Globalization;
using System.Threading;
using Shouldly;
using TIKSN.Finance;
using Xunit;

namespace TIKSN.Tests.Finance;

public class MoneyTests
{
    [Fact]
    public void ATMWithdraw001()
    {
        var armenia = new RegionInfo("hy-AM");
        var armenianDram = new CurrencyInfo(armenia);

        var account = new Money(armenianDram, 150600);
        var requested = new Money(armenianDram, 5600);

        var withdrawal = requested - (requested % 1000);
        account -= withdrawal;

        account.Amount.ShouldBe(145600m);
    }

    [Fact]
    public void ATMWithdraw002()
    {
        var spain = new RegionInfo("ES");
        var euro = new CurrencyInfo(spain);

        var account = new Money(euro, 2520.2m);
        var requested = new Money(euro, 6.8m);

        var withdrawal = requested - (requested % 0.5m);
        account -= withdrawal;

        account.Amount.ShouldBe(2513.7m);
    }

    [Fact]
    public void BuyAndSell001()
    {
        var italy = new RegionInfo("IT");
        var euro = new CurrencyInfo(italy);

        var buyerWallet = new Money(euro, 650m);
        var sellerWallet = new Money(euro, 1256.7m);
        var payment = new Money(euro, 1.50m);

        buyerWallet -= payment;
        sellerWallet += payment;

        buyerWallet.Amount.ShouldBe(648.5m);
        sellerWallet.Amount.ShouldBe(1258.2m);
    }

    [Fact]
    public void BuyAndSell002()
    {
        var armenia = new RegionInfo("hy-AM");
        var armenianDram = new CurrencyInfo(armenia);

        var buyerWallet = new Money(armenianDram, 5000m);

        var breadPrice = new Money(armenianDram, 250m);
        var breadQuantity = 4m;

        var milkPrice = new Money(armenianDram, 350m);
        var milkBottleVolume = 1.5m;
        var milkQuantity = 2;

        var butterPrice = new Money(armenianDram, 2500m);
        var butterPart = 2.5m;

        buyerWallet -= (breadPrice * breadQuantity) + (milkPrice * milkBottleVolume * milkQuantity) + (butterPrice / butterPart);

        buyerWallet.Amount.ShouldBe(1950m);
    }

    [Fact]
    public void Commonwealth001()
    {
        var russia = new RegionInfo("RU");
        var russianRuble = new CurrencyInfo(russia);

        var ivan = new Money(russianRuble, 1250m);
        var olga = new Money(russianRuble, 2360m);

        var commonwealth = (ivan + olga) / 2;

        commonwealth.Amount.ShouldBe(1805m);
    }

    [Fact]
    public void Commonwealth002()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var britainWealth = new Money(pound, 356000000000m);

        var englandWealth = britainWealth * 45m / 100m;
        var scotlandWealth = britainWealth * 36m / 100m;

        englandWealth.Amount.ShouldBe(160200000000m);
        scotlandWealth.Amount.ShouldBe(128160000000m);
    }

    [Fact]
    public void CompareDifferentCurrency001()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var wallet1 = new Money(franc, 12.6m);
        var wallet2 = new Money(krone, 12.6m);

        _ = new Func<object>(() =>
                wallet1.CompareTo(wallet2)).ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void CompareDifferentCurrency002()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var wallet1 = new Money(franc, 12.6m);
        var wallet2 = new Money(krone, 12.6m);

        _ = new Func<object>(() =>
                wallet1.Equals(wallet2)).ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void CompareDifferentCurrency003()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var wallet1 = new Money(franc, 12.6m);
        var wallet2 = new Money(krone, 12.6m);

        _ = new Func<object>(() =>
                wallet1 > wallet2).ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void CompareDifferentCurrency004()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var wallet1 = new Money(franc, 12.6m);
        var wallet2 = new Money(krone, 12.6m);

        _ = new Func<object>(() =>
                wallet1 < wallet2).ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void CompareDifferentCurrency005()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var wallet1 = new Money(franc, 12.6m);
        var wallet2 = new Money(krone, 12.6m);

        _ = new Func<object>(() =>
                wallet1 >= wallet2).ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void CompareDifferentCurrency006()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var wallet1 = new Money(franc, 12.6m);
        var wallet2 = new Money(krone, 12.6m);

        _ = new Func<object>(() =>
                wallet1 <= wallet2).ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void CompareDifferentCurrency007()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var wallet1 = new Money(franc, 12.6m);
        var wallet2 = new Money(krone, 12.6m);

        _ = new Func<object>(() =>
                wallet1 == wallet2).ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void CompareDifferentCurrency008()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var wallet1 = new Money(franc, 12.6m);
        var wallet2 = new Money(krone, 12.6m);

        _ = new Func<object>(() =>
                wallet1 != wallet2).ShouldThrow<InvalidOperationException>();
    }

    [Fact]
    public void CreateMoney001()
    {
        var unitedStates = new RegionInfo("US");
        var usdollar = new CurrencyInfo(unitedStates);

        var payment = new Money(usdollar);

        payment.Amount.ShouldBe(decimal.Zero);
        payment.Currency.ShouldBe(usdollar);
        payment.Currency.ISOCurrencySymbol.ShouldBe("USD");
    }

    [Fact]
    public void CreateMoney002()
    {
        var armenia = new RegionInfo("AM");
        var armenianDram = new CurrencyInfo(armenia);

        var payment = new Money(armenianDram, 1000m);

        payment.Amount.ShouldBe(1000m);
        payment.Currency.ShouldBe(armenianDram);
        payment.Currency.ISOCurrencySymbol.ShouldBe("AMD");
    }

    [Fact]
    public void Equality001()
    {
        var unitedStates1 = new RegionInfo("en-US");
        var unitedStates2 = new RegionInfo("es-US");

        var dollar1 = new CurrencyInfo(unitedStates1);
        var dollar2 = new CurrencyInfo(unitedStates2);

        var money1 = new Money(dollar1, 125.6m);
        var money2 = new Money(dollar2, 125.6m);

        (money1 == money2).ShouldBeTrue();
        (money1 != money2).ShouldBeFalse();
        money1.Equals(money2).ShouldBeTrue();
        money2.Equals(money1).ShouldBeTrue();
        money1.CompareTo(money2).ShouldBe(0);
        money2.CompareTo(money1).ShouldBe(0);
    }

    [Fact]
    public void Equals001()
    {
        var unitedStates = new RegionInfo("en-US");
        var dollar = new CurrencyInfo(unitedStates);

        var m = new Money(dollar, 145.6m);

        m.Equals(m).ShouldBeTrue();
    }

    [Fact]
    public void Equals002()
    {
        var unitedStates = new RegionInfo("en-US");
        var dollar = new CurrencyInfo(unitedStates);

        var m = new Money(dollar, 145.6m);

        m.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals003()
    {
        var unitedStates = new RegionInfo("en-US");
        var dollar = new CurrencyInfo(unitedStates);

        var m1 = new Money(dollar, 145.6m);
        object m2 = new Money(dollar, 145.6m);

        m1.Equals(m2).ShouldBeTrue();
    }

    [Fact]
    public void GreaterThan001()
    {
        var unitedStates = new RegionInfo("US");
        var dollar = new CurrencyInfo(unitedStates);

        var hundred = new Money(dollar, 100m);
        var thousand = new Money(dollar, 1000m);

        (thousand > hundred).ShouldBeTrue();
        (thousand < hundred).ShouldBeFalse();
    }

    [Fact]
    public void GreaterThanOrEqual001()
    {
        var unitedStates = new RegionInfo("US");
        var dollar = new CurrencyInfo(unitedStates);

        var m1 = new Money(dollar, 100m);
        var m2 = new Money(dollar, 1000m);

        (m1 >= m2).ShouldBeFalse();
        (m2 >= m1).ShouldBeTrue();
        (m1.CompareTo(m2) >= 0).ShouldBeFalse();
        (m2.CompareTo(m1) >= 0).ShouldBeTrue();
    }

    [Fact]
    public void GreaterThanOrEqual002()
    {
        var unitedStates = new RegionInfo("US");
        var dollar = new CurrencyInfo(unitedStates);

        var m1 = new Money(dollar, 100m);
        var m2 = new Money(dollar, 100m);

        (m1 >= m2).ShouldBeTrue();
        (m2 >= m1).ShouldBeTrue();
        (m1.CompareTo(m2) >= 0).ShouldBeTrue();
        (m2.CompareTo(m1) >= 0).ShouldBeTrue();
    }

    [Fact]
    public void Inequality001()
    {
        var unitedStates = new RegionInfo("US");
        var dollar = new CurrencyInfo(unitedStates);

        var hundred = new Money(dollar, 100m);
        var thousand = new Money(dollar, 1000m);

        (hundred == thousand).ShouldBeFalse();
        (hundred != thousand).ShouldBeTrue();
        hundred.Equals(thousand).ShouldBeFalse();
        thousand.Equals(hundred).ShouldBeFalse();
        hundred.CompareTo(thousand).ShouldNotBe(0);
        thousand.CompareTo(hundred).ShouldNotBe(0);
    }

    [Fact]
    public void LessThan001()
    {
        var unitedStates = new RegionInfo("US");
        var dollar = new CurrencyInfo(unitedStates);

        var hundred = new Money(dollar, 100m);
        var thousand = new Money(dollar, 1000m);

        (hundred < thousand).ShouldBeTrue();
        (hundred > thousand).ShouldBeFalse();
        (hundred.CompareTo(thousand) < 0).ShouldBeTrue();
        (thousand.CompareTo(hundred) > 0).ShouldBeTrue();
    }

    [Fact]
    public void LessThanOrEqual001()
    {
        var unitedStates = new RegionInfo("US");
        var dollar = new CurrencyInfo(unitedStates);

        var hundred = new Money(dollar, 100m);
        var thousand = new Money(dollar, 1000m);

        (hundred <= thousand).ShouldBeTrue();
        (thousand <= hundred).ShouldBeFalse();
        (hundred.CompareTo(thousand) <= 0).ShouldBeTrue();
    }

    [Fact]
    public void LessThanOrEqual002()
    {
        var unitedStates = new RegionInfo("US");
        var dollar = new CurrencyInfo(unitedStates);

        var m1 = new Money(dollar, 100m);
        var m2 = new Money(dollar, 100m);

        (m1 <= m2).ShouldBeTrue();
        (m2 <= m1).ShouldBeTrue();
        (m1.CompareTo(m2) <= 0).ShouldBeTrue();
        (m2.CompareTo(m2) <= 0).ShouldBeTrue();
    }

    [Fact]
    public void LessThanOrEqual003()
    {
        var unitedStates = new RegionInfo("US");
        var dollar = new CurrencyInfo(unitedStates);

        var m1 = new Money(dollar, 100m);
        var m2 = new Money(dollar, 1000m);

        (m1 <= m2).ShouldBeTrue();
        (m2 <= m1).ShouldBeFalse();
        (m1.CompareTo(m2) <= 0).ShouldBeTrue();
        (m2.CompareTo(m1) <= 0).ShouldBeFalse();
    }

    [Fact]
    public void OperatorDivision001()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 1520m);

        sbyte part = 4;

        var fund = budget / part;

        fund.Amount.ShouldBe(380m);
    }

    [Fact]
    public void OperatorDivision002()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 650m);

        byte part = 5;

        var fund = budget / part;

        fund.Amount.ShouldBe(130m);
    }

    [Fact]
    public void OperatorDivision003()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 420m);

        short part = 6;

        var fund = budget / part;

        fund.Amount.ShouldBe(70m);
    }

    [Fact]
    public void OperatorDivision004()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 712m);

        ushort part = 10;

        var fund = budget / part;

        fund.Amount.ShouldBe(71.2m);
    }

    [Fact]
    public void OperatorDivision005()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 1260m);

        var part = 42;

        var fund = budget / part;

        fund.Amount.ShouldBe(30.0m);
    }

    [Fact]
    public void OperatorDivision006()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 252m);

        uint part = 6;

        var fund = budget / part;

        fund.Amount.ShouldBe(42m);
    }

    [Fact]
    public void OperatorDivision007()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 48m);

        long part = 12;

        var fund = budget / part;

        fund.Amount.ShouldBe(4m);
    }

    [Fact]
    public void OperatorDivision008()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 43.2m);

        ulong part = 36;

        var fund = budget / part;

        fund.Amount.ShouldBe(1.2m);
    }

    [Fact]
    public void OperatorDivision009()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 176m);

        var part = 88m;

        var fund = budget / part;

        fund.Amount.ShouldBe(2m);
    }

    [Fact]
    public void OperatorDivision010()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 882m);

        var part = 42m;

        var fund = budget / part;

        fund.Amount.ShouldBe(21m);
    }

    [Fact]
    public void OperatorDivision011()
    {
        var switzerland = new RegionInfo("CH");
        var franc = new CurrencyInfo(switzerland);

        var budget = new Money(franc, 744m);

        var part = 62m;

        var fund = budget / part;

        fund.Amount.ShouldBe(12m);
    }

    [Fact]
    public void OperatorModulus001()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 30m);

        sbyte whole = 12;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(6m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus002()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 62m);

        byte whole = 10;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(2m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus003()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 62m);

        short whole = 9;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(8m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus004()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 135m);

        ushort whole = 25;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(10m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus005()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 98m);

        var whole = 264;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(98m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus006()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 42.2m);

        uint whole = 26;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(16.2m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus007()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 6568.8m);

        long whole = 464;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(72.8m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus008()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 89.9m);

        ulong whole = 64;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(25.9m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus009()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 2500m);

        var whole = 64.7m;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(41.4m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorModulus010()
    {
        var denmark = new RegionInfo("DK");
        var krone = new CurrencyInfo(denmark);

        var budget = new Money(krone, 4526m);

        var whole = 24.4m;

        var truncated = budget % whole;

        truncated.Amount.ShouldBe(12m);
        (truncated.Amount < whole).ShouldBeTrue();
    }

    [Fact]
    public void OperatorMultiply001()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 12.5m);

        sbyte m = 23;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(287.5m);
    }

    [Fact]
    public void OperatorMultiply002()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 22.5m);

        byte m = 24;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(540m);
    }

    [Fact]
    public void OperatorMultiply003()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 26m);

        short m = 240;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(6240m);
    }

    [Fact]
    public void OperatorMultiply004()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 250m);

        ushort m = 667;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(166750m);
    }

    [Fact]
    public void OperatorMultiply005()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 24.4m);

        uint m = 598;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(14591.2m);
    }

    [Fact]
    public void OperatorMultiply006()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 4.44m);

        long m = 662;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(2939.28m);
    }

    [Fact]
    public void OperatorMultiply007()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 88.2m);

        ulong m = 42;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(3704.4m);
    }

    [Fact]
    public void OperatorMultiply008()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 242m);

        var m = 6.2m;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(1500.4m);
    }

    [Fact]
    public void OperatorMultiply009()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 56m);

        var m = 8.4m;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(470.4m);
    }

    [Fact]
    public void OperatorMultiply010()
    {
        var britain = new RegionInfo("GB");
        var pound = new CurrencyInfo(britain);

        var payment = new Money(pound, 69m);

        var m = 1.44m;

        var wallet = payment * m;

        wallet.Amount.ShouldBe(99.36m);
    }

    [Fact]
    public void ToString022()
    {
        var unitedKingdom = new RegionInfo("GB");
        var pound = new CurrencyInfo(unitedKingdom);

        var price = new Money(pound, 66.32m);

        _ = new Func<object>(() =>
                    price.ToString("K", new CultureInfo("pl-PL"))).ShouldThrow<FormatException>();
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

            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            stringValue = price.ToString();
        });

        thread.Start();
        thread.Join();

        stringValue.ShouldBe(expected);
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

        price.ToString(format, ci).ShouldBe(expected);
    }

    [Fact]
    public void UnaryNegation001()
    {
        var thetheUSA = new RegionInfo("US");
        var dollar = new CurrencyInfo(thetheUSA);

        var m1 = new Money(dollar, 235.6m);
        var m2 = -m1;

        m2.Currency.ShouldBe(m1.Currency);
        m2.Amount.ShouldBe(-235.6m);
    }

    [Fact]
    public void UnaryPlus001()
    {
        var theUSA = new RegionInfo("US");
        var dollar = new CurrencyInfo(theUSA);

        var m1 = new Money(dollar, 235.6m);
        var m2 = +m1;

        m2.Currency.ShouldBe(m1.Currency);
        m2.Amount.ShouldBe(235.6m);
    }
}
