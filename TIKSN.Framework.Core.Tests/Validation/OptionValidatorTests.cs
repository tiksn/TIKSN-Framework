using FluentValidation;
using LanguageExt;
using Shouldly;
using TIKSN.Validation;
using Xunit;

namespace TIKSN.Tests.Validation;

public class OptionValidatorTests
{
    [Fact]
    public void Validate_None_ShouldBeValid()
    {
        var validator = new TestClassValidator();
        var model = new TestClass { OptionalString = Option<string>.None };

        var result = validator.Validate(model);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_SomeInvalid_ShouldBeInvalid()
    {
        var validator = new TestClassValidator();
        var model = new TestClass { OptionalString = Option<string>.Some("") };

        var result = validator.Validate(model);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public void Validate_SomeValid_ShouldBeValid()
    {
        var validator = new TestClassValidator();
        var model = new TestClass { OptionalString = Option<string>.Some("Valid") };

        var result = validator.Validate(model);

        result.IsValid.ShouldBeTrue();
    }

    public class StringValidator : AbstractValidator<string>
    {
        public StringValidator() => this.RuleFor(x => x).NotEmpty();
    }

    public class TestClass
    {
        public Option<string> OptionalString { get; set; }
    }

    public class TestClassValidator : AbstractValidator<TestClass>
    {
        public TestClassValidator() => this.RuleFor(x => x.OptionalString).IfSome(new StringValidator());
    }
}
