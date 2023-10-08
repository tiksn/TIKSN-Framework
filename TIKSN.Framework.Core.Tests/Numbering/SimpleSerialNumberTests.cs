using System;
using System.Globalization;
using FluentAssertions;
using TIKSN.Numbering.Acronyms;
using Xunit;

namespace TIKSN.Numbering.Tests;

public class SimpleSerialNumberTests
{
    [Theory]
    [InlineData(null, false, null)]
    [InlineData(null, true, null)]
    [InlineData("", false, null)]
    [InlineData("", true, null)]
    [InlineData("-123", false, null)]
    [InlineData("-123", true, null)]
    [InlineData("a", false, null)]
    [InlineData("a", true, null)]
    [InlineData("a-123", false, null)]
    [InlineData("a-123", true, null)]
    [InlineData("A", false, null)]
    [InlineData("A", true, null)]
    [InlineData("A-123", false, null)]
    [InlineData("A-123", true, null)]
    [InlineData("123", false, null)]
    [InlineData("123", true, null)]
    [InlineData("ԱԲԳ", false, null)]
    [InlineData("ԱԲԳ", true, null)]
    [InlineData("ԱԲԳ-123", false, "ԱԲԳ-123")]
    [InlineData("ԱԲԳ-123", true, null)]
    [InlineData("ԱԲԳ123", false, "ԱԲԳ-123")]
    [InlineData("ԱԲԳ123", true, null)]
    [InlineData("B21", false, null)]
    [InlineData("B21", true, null)]
    [InlineData("B21-123", false, null)]
    [InlineData("B21-123", true, null)]
    [InlineData("BBC", false, null)]
    [InlineData("BBC", true, null)]
    [InlineData("BBC-123", false, "BBC-123")]
    [InlineData("BBC-123", true, "BBC-123")]
    [InlineData("BBC123", false, "BBC-123")]
    [InlineData("BBC123", true, "BBC-123")]
    [InlineData("cbc", false, null)]
    [InlineData("cbc", true, null)]
    [InlineData("cbc-123", false, "CBC-123")]
    [InlineData("cbc-123", true, "CBC-123")]
    [InlineData("cbc123", false, "CBC-123")]
    [InlineData("cbc123", true, "CBC-123")]
    [InlineData("TLDR", false, null)]
    [InlineData("TLDR", true, null)]
    [InlineData("TLDR-123", false, null)]
    [InlineData("TLDR-123", true, null)]
    [InlineData("TLDR123", false, null)]
    [InlineData("TLDR123", true, null)]
    public void GivenSerialNumber_WhenParsed_ThenValueShouldBe(string input, bool asciiOnly, string expectedValue)
    {
        // Arrange

        // Act
        var actual = SimpleSerialNumber<TLA, ushort>.Parse(input, asciiOnly, CultureInfo.InvariantCulture);
        var actualValue = actual.Map(x => x.ToString()).MatchUnsafe(x => x, () => null);

        // Assert
        actualValue.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("ABC-123", "ABC-122", "ABC-124")]
    [InlineData("ABC-0", null, "ABC-1")]
    [InlineData("ABC-65535", "ABC-65534", null)]
    public void GivenSerialNumber_WhenNextAndPreviousRequested_ThenValueShouldBe(
        string serialNumber, string previous, string next)
    {
        // Arrange
        var simpleSerialNumber = SimpleSerialNumber<TLA, ushort>
            .Parse(serialNumber, asciiOnly: true, CultureInfo.InvariantCulture)
            .Match(s => s, () => throw new InvalidOperationException());

        // Act
        var previousSerialNumber = simpleSerialNumber.GetPrevious();
        var previousSerialNumberValue = previousSerialNumber.MatchUnsafe(s => s.ToString(), () => null);
        var nextSerialNumber = simpleSerialNumber.GetNext();
        var nextSerialNumberValue = nextSerialNumber.MatchUnsafe(s => s.ToString(), () => null);

        // Assert
        previousSerialNumberValue.Should().Be(previous);
        nextSerialNumberValue.Should().Be(next);
    }
}
