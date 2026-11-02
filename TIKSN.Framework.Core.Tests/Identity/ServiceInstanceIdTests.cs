using System;
using System.Globalization;
using TIKSN.Identity;
using Xunit;

namespace TIKSN.Tests.Identity;

public class ServiceInstanceIdTests
{
    [Fact]
    public void Equals_DifferentType_ShouldReturnFalse()
    {
        // Arrange
        var id1 = ServiceInstanceId.Parse("1234567890", CultureInfo.InvariantCulture); // long
        var id2 = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture); // Guid

        // Act & Assert
        Assert.False(id1.Equals(id2));
    }

    [Fact]
    public void Equals_DifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var id1 = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var id2 = ServiceInstanceId.Parse("456", CultureInfo.InvariantCulture);

        // Act & Assert
        Assert.False(id1.Equals(id2));
        Assert.True(id1 != id2);
    }

    [Fact]
    public void Equals_SameValue_ShouldReturnTrue()
    {
        // Arrange
        var id1 = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var id2 = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);

        // Act & Assert
        Assert.True(id1.Equals(id2));
        Assert.True(id1 == id2);
    }

    [Fact]
    public void Interfaces_ShouldImplementExpectedInterfaces()
    {
        AssertSpanParsable<ServiceInstanceId>();
        AssertSpanFormattable<ServiceInstanceId>();

        static void AssertSpanParsable<T>() where T : ISpanParsable<T>, IParsable<T>
        {
        }

        static void AssertSpanFormattable<T>() where T : ISpanFormattable, IFormattable
        {
        }
    }

    [Fact]
    public void Parse_InvalidFormat_ShouldThrowFormatException()
    {
        // Arrange
        var input = "invalid-id";

        // Act & Assert
        _ = Assert.Throws<FormatException>(() => ServiceInstanceId.Parse(input, CultureInfo.InvariantCulture));
        _ = Assert.Throws<FormatException>(() => ServiceInstanceId.Parse(input.AsSpan(), CultureInfo.InvariantCulture));
#pragma warning disable CA1305 // Specify IFormatProvider
        _ = Assert.Throws<FormatException>(() => ServiceInstanceId.Parse(input));
        _ = Assert.Throws<FormatException>(() => ServiceInstanceId.Parse(input.AsSpan()));
#pragma warning restore CA1305 // Specify IFormatProvider
    }

    [Fact]
    public void Parse_Null_ShouldThrowArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => ServiceInstanceId.Parse(null!, CultureInfo.InvariantCulture));
#pragma warning disable CA1305 // Specify IFormatProvider
        _ = Assert.Throws<ArgumentNullException>(() => ServiceInstanceId.Parse(null!));
