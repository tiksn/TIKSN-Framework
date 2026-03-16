using System;
using System.Collections.Generic;
using System.Globalization;
using Shouldly;
using TIKSN.Numbering;
using Xunit;

namespace TIKSN.Tests.Numbering;

public class A1NotationTests
{
    public static IEnumerable<TheoryDataRow<string, int, string>> GetParseTestData()
    {
        yield return new TheoryDataRow<string, int, string>("A1", 1, "A");
        yield return new TheoryDataRow<string, int, string>("a1", 1, "A");
        yield return new TheoryDataRow<string, int, string>("A-1", 1, "A");
        yield return new TheoryDataRow<string, int, string>("B2", 2, "B");
        yield return new TheoryDataRow<string, int, string>("b2", 2, "B");
        yield return new TheoryDataRow<string, int, string>("B-2", 2, "B");
        yield return new TheoryDataRow<string, int, string>("C10", 10, "C");
        yield return new TheoryDataRow<string, int, string>("AA10", 10, "AA");
        yield return new TheoryDataRow<string, int, string>("ZZ65535", 65535, "ZZ");
    }

    [Theory]
    [MemberData(nameof(GetParseTestData))]
    public void GivenValidString_WhenParse_ThenResultShouldBe(string input, int expectedNumber, string expectedSerial)
    {
        var result = A1Notation<ushort>.Parse(input, false, CultureInfo.InvariantCulture);

        result.IsSome.ShouldBeTrue();
        var a1Notation = result.Match(x => x, () => throw new InvalidOperationException());
        a1Notation.Number.ShouldBe((ushort)expectedNumber);
        a1Notation.Serial.ToString().ShouldBe(expectedSerial);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("A0")]
    [InlineData("a0")]
    [InlineData("A65536")]
    public void GivenInvalidString_WhenParse_ThenResultShouldBeNone(string input)
    {
        var result = A1Notation<ushort>.Parse(input, false, CultureInfo.InvariantCulture);

        result.IsNone.ShouldBeTrue();
    }

    [Theory]
    [InlineData("A1", true)]
    [InlineData("B2", true)]
    [InlineData("ZZ65535", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("A", false)]
    public void GivenString_WhenTryParse_ThenResultShouldBe(string input, bool expectedSuccess)
    {
        var success = A1Notation<ushort>.TryParse(input, CultureInfo.InvariantCulture, out var result);

        success.ShouldBe(expectedSuccess);
        if (expectedSuccess)
        {
            result.ShouldNotBeNull();
        }
        else
        {
            result.ShouldBeNull();
        }
    }

    [Theory]
    [InlineData("A1", "A1")]
    [InlineData("B2", "B2")]
    [InlineData("AA10", "AA10")]
    [InlineData("ZZ65535", "ZZ65535")]
    public void GivenA1Notation_WhenToString_ThenResultShouldBe(string expected, string input)
    {
        var a1Notation = A1Notation<ushort>.Parse(input, false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var result = a1Notation.ToString();

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("A1", "A1")]
    [InlineData("ZZ65535", "ZZ65535")]
    public void GivenA1Notation_WhenToStringWithFormat_ThenResultShouldBe(string expected, string input)
    {
        var a1Notation = A1Notation<ushort>.Parse(input, false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var result = a1Notation.ToString(null, CultureInfo.InvariantCulture);

        result.ShouldBe(expected);
    }

    [Fact]
    public void GivenA1Notation_WhenTryFormatWithSufficientBuffer_ThenSuccess()
    {
        var a1Notation = A1Notation<ushort>.Parse("A1", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var buffer = new char[10];
        var success = a1Notation.TryFormat(buffer, out var charsWritten, null, CultureInfo.InvariantCulture);

        success.ShouldBeTrue();
        charsWritten.ShouldBe(2);
        new string(buffer, 0, charsWritten).ShouldBe("A1");
    }

    [Fact]
    public void GivenA1Notation_WhenTryFormatWithInsufficientBuffer_ThenFailure()
    {
        var a1Notation = A1Notation<ushort>.Parse("A1", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var buffer = new char[1];
        var success = a1Notation.TryFormat(buffer, out var charsWritten, null, CultureInfo.InvariantCulture);

        success.ShouldBeFalse();
        charsWritten.ShouldBe(1);
    }

    [Theory]
    [InlineData("A1", "A2", null)]
    [InlineData("A65535", null, "A65534")]
    [InlineData("Z1", "Z2", null)]
    [InlineData("Z65535", null, "Z65534")]
    [InlineData("AA1", "AA2", null)]
    [InlineData("AA65535", null, "AA65534")]
    public void GivenA1Notation_WhenGetNextAndPrevious_ThenResultShouldBe(
        string input, string? expectedNext, string? expectedPrevious)
    {
        var a1Notation = A1Notation<ushort>.Parse(input, false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var next = a1Notation.GetNext();
        var previous = a1Notation.GetPrevious();

        var nextValue = next.Map(x => x.ToString()).MatchUnsafe(x => x, () => null);
        var previousValue = previous.Map(x => x.ToString()).MatchUnsafe(x => x, () => null);

        nextValue.ShouldBe(expectedNext);
        previousValue.ShouldBe(expectedPrevious);
    }

    [Fact]
    public void GivenTwoEqualA1Notations_WhenEquals_ThenTrue()
    {
        var a1 = A1Notation<ushort>.Parse("A1", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());
        var a2 = A1Notation<ushort>.Parse("A1", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        a1.Equals(a2).ShouldBeTrue();
        (a1 == a2).ShouldBeTrue();
    }

    [Fact]
    public void GivenTwoDifferentA1Notations_WhenEquals_ThenFalse()
    {
        var a1 = A1Notation<ushort>.Parse("A1", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());
        var a2 = A1Notation<ushort>.Parse("A2", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        a1.Equals(a2).ShouldBeFalse();
        (a1 == a2).ShouldBeFalse();
        (a1 != a2).ShouldBeTrue();
    }

    [Fact]
    public void GivenA1Notation_WhenGetHashCode_ThenConsistent()
    {
        var a1 = A1Notation<ushort>.Parse("A1", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());
        var a2 = A1Notation<ushort>.Parse("A1", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        a1.GetHashCode().ShouldBe(a2.GetHashCode());
    }

    [Fact]
    public void GivenA1Notation_WhenNullEquals_ThenFalse()
    {
        var a1 = A1Notation<ushort>.Parse("A1", false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        a1.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void GivenA1Notation_WhenConstructedWithNullSerial_ThenThrowsArgumentNullException() => Should.Throw<ArgumentNullException>(() => new A1Notation<ushort>(null, 1));

    [Fact]
    public void GivenA1Notation_WhenConstructedWithNullNumber_ThenThrowsArgumentNullException() => Should.Throw<ArgumentException>(() => new A1Notation<ushort>(new BB26(1), default));

    [Fact]
    public void GivenA1Notation_WhenConstructedWithValidParameters_ThenPropertiesSet()
    {
        var serial = new BB26(1);
        var number = ushort.MaxValue;

        var a1Notation = new A1Notation<ushort>(serial, number);

        a1Notation.Serial.ShouldBe(serial);
        a1Notation.Number.ShouldBe(number);
    }
}
