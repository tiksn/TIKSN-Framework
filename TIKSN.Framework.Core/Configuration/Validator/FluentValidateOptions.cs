using FluentValidation;
using Microsoft.Extensions.Options;

namespace TIKSN.Configuration.Validator;

public abstract class FluentValidateOptions<T> : AbstractValidator<T>, IValidateOptions<T>
    where T : class
{
    public ValidateOptionsResult Validate(string? name, T options)
    {
        var result = this.Validate(options);

        return result.IsValid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(result.Errors.Select(error => error.ErrorMessage));
    }
}
