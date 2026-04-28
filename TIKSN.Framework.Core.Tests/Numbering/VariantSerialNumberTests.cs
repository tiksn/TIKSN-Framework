using System;
using System.Globalization;
using Shouldly;
using TIKSN.Numbering;
using TIKSN.Numbering.Acronyms;
using Xunit;

namespace TIKSN.Tests.Numbering;

public class VariantSerialNumberTests
{
    [Theory]
    [InlineData("ABC-123DEF", "ABC-123DEF")]
    [InlineData("ABC123DEF", "ABC-123DEF")]
    [InlineData("ABC-123-DEF", "ABC-123DEF")]
    [InlineData("abc-123def", "ABC-123DEF")]
    public void GivenSerialNumber_WhenParsed_ThenValueShouldBe(string input, string expectedValue)
    {
        // Arrange

        // Act
        var actual = VariantSerialNumber<TLA, ushort, TLA>.Parse(input, asciiOnly: true, CultureInfo.InvariantCulture);
        var actualValue = actual.Map(x => x.ToString()).MatchUnsafe(x => x, () => null);

        // Assert
        actualValue.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData("ABC-123DEF", "ABC-122DEF", "ABC-124DEF")]
    public void GivenSerialNumber_WhenNextAndPreviousRequested_ThenValueShouldBe(
        string serialNumber, string previous, string next)
    {
        // Arrange
        var variantSerialNumber = VariantSerialNumber<TLA, ushort, TLA>
            .Parse(serialNumber, asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => throw new InvalidOperationException());

        // Act
        var previousSerialNumber = variantSerialNumber.GetPrevious();
        var previousSerialNumberValue = previousSerialNumber.MatchUnsafe(s => s.ToString(), () => null);
        var nextSerialNumber = variantSerialNumber.GetNext();
        var nextSerialNumberValue = nextSerialNumber.MatchUnsafe(s => s.ToString(), () => null);

        // Assert
        previousSerialNumberValue.ShouldBe(previous);
        nextSerialNumberValue.ShouldBe(next);
    }

    [Theory]
    [InlineData(null, "ABC-123DEF")]
    [InlineData("", "ABC-123DEF")]
    [InlineData("G", "ABC-123DEF")]
    [InlineData("N", "ABC123DEF")]
    public void GivenSerialNumber_WhenFormatted_ThenValueShouldBe(string? format, string expected)
    {
        // Arrange
        var variantSerialNumber = VariantSerialNumber<TLA, ushort, TLA>
            .Parse("ABC-123DEF", asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => throw new InvalidOperationException());

        // Act
        var actual = variantSerialNumber.ToString(format, CultureInfo.InvariantCulture);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void GivenSerialNumber_WhenEqualityChecked_ThenShouldBeCorrect()
    {
        // Arrange
        var sn1 = VariantSerialNumber<TLA, ushort, TLA>
            .Parse("ABC-123DEF", asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => throw new InvalidOperationException());
        var sn2 = VariantSerialNumber<TLA, ushort, TLA>
            .Parse("ABC-123DEF", asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => throw new InvalidOperationException());
        var sn3 = VariantSerialNumber<TLA, ushort, TLA>
            .Parse("ABC-124DEF", asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => throw new InvalidOperationException());
        var sn4 = VariantSerialNumber<TLA, ushort, TLA>
            .Parse("ABD-123DEF", asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => throw new InvalidOperationException());
        var sn5 = VariantSerialNumber<TLA, ushort, TLA>
            .Parse("ABC-123DEG", asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => throw new InvalidOperationException());

        // Assert
        sn1.ShouldBe(sn2);
        sn1.GetHashCode().ShouldBe(sn2.GetHashCode());
        sn1.ShouldNotBe(sn3);
        sn1.ShouldNotBe(sn4);
        sn1.ShouldNotBe(sn5);
        (sn1 == sn2).ShouldBeTrue();
        (sn1 != sn3).ShouldBeTrue();
    }
}
