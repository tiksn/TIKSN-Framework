using System;
using System.Collections.Generic;
using System.Globalization;
using TIKSN.Identity;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Identity;

public class ServiceIdentityTests
{
    [Fact]
    public void Constructor_ApplicationNameIsNullOrWhiteSpace_ThrowsArgumentException()
    {
        // Arrange
        string? applicationName1 = null;
        var applicationName2 = "";
        var applicationName3 = " ";
        var componentNames = Seq1("ServiceA");
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        // Act & Assert
#pragma warning disable CS8604 // Possible null reference argument.
        _ = Assert.Throws<ArgumentException>(() => new ServiceIdentity(applicationName1, componentNames, instanceId));
#pragma warning restore CS8604 // Possible null reference argument.
        _ = Assert.Throws<ArgumentException>(() => new ServiceIdentity(applicationName2, componentNames, instanceId));
        _ = Assert.Throws<ArgumentException>(() => new ServiceIdentity(applicationName3, componentNames, instanceId));
    }

    [Fact]
    public void Constructor_EmptyComponentNames_ComponentPathIsNone()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq<string>();
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        // Act
        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Assert
        Assert.True(serviceIdentity.ComponentPath.IsNone);
    }

    [Fact]
    public void Constructor_NullComponentNames_ComponentPathIsNone()
    {
        // Arrange
        var applicationName = "MyApp";
        IEnumerable<string>? componentNames = null;
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        // Act
#pragma warning disable CS8604 // Possible null reference argument.
        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        Assert.True(serviceIdentity.ComponentPath.IsNone);
    }

    [Fact]
    public void Constructor_NullInstanceId_ThrowsArgumentNullException()
    {
        var applicationName = "MyApp";
        var componentNames = Seq1("ServiceA");

        _ = Assert.Throws<ArgumentNullException>(() => new ServiceIdentity(applicationName, componentNames, null!));
    }

    [Fact]
    public void Constructor_ValidArguments_CreatesInstance()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq(["ServiceA", "ComponentB"]);
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        // Act
        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Assert
        Assert.Equal(applicationName, serviceIdentity.ApplicationName);
        Assert.Equal(componentNames, serviceIdentity.ComponentNames);
        Assert.Equal(instanceId, serviceIdentity.InstanceId);
        Assert.True(serviceIdentity.ComponentPath.IsSome);
        _ = serviceIdentity.ComponentPath.IfSome(path => Assert.Equal("ServiceA.ComponentB", path));
    }

    [Fact]
    public void CreateAnotherInstance_CreatesNewInstanceWithDifferentId()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq1("ServiceA");
        var instanceId1 = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);
        var serviceIdentity1 = new ServiceIdentity(applicationName, componentNames, instanceId1);
        var instanceId2 = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        // Act
        var serviceIdentity2 = serviceIdentity1.CreateAnotherInstance(instanceId2);

        // Assert
        Assert.Equal(serviceIdentity1.ApplicationName, serviceIdentity2.ApplicationName);
        Assert.Equal(serviceIdentity1.ComponentNames, serviceIdentity2.ComponentNames);
        Assert.NotEqual(serviceIdentity1.InstanceId, serviceIdentity2.InstanceId);
        Assert.Equal(instanceId2, serviceIdentity2.InstanceId);
        Assert.Equal(serviceIdentity1.ComponentPath, serviceIdentity2.ComponentPath);
    }

    [Fact]
    public void Equals_DifferentApplicationName_ReturnsFalse()
    {
        // Arrange
        var applicationName1 = "MyApp1";
        var applicationName2 = "MyApp2";
        var componentNames = Seq1("ServiceA");
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        var serviceIdentity1 = new ServiceIdentity(applicationName1, componentNames, instanceId);
        var serviceIdentity2 = new ServiceIdentity(applicationName2, componentNames, instanceId);

        // Act & Assert
        Assert.False(serviceIdentity1.Equals(serviceIdentity2));
        Assert.True(serviceIdentity1 != serviceIdentity2);
    }

    [Fact]
    public void Equals_DifferentComponentNames_ReturnsFalse()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames1 = Seq1("ServiceA");
        var componentNames2 = Seq1("ServiceB");
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        var serviceIdentity1 = new ServiceIdentity(applicationName, componentNames1, instanceId);
        var serviceIdentity2 = new ServiceIdentity(applicationName, componentNames2, instanceId);

        // Act & Assert
        Assert.False(serviceIdentity1.Equals(serviceIdentity2));
        Assert.True(serviceIdentity1 != serviceIdentity2);
    }

    [Fact]
    public void Equals_DifferentInstanceId_ReturnsFalse()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq1("ServiceA");
        var instanceId1 = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);
        var instanceId2 = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        var serviceIdentity1 = new ServiceIdentity(applicationName, componentNames, instanceId1);
        var serviceIdentity2 = new ServiceIdentity(applicationName, componentNames, instanceId2);

        // Act & Assert
        Assert.False(serviceIdentity1.Equals(serviceIdentity2));
        Assert.True(serviceIdentity1 != serviceIdentity2);
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq(["ServiceA", "ComponentB"]);
        var instanceId = ServiceInstanceId.Parse(Guid.NewGuid().ToString(), CultureInfo.InvariantCulture);

        var serviceIdentity1 = new ServiceIdentity(applicationName, componentNames, instanceId);
        var serviceIdentity2 = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Act & Assert
        Assert.True(serviceIdentity1.Equals(serviceIdentity2));
        Assert.True(serviceIdentity1 == serviceIdentity2);
        Assert.Equal(serviceIdentity1.GetHashCode(), serviceIdentity2.GetHashCode());
    }

    [Fact]
    public void Interfaces_ShouldImplementExpectedInterfaces()
    {
        AssertSpanParsable<ServiceIdentity>();
        AssertSpanFormattable<ServiceIdentity>();

        static void AssertSpanParsable<T>() where T : ISpanParsable<T>, IParsable<T>
        {
        }

        static void AssertSpanFormattable<T>() where T : ISpanFormattable, IFormattable
        {
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("MyApp")]
    [InlineData("MyApp.ServiceA")]
    [InlineData(":123")]
    [InlineData("   :123")]
    [InlineData("MyApp:")]
    [InlineData("MyApp:   ")]
    [InlineData("MyApp:invalid-id")]
    [InlineData("MyApp:-123")]
    [InlineData("MyApp:ServiceA:123")]
    [InlineData(".MyApp:123")]
    [InlineData("MyApp.:123")]
    [InlineData("MyApp..ServiceA:123")]
    [InlineData("MyApp.   :123")]
    [InlineData("MyApp.   .ServiceA:123")]
    [InlineData(" MyApp:123")]
    [InlineData("MyApp :123")]
    [InlineData("MyApp. ServiceA:123")]
    [InlineData("MyApp.ServiceA :123")]
    [InlineData("MyApp: 123")]
    [InlineData("MyApp:123 ")]
    public void Parse_InvalidInputs_ShouldThrowFormatException(string input)
    {
        _ = Assert.Throws<FormatException>(() => ServiceIdentity.Parse(input, CultureInfo.InvariantCulture));
        _ = Assert.Throws<FormatException>(() => ServiceIdentity.Parse(input.AsSpan(), CultureInfo.InvariantCulture));
#pragma warning disable CA1305 // Specify IFormatProvider
        _ = Assert.Throws<FormatException>(() => ServiceIdentity.Parse(input));
        _ = Assert.Throws<FormatException>(() => ServiceIdentity.Parse(input.AsSpan()));
#pragma warning restore CA1305 // Specify IFormatProvider
    }

    [Fact]
    public void Parse_Null_ShouldThrowArgumentNullException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => ServiceIdentity.Parse(null!, CultureInfo.InvariantCulture));
