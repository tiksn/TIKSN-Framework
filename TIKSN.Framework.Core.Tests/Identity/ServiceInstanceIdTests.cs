using System;
using TIKSN.Identity;
using Xunit;

namespace TIKSN.Tests.Identity;

public class ServiceInstanceIdTests
{
    [Theory]
    [InlineData("123", true)]
    [InlineData("9876543210", true)]
    [InlineData("01H8XGJWBWBAQ4J1Z5F4ANNJ4A", true)]
    [InlineData("d8f2f6a2-4b3f-4e6a-8b0c-9a0a0a0a0a0a", true)]
    [InlineData("0", true)]
    [InlineData("-123", false)]
    [InlineData("abc", false)]
    [InlineData("01H8XGJWBWBAQ4J1Z5F4ANNJ4", false)] // Invalid Ulid (length 25)
    public void TryParse_ShouldReturnExpectedResult(string input, bool expected)
    {
        // Act
        var result = ServiceInstanceId.TryParse(input);

        // Assert
        Assert.Equal(expected, result.IsSome);
    }

    [Fact]
    public void Parse_InvalidFormat_ShouldThrowFormatException()
    {
        // Arrange
        var input = "invalid-id";

        // Act & Assert
        _ = Assert.Throws<FormatException>(() => ServiceInstanceId.Parse(input));
    }

    [Fact]
    public void Equals_SameValue_ShouldReturnTrue()
    {
        // Arrange
        var id1 = ServiceInstanceId.Parse("123");
        var id2 = ServiceInstanceId.Parse("123");

        // Act & Assert
        Assert.True(id1.Equals(id2));
        Assert.True(id1 == id2);
    }

    [Fact]
    public void Equals_DifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var id1 = ServiceInstanceId.Parse("123");
        var id2 = ServiceInstanceId.Parse("456");

        // Act & Assert
        Assert.False(id1.Equals(id2));
        Assert.True(id1 != id2);
    }

    [Fact]
    public void Equals_DifferentType_ShouldReturnFalse()
    {
        // Arrange
        var id1 = ServiceInstanceId.Parse("1234567890"); // long
        var id2 = ServiceInstanceId.Parse(Guid.NewGuid().ToString()); // Guid

        // Act & Assert
        Assert.False(id1.Equals(id2));
    }

    [Fact]
    public void ToString_ShouldReturnOriginalValue()
    {
        // Arrange
        var input = "d8f2f6a2-4b3f-4e6a-8b0c-9a0a0a0a0a0a";
        var id = ServiceInstanceId.Parse(input);

        // Act
        var result = id.ToString();

        // Assert
        Assert.Equal(input, result, StringComparer.OrdinalIgnoreCase);
    }
}
