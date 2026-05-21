using System;
using System.Collections.Generic;
using System.Globalization;
using Shouldly;
using TIKSN.Numbering;
using Xunit;

namespace TIKSN.Tests.Numbering;

public class OneANotationTests
{
    public static IEnumerable<TheoryDataRow<string, int, string>> GetParseTestData()
    {
        yield return new TheoryDataRow<string, int, string>("1A", p2: 1, "A");
        yield return new TheoryDataRow<string, int, string>("1a", p2: 1, "A");
        yield return new TheoryDataRow<string, int, string>("1-A", p2: 1, "A");
        yield return new TheoryDataRow<string, int, string>("1-a", p2: 1, "A");
        yield return new TheoryDataRow<string, int, string>("2B", p2: 2, "B");
        yield return new TheoryDataRow<string, int, string>("10C", p2: 10, "C");
        yield return new TheoryDataRow<string, int, string>("10AA", p2: 10, "AA");
        yield return new TheoryDataRow<string, int, string>("65535ZZ", p2: 65535, "ZZ");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A1")]
    [InlineData("A-1")]
    [InlineData("1")]
    [InlineData("A")]
    [InlineData("0A")]
    [InlineData("0-A")]
    [InlineData("65536A")]
    public void GivenInvalidString_WhenParse_ThenResultShouldBeNone(string input)
    {
        var result = OneANotation<ushort>.Parse(input, asciiOnly: false, CultureInfo.InvariantCulture);

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void GivenOneANotation_WhenConstructedWithNullSerial_ThenThrowsArgumentNullException() =>
        Should.Throw<ArgumentNullException>(() => new OneANotation<ushort>(number: 1, serial: null));

    [Fact]
    public void GivenOneANotation_WhenConstructedWithValidParameters_ThenPropertiesSet()
    {
        var serial = new BB26(1);
        var number = ushort.MaxValue;

        var oneANotation = new OneANotation<ushort>(number, serial);

        oneANotation.Serial.ShouldBe(serial);
        oneANotation.Number.ShouldBe(number);
    }

    [Fact]
    public void GivenOneANotation_WhenConstructedWithZeroNumber_ThenThrowsArgumentException() =>
        Should.Throw<ArgumentException>(() => new OneANotation<ushort>(number: default, new BB26(1)));

    [Fact]
    public void GivenOneANotation_WhenGetHashCode_ThenConsistent()
    {
        var oneA = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());
        var anotherOneA = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        oneA.GetHashCode().ShouldBe(anotherOneA.GetHashCode());
    }

    [Theory]
    [InlineData("1A", "2A", null)]
    [InlineData("65535A", null, "65534A")]
    [InlineData("1Z", "2Z", null)]
    [InlineData("65535Z", null, "65534Z")]
    [InlineData("1AA", "2AA", null)]
    [InlineData("65535AA", null, "65534AA")]
    public void GivenOneANotation_WhenGetNextAndPrevious_ThenResultShouldBe(
        string input, string? expectedNext, string? expectedPrevious)
    {
        var oneANotation = OneANotation<ushort>.Parse(input, asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var next = oneANotation.GetNext();
        var previous = oneANotation.GetPrevious();

        var nextValue = next.Map(x => x.ToString()).MatchUnsafe(x => x, () => null);
        var previousValue = previous.Map(x => x.ToString()).MatchUnsafe(x => x, () => null);

        nextValue.ShouldBe(expectedNext);
        previousValue.ShouldBe(expectedPrevious);
    }

    [Fact]
    public void GivenOneANotation_WhenNullEquals_ThenFalse()
    {
        var oneA = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        oneA.Equals(null).ShouldBeFalse();
    }

    [Theory]
    [InlineData(null, "1A")]
    [InlineData("", "1A")]
    [InlineData("G", "1A")]
    [InlineData("N", "1A")]
    [InlineData("H", "1-A")]
    public void GivenOneANotation_WhenToStringWithFormat_ThenResultShouldBe(string? format, string expected)
    {
        var oneANotation = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var result = oneANotation.ToString(format, CultureInfo.InvariantCulture);

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("1A", "1A")]
    [InlineData("2B", "2B")]
    [InlineData("10AA", "10AA")]
    [InlineData("65535ZZ", "65535ZZ")]
    public void GivenOneANotation_WhenToString_ThenResultShouldBe(string expected, string input)
    {
        var oneANotation = OneANotation<ushort>.Parse(input, asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var result = oneANotation.ToString();

        result.ShouldBe(expected);
    }

    [Fact]
    public void GivenOneANotation_WhenTryFormatWithHyphenFormat_ThenSuccess()
    {
        var oneANotation = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var buffer = new char[10];
        var success = oneANotation.TryFormat(buffer, out var charsWritten, "H", CultureInfo.InvariantCulture);

        success.ShouldBeTrue();
        charsWritten.ShouldBe(3);
        new string(buffer, startIndex: 0, charsWritten).ShouldBe("1-A");
    }

    [Fact]
    public void GivenOneANotation_WhenTryFormatWithInsufficientBuffer_ThenFailure()
    {
        var oneANotation = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var buffer = new char[1];
        var success = oneANotation.TryFormat(buffer, out var charsWritten, format: null, CultureInfo.InvariantCulture);

        success.ShouldBeFalse();
        charsWritten.ShouldBe(1);
    }

    [Fact]
    public void GivenOneANotation_WhenTryFormatWithSufficientBuffer_ThenSuccess()
    {
        var oneANotation = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        var buffer = new char[10];
        var success = oneANotation.TryFormat(buffer, out var charsWritten, format: null, CultureInfo.InvariantCulture);

        success.ShouldBeTrue();
        charsWritten.ShouldBe(2);
        new string(buffer, startIndex: 0, charsWritten).ShouldBe("1A");
    }

    [Theory]
    [InlineData("1A", true)]
    [InlineData("1-A", true)]
    [InlineData("2B", true)]
    [InlineData("65535ZZ", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("A", false)]
    [InlineData("0A", false)]
    public void GivenString_WhenTryParse_ThenResultShouldBe(string input, bool expectedSuccess)
    {
        var success = OneANotation<ushort>.TryParse(input, CultureInfo.InvariantCulture, out var result);

        success.ShouldBe(expectedSuccess);
        if (expectedSuccess)
        {
            _ = result.ShouldNotBeNull();
        }
        else
        {
            result.ShouldBeNull();
        }
    }

    [Fact]
    public void GivenTwoDifferentOneANotations_WhenEquals_ThenFalse()
    {
        var oneA = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());
        var twoA = OneANotation<ushort>.Parse("2A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        oneA.Equals(twoA).ShouldBeFalse();
        (oneA == twoA).ShouldBeFalse();
        (oneA != twoA).ShouldBeTrue();
    }

    [Fact]
    public void GivenTwoEqualOneANotations_WhenEquals_ThenTrue()
    {
        var oneA = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());
        var anotherOneA = OneANotation<ushort>.Parse("1A", asciiOnly: false, CultureInfo.InvariantCulture)
            .Match(x => x, () => throw new InvalidOperationException());

        oneA.Equals(anotherOneA).ShouldBeTrue();
        (oneA == anotherOneA).ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(GetParseTestData))]
    public void GivenValidString_WhenParse_ThenResultShouldBe(string input, int expectedNumber, string expectedSerial)
    {
        var result = OneANotation<ushort>.Parse(input, asciiOnly: false, CultureInfo.InvariantCulture);

        result.IsSome.ShouldBeTrue();
        var oneANotation = result.Match(x => x, () => throw new InvalidOperationException());
        oneANotation.Number.ShouldBe((ushort)expectedNumber);
        oneANotation.Serial.ToString().ShouldBe(expectedSerial);
    }
}
