using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shouldly;
using TIKSN.Configuration;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Configuration;

public class ConfigurationValidationExceptionTests
{
    [Fact]
    public void MultipleErrors_ShouldAppendToMessage()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var options = Options.Create(new TestOptions());

        // Act
        _ = ex.WithOptions(options, "Min", "10")
            .WithOptions(options, "Max", "5");

        // Assert
        ex.Errors.Count.ShouldBe(2);

        ex.Message.ShouldBe(
            "Test error [Options Type: 'TIKSN.Tests.Configuration.ConfigurationValidationExceptionTests+TestOptions', Key: 'Min', Value: '10'] [Options Type: 'TIKSN.Tests.Configuration.ConfigurationValidationExceptionTests+TestOptions', Key: 'Max', Value: '5']");
    }

    [Fact]
    public void WithConfigurationSectionSensitive_ShouldMarkValueSensitive()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var section = config.GetSection("Section:ApiKey");

        // Act
        _ = ex.WithConfigurationSectionSensitive(section);

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        _ = error.ConfigurationValue.IfLeft(l => l.ShouldBe(unit));

        ex.Message.ShouldBe("Test error [Path: 'Section:ApiKey', Key: 'ApiKey', Value is *** SENSITIVE ***]");
    }

    [Fact]
    public void WithConfigurationSectionSensitive_WithChildKey_ShouldMarkValueSensitive()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var section = config.GetSection("Section");

        // Act
        _ = ex.WithConfigurationSectionSensitive(section, "ApiKey");

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        error.ConfigurationPath.ShouldBe(Some("Section:ApiKey"));
        _ = error.ConfigurationValue.IfLeft(l => l.ShouldBe(unit));

        ex.Message.ShouldBe("Test error [Path: 'Section:ApiKey', Key: 'ApiKey', Value is *** SENSITIVE ***]");
    }

    [Fact]
    public void WithConfigurationSection_ShouldIncludePathKeyAndSectionValue()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "Section:Key", "SectionValue" } }).Build();
        var section = config.GetSection("Section:Key");

        // Act
        _ = ex.WithConfigurationSection(section);

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        error.OptionsType.IsNone.ShouldBeTrue();
        error.ConfigurationPath.ShouldBe(Some("Section:Key"));
        error.ConfigurationKey.ShouldBe(Some("Key"));
        error.ConfigurationValue.IsRight.ShouldBeTrue();
        _ = error.ConfigurationValue.IfRight(r => r.IfSome(v => v.ShouldBe("SectionValue")));

        ex.Message.ShouldBe("Test error [Path: 'Section:Key', Key: 'Key', Value: 'SectionValue']");
    }

    [Fact]
    public void WithConfigurationSection_WithIntValue_ShouldIncludeValue()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var section = config.GetSection("Section");

        // Act
        _ = ex.WithConfigurationSection(section, "Size", 99);

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        _ = error.ConfigurationValue.IfRight(r => r.IfSome(v => v.ShouldBe(99)));

        ex.Message.ShouldBe("Test error [Path: 'Section:Size', Key: 'Size', Value: '99']");
    }

    [Fact]
    public void WithConfigurationSection_WithNullValue_ShouldIncludeNullValue()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var section = config.GetSection("Section");

        // Act
        _ = ex.WithConfigurationSection(section, "MissingKey", (string?)null);

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        _ = error.ConfigurationValue.IfRight(r => r.IsNone.ShouldBeTrue());

        ex.Message.ShouldBe("Test error [Path: 'Section:MissingKey', Key: 'MissingKey', Value is null]");
    }

    [Fact]
    public void WithConfigurationSection_WithValue_ShouldIncludeValue()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "Section:Key", "MyValue" } }).Build();
        var section = config.GetSection("Section");

        // Act
        _ = ex.WithConfigurationSection(section, "Key", "MyValue");

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        _ = error.ConfigurationValue.IfRight(r => r.IfSome(v => v.ShouldBe("MyValue")));

        ex.Message.ShouldBe("Test error [Path: 'Section:Key', Key: 'Key', Value: 'MyValue']");
    }

    [Fact]
    public void WithOptionsSensitive_ShouldIncludeOptionsTypeAndKey_ButMarkValueSensitive()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var options = Options.Create(new TestOptions());

        // Act
        _ = ex.WithOptionsSensitive(options, "Password");

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        error.ConfigurationValue.IsLeft.ShouldBeTrue();
        _ = error.ConfigurationValue.IfLeft(l => l.ShouldBe(unit));

        ex.Message.ShouldBe(
            "Test error [Options Type: 'TIKSN.Tests.Configuration.ConfigurationValidationExceptionTests+TestOptions', Key: 'Password', Value is *** SENSITIVE ***]");
    }

    [Fact]
    public void WithOptions_WithIntValue_ShouldIncludeOptionsTypeKeyAndValue()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var options = Options.Create(new TestOptions());

        // Act
        _ = ex.WithOptions(options, "Size", 42);

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        error.ConfigurationValue.IsRight.ShouldBeTrue();

        // Assert we boxed the int correctly
        _ = error.ConfigurationValue.IfRight(r => r.IfSome(v => v.ShouldBe(42)));

        ex.Message.ShouldBe(
            "Test error [Options Type: 'TIKSN.Tests.Configuration.ConfigurationValidationExceptionTests+TestOptions', Key: 'Size', Value: '42']");
    }

    [Fact]
    public void WithOptions_WithNullValue_ShouldIncludeOptionsTypeKeyAndNullValue()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var options = Options.Create(new TestOptions());

        // Act
        _ = ex.WithOptions(options, "Size", (string?)null);

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        error.ConfigurationValue.IsRight.ShouldBeTrue();
        _ = error.ConfigurationValue.IfRight(r => r.IsNone.ShouldBeTrue());

        ex.Message.ShouldBe(
            "Test error [Options Type: 'TIKSN.Tests.Configuration.ConfigurationValidationExceptionTests+TestOptions', Key: 'Size', Value is null]");
    }

    [Fact]
    public void WithOptions_WithValue_ShouldIncludeOptionsTypeKeyAndValue()
    {
        // Arrange
        var ex = new ConfigurationValidationException("Test error");
        var options = Options.Create(new TestOptions());

        // Act
        _ = ex.WithOptions(options, "Size", "10");

        // Assert
        ex.Errors.Count.ShouldBe(1);
        var error = ex.Errors[0];
        error.ConfigurationValue.IsRight.ShouldBeTrue();
        _ = error.ConfigurationValue.IfRight(r => r.IfSome(v => v.ShouldBe("10")));

        ex.Message.ShouldBe(
            "Test error [Options Type: 'TIKSN.Tests.Configuration.ConfigurationValidationExceptionTests+TestOptions', Key: 'Size', Value: '10']");
    }

    private sealed class TestOptions
    {
        public int Size { get; set; }
    }
}
