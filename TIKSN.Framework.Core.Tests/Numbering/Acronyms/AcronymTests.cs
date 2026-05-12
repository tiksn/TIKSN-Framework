using System.Globalization;
using Shouldly;
using TIKSN.Numbering.Acronyms;
using Xunit;

namespace TIKSN.Tests.Numbering.Acronyms;

public class AcronymTests
{
    [Theory]
    [InlineData(null, false, null)]
    [InlineData(null, true, null)]
    [InlineData("", false, null)]
    [InlineData("", true, null)]
    [InlineData("a", false, null)]
    [InlineData("a", true, null)]
    [InlineData("A", false, null)]
    [InlineData("A", true, null)]
    [InlineData("123", false, null)]
    [InlineData("123", true, null)]
    [InlineData("ԱԲԳ", false, "ԱԲԳ")]
    [InlineData("ԱԲԳ", true, null)]
    [InlineData("B21", false, null)]
    [InlineData("B21", true, null)]
    [InlineData("BBC", false, "BBC")]
    [InlineData("BBC", true, "BBC")]
    [InlineData("CNN", false, "CNN")]
    [InlineData("CNN", true, "CNN")]
    [InlineData("cbc", false, "CBC")]
    [InlineData("cbc", true, "CBC")]
    [InlineData("TLDR", false, null)]
    [InlineData("TLDR", true, null)]
    public void GivenTLA_WhenParsed_ThenValueShouldBe(string input, bool asciiOnly, string expectedValue)
    {
        // Arrange

        // Act
        var actual = TLA.Parse(input, asciiOnly, CultureInfo.InvariantCulture);
        var actualValue = actual.Map(x => x.ToString()).MatchUnsafe(x => x, () => null);

        // Assert
        actualValue.ShouldBe(expectedValue);
    }

    [Theory]
    [InlineData("A", null)]
    [InlineData("AB", "AB")]
    [InlineData("ABC", "ABC")]
    [InlineData("ABCD", "ABCD")]
    [InlineData("ABCDE", null)]
    public void GivenVariableLengthAcronym_WhenParsed_ThenValueShouldRespectLengthRange(
        string input,
        string expectedValue)
    {
        // Arrange

        // Act
        var actual = VariableLengthAcronym.Parse(input, asciiOnly: true, CultureInfo.InvariantCulture);
        var actualValue = actual.Map(x => x.ToString()).MatchUnsafe(x => x, () => null);

        // Assert
        actualValue.ShouldBe(expectedValue);
    }

    private sealed class VariableLengthAcronym : Acronym<VariableLengthAcronym>, IAcronymLength
    {
#pragma warning disable IDE0051 // Remove unused private members

        private VariableLengthAcronym(string value) : base(value)
#pragma warning restore IDE0051 // Remove unused private members
        {
        }

        public static int MaximumLetterCount => 4;

        public static int MinimumLetterCount => 2;
    }
}