#pragma warning disable CA1305 // Specify IFormatProvider
        _ = Assert.Throws<ArgumentNullException>(() => ServiceIdentity.Parse(null!));
#pragma warning restore CA1305 // Specify IFormatProvider
    }

    [Theory]
    [InlineData("MyApp:123", "MyApp", new string[0], "123")]
    [InlineData("MyApp.ServiceA:123", "MyApp", new[]
    {
        "ServiceA"
    }, "123")]
    [InlineData("MyApp.ServiceA.ComponentB:123", "MyApp", new[]
    {
        "ServiceA",
        "ComponentB"
    }, "123")]
    [InlineData("MyApp.ServiceA.ComponentB.SubC:9876543210", "MyApp", new[]
    {
        "ServiceA",
        "ComponentB",
        "SubC"
    }, "9876543210")]
    [InlineData("MyApp:00000000-0000-0000-0000-000000000001", "MyApp", new string[0],
        "00000000-0000-0000-0000-000000000001")]
    [InlineData("MyApp.ServiceA:01H8XGJWBWBAQ4J1Z5F4ANNJ4A", "MyApp", new[]
    {
        "ServiceA"
    }, "01H8XGJWBWBAQ4J1Z5F4ANNJ4A")]
    public void Parse_ValidInputs_ShouldSucceed(string input, string expectedApp, string[] expectedComponents,
        string expectedId)
    {
        var identity = ServiceIdentity.Parse(input, CultureInfo.InvariantCulture);
        var identitySpan = ServiceIdentity.Parse(input.AsSpan(), CultureInfo.InvariantCulture);
#pragma warning disable CA1305 // Specify IFormatProvider
        var identityDefault = ServiceIdentity.Parse(input);
        var identitySpanDefault = ServiceIdentity.Parse(input.AsSpan());
#pragma warning restore CA1305 // Specify IFormatProvider

        var expectedInstanceId = ServiceInstanceId.Parse(expectedId, CultureInfo.InvariantCulture);

        Assert.Equal(expectedApp, identity.ApplicationName);
        Assert.Equal(Seq(expectedComponents), identity.ComponentNames);
        Assert.Equal(expectedInstanceId, identity.InstanceId);

        Assert.Equal(identity, identitySpan);
        Assert.Equal(identity, identityDefault);
        Assert.Equal(identity, identitySpanDefault);
    }

    [Theory]
    [InlineData("MyApp", new string[0], "123")]
    [InlineData("MyApp", new[]
    {
        "ServiceA"
    }, "456")]
    [InlineData("MyApp", new[]
    {
        "ServiceA",
        "ComponentB"
    }, "00000000-0000-0000-0000-000000000001")]
    [InlineData("AppX", new[]
    {
        "A",
        "B",
        "C"
    }, "01H8XGJWBWBAQ4J1Z5F4ANNJ4A")]
    public void RoundTrip_FormattingAndParsing_ShouldPreserveIdentity(string appName, string[] components, string idStr)
    {
        var instanceId = ServiceInstanceId.Parse(idStr, CultureInfo.InvariantCulture);
        var original = new ServiceIdentity(appName, Seq(components), instanceId);

        var formatted = original.ToString();
        var parsed = ServiceIdentity.Parse(formatted, CultureInfo.InvariantCulture);

        Assert.Equal(original, parsed);
        Assert.Equal(original.ApplicationName, parsed.ApplicationName);
        Assert.Equal(original.ComponentNames, parsed.ComponentNames);
        Assert.Equal(original.InstanceId, parsed.InstanceId);
        Assert.Equal(original.ServicePath, parsed.ServicePath);
        Assert.Equal(original.ComponentPath, parsed.ComponentPath);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("a")]
    public void ToString_ApplicationNameFormat_ReturnsApplicationName(string format)
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);

        Assert.Equal("MyApp", identity.ToString(format, CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("C")]
    [InlineData("c")]
    public void ToString_ComponentPathFormat_ReturnsComponentPath(string format)
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identityWithComponents = new ServiceIdentity("MyApp", Seq(["ServiceA", "ComponentB"]), instanceId);
        var identityWithoutComponents = new ServiceIdentity("MyApp", Seq<string>(), instanceId);

        Assert.Equal("ServiceA.ComponentB", identityWithComponents.ToString(format, CultureInfo.InvariantCulture));
        Assert.Equal(string.Empty, identityWithoutComponents.ToString(format, CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("G")]
    [InlineData("g")]
    public void ToString_GeneralFormats_ReturnsFullIdentity(string? format)
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);

        Assert.Equal("MyApp.ServiceA:123", identity.ToString(format, CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("I")]
    [InlineData("i")]
    public void ToString_InstanceIdFormat_ReturnsInstanceId(string format)
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);

        Assert.Equal("123", identity.ToString(format, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToString_NoComponents_ReturnsApplicationNameWithId()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq<string>();
        var instanceId = ServiceInstanceId.Parse("00000000-0000-0000-0000-000000000002", CultureInfo.InvariantCulture);

        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Act
        var result = serviceIdentity.ToString();

        // Assert
        Assert.Equal($"MyApp:{instanceId}", result);
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        // Arrange
        var applicationName = "MyApp";
        var componentNames = Seq(["ServiceA", "ComponentB"]);
        var instanceId = ServiceInstanceId.Parse("00000000-0000-0000-0000-000000000001", CultureInfo.InvariantCulture);

        var serviceIdentity = new ServiceIdentity(applicationName, componentNames, instanceId);

        // Act
        var result = serviceIdentity.ToString();

        // Assert
        Assert.Equal($"MyApp.ServiceA.ComponentB:{instanceId}", result);
    }

    [Theory]
    [InlineData("S")]
    [InlineData("s")]
    public void ToString_ServicePathFormat_ReturnsServicePath(string format)
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq(["ServiceA", "ComponentB"]), instanceId);

        Assert.Equal("MyApp.ServiceA.ComponentB", identity.ToString(format, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToString_UnsupportedFormat_ThrowsFormatException()
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);

        _ = Assert.Throws<FormatException>(() => identity.ToString("INVALID", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TryFormat_GeneralFormat_DestinationTooSmall_ShouldReturnFalse()
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);

        // Destination smaller than required
        Span<char> smallBuffer = stackalloc char[5];
        var success = identity.TryFormat(smallBuffer, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    public void TryFormat_GeneralFormat_ExactAndOversizedDestination_ShouldSucceed()
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);
        var expected = "MyApp.ServiceA:123";

        // Exact buffer
        Span<char> exactBuffer = stackalloc char[expected.Length];
        var successExact =
            identity.TryFormat(exactBuffer, out var charsWrittenExact, "G", CultureInfo.InvariantCulture);
        Assert.True(successExact);
        Assert.Equal(expected.Length, charsWrittenExact);
        Assert.Equal(expected, new string(exactBuffer));

        // Oversized buffer
        Span<char> oversizedBuffer = stackalloc char[50];
        var successOver = identity.TryFormat(oversizedBuffer, out var charsWrittenOver, default,
            CultureInfo.InvariantCulture);
        Assert.True(successOver);
        Assert.Equal(expected.Length, charsWrittenOver);
        Assert.Equal(expected, oversizedBuffer[..charsWrittenOver].ToString());
    }

    [Theory]
    [InlineData("S")]
    [InlineData("A")]
    [InlineData("C")]
    [InlineData("I")]
    public void TryFormat_SpecificFormats_DestinationTooSmall_ShouldReturnFalse(string format)
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);

        Span<char> buffer = stackalloc char[1];
        var success = identity.TryFormat(buffer, out var charsWritten, format, CultureInfo.InvariantCulture);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    [Theory]
    [InlineData("S", "MyApp.ServiceA")]
    [InlineData("A", "MyApp")]
    [InlineData("C", "ServiceA")]
    [InlineData("I", "123")]
    public void TryFormat_SpecificFormats_ShouldSucceed(string format, string expected)
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);

        Span<char> buffer = stackalloc char[expected.Length];
        var success = identity.TryFormat(buffer, out var charsWritten, format, CultureInfo.InvariantCulture);

        Assert.True(success);
        Assert.Equal(expected.Length, charsWritten);
        Assert.Equal(expected, new string(buffer));
    }

    [Fact]
    public void TryFormat_UnsupportedFormat_ThrowsFormatException()
    {
        var instanceId = ServiceInstanceId.Parse("123", CultureInfo.InvariantCulture);
        var identity = new ServiceIdentity("MyApp", Seq1("ServiceA"), instanceId);

        var buffer = new char[50];
        _ = Assert.Throws<FormatException>(() =>
        {
            var span = buffer.AsSpan();
            _ = identity.TryFormat(span, out _, "UNKNOWN", CultureInfo.InvariantCulture);
        });
    }

    [Fact]
    public void TryParse_NullString_ShouldReturnFalse()
    {
        var bclResult = ServiceIdentity.TryParse(null, CultureInfo.InvariantCulture, out var parsedResult);
        Assert.False(bclResult);
        Assert.Null(parsedResult);
    }

    [Theory]
    [InlineData("MyApp:123", true)]
    [InlineData("MyApp.ServiceA:123", true)]
    [InlineData("MyApp.ServiceA.ComponentB:123", true)]
    [InlineData("MyApp:00000000-0000-0000-0000-000000000001", true)]
    [InlineData("MyApp:01H8XGJWBWBAQ4J1Z5F4ANNJ4A", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("MyApp", false)]
    [InlineData(":123", false)]
    [InlineData("MyApp:", false)]
    [InlineData("MyApp:invalid-id", false)]
    [InlineData("MyApp:-1", false)]
    [InlineData(".MyApp:123", false)]
    [InlineData("MyApp.:123", false)]
    [InlineData("MyApp..Service:123", false)]
    public void TryParse_VariousInputs_ShouldReturnExpectedResult(string input, bool expected)
    {
        // Act - Option TryParse(string)
        var optionResult = ServiceIdentity.TryParse(input);

        // Act - Option TryParse(ReadOnlySpan<char>)
        var optionSpanResult = ServiceIdentity.TryParse(input.AsSpan());

        // Act - BCL TryParse(string, provider, out result)
        var bclResult = ServiceIdentity.TryParse(input, CultureInfo.InvariantCulture, out var parsedResult);

        // Act - BCL TryParse(ReadOnlySpan<char>, provider, out result)
        var bclSpanResult =
            ServiceIdentity.TryParse(input.AsSpan(), CultureInfo.InvariantCulture, out var parsedSpanResult);

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