#pragma warning restore CA1305 // Specify IFormatProvider
    }

    [Theory]
    [InlineData("123")]
    [InlineData("0")]
    [InlineData("9876543210")]
    [InlineData("01H8XGJWBWBAQ4J1Z5F4ANNJ4A")]
    [InlineData("d8f2f6a2-4b3f-4e6a-8b0c-9a0a0a0a0a0a")]
    public void Parse_ValidInputs_ShouldSucceed(string input)
    {
        var idFromString = ServiceInstanceId.Parse(input, CultureInfo.InvariantCulture);
        var idFromSpan = ServiceInstanceId.Parse(input.AsSpan(), CultureInfo.InvariantCulture);
#pragma warning disable CA1305 // Specify IFormatProvider
        var idDefaultString = ServiceInstanceId.Parse(input);
        var idDefaultSpan = ServiceInstanceId.Parse(input.AsSpan());
#pragma warning restore CA1305 // Specify IFormatProvider

        Assert.Equal(idFromString, idFromSpan);
        Assert.Equal(idFromString, idDefaultString);
        Assert.Equal(idFromString, idDefaultSpan);
    }

    [Fact]
    public void ToString_GuidWithFormats_ShouldFormatCorrectly()
    {
        var guid = Guid.Parse("d8f2f6a2-4b3f-4e6a-8b0c-9a0a0a0a0a0a", CultureInfo.InvariantCulture);
        var id = ServiceInstanceId.Create(guid);

        Assert.Equal(guid.ToString("N", CultureInfo.InvariantCulture), id.ToString("N", CultureInfo.InvariantCulture));
        Assert.Equal(guid.ToString("D", CultureInfo.InvariantCulture), id.ToString("D", CultureInfo.InvariantCulture));
        Assert.Equal(guid.ToString("B", CultureInfo.InvariantCulture), id.ToString("B", CultureInfo.InvariantCulture));
        Assert.Equal(guid.ToString("P", CultureInfo.InvariantCulture), id.ToString("P", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToString_Guid_ShouldReturnOriginalValue()
    {
        // Arrange
        var input = "d8f2f6a2-4b3f-4e6a-8b0c-9a0a0a0a0a0a";
        var id = ServiceInstanceId.Parse(input, CultureInfo.InvariantCulture);

        // Act
        var result = id.ToString();

        // Assert
        Assert.Equal(input, result, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToString_LongWithFormats_ShouldFormatCorrectly()
    {
        var id = ServiceInstanceId.Create(42L);

        Assert.Equal("42", id.ToString("D", CultureInfo.InvariantCulture));
        Assert.Equal("00042", id.ToString("D5", CultureInfo.InvariantCulture));
        Assert.Equal("2A", id.ToString("X", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TryFormat_DestinationTooSmall_ShouldReturnFalse()
    {
        var guid = Guid.Parse("d8f2f6a2-4b3f-4e6a-8b0c-9a0a0a0a0a0a", CultureInfo.InvariantCulture);
        var id = ServiceInstanceId.Create(guid);

        Span<char> destination = stackalloc char[10];
        var success = id.TryFormat(destination, out var charsWritten, "D", CultureInfo.InvariantCulture);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    public void TryFormat_ExactDestinationLength_ShouldSucceed()
    {
        var guid = Guid.Parse("d8f2f6a2-4b3f-4e6a-8b0c-9a0a0a0a0a0a", CultureInfo.InvariantCulture);
        var id = ServiceInstanceId.Create(guid);

        Span<char> destination = stackalloc char[36];
        var success = id.TryFormat(destination, out var charsWritten, "D", CultureInfo.InvariantCulture);

        Assert.True(success);
        Assert.Equal(36, charsWritten);
        Assert.Equal(guid.ToString("D", CultureInfo.InvariantCulture), new string(destination));
    }

    [Fact]
    public void TryFormat_Long_ShouldSucceed()
    {
        var id = ServiceInstanceId.Create(12345L);

        Span<char> destination = stackalloc char[10];
        var success = id.TryFormat(destination, out var charsWritten, "D", CultureInfo.InvariantCulture);

        Assert.True(success);
        Assert.Equal(5, charsWritten);
        Assert.Equal("12345", destination[..charsWritten].ToString());
    }

    [Fact]
    public void TryParse_NullString_ShouldReturnFalse()
    {
        var bclResult = ServiceInstanceId.TryParse(null, CultureInfo.InvariantCulture, out var parsedResult);
        Assert.False(bclResult);
        Assert.Null(parsedResult);
    }

    [Theory]
    [InlineData("123", true)]
    [InlineData("9876543210", true)]
    [InlineData("01H8XGJWBWBAQ4J1Z5F4ANNJ4A", true)]
    [InlineData("d8f2f6a2-4b3f-4e6a-8b0c-9a0a0a0a0a0a", true)]
    [InlineData("0", true)]
    [InlineData("-123", false)]
    [InlineData("abc", false)]
    [InlineData("01H8XGJWBWBAQ4J1Z5F4ANNJ4", false)] // Invalid Ulid (length 25)
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(" 123 ", false)]
    public void TryParse_VariousInputs_ShouldReturnExpectedResult(string input, bool expected)
    {
        // Act - Option TryParse(string)
        var optionResult = ServiceInstanceId.TryParse(input);

        // Act - Option TryParse(ReadOnlySpan<char>)
        var optionSpanResult = ServiceInstanceId.TryParse(input.AsSpan());

        // Act - BCL TryParse(string, provider, out result)
        var bclResult = ServiceInstanceId.TryParse(input, CultureInfo.InvariantCulture, out var parsedResult);

        // Act - BCL TryParse(ReadOnlySpan<char>, provider, out result)
        var bclSpanResult =
            ServiceInstanceId.TryParse(input.AsSpan(), CultureInfo.InvariantCulture, out var parsedSpanResult);

        // Assert
        Assert.Equal(expected, optionResult.IsSome);
        Assert.Equal(expected, optionSpanResult.IsSome);
        Assert.Equal(expected, bclResult);
        Assert.Equal(expected, bclSpanResult);

        if (expected)
        {
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedSpanResult);
            Assert.Equal(parsedResult, parsedSpanResult);
            _ = optionResult.IfSome(val => Assert.Equal(val, parsedResult));
        }
        else
        {
            Assert.Null(parsedResult);
            Assert.Null(parsedSpanResult);
        }
    }
}
