using FluentValidation;
using LanguageExt.Common;
using Shouldly;
using TIKSN.Validation;
using Xunit;
using static LanguageExt.Prelude;

namespace TIKSN.Tests.Validation;

public class ValidationExtensionsTests
{
    [Fact]
    public void GetOrThrow_Fail_ThrowsValidationException()
    {
        var error = Error.New(1, "Test error");
        var validation = Fail<Error, string>(Seq1(error));

        var exception = Should.Throw<ValidationException>(() => validation.GetOrThrow());

        exception.Errors.ShouldNotBeEmpty();
    }

    [Fact]
    public void GetOrThrow_Success_ReturnsValue()
    {
        var validation = Success<Error, string>("Valid");

        var result = validation.GetOrThrow();

        result.ShouldBe("Valid");
    }
}
