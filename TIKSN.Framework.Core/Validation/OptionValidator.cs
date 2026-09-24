using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;
using LanguageExt;

namespace TIKSN.Validation;

public class OptionValidator<T, TProperty> : PropertyValidator<T, Option<TProperty>>
{
    private readonly IValidator<TProperty> _validator;

    public OptionValidator(IValidator<TProperty> validator) => this._validator = validator ?? throw new ArgumentNullException(nameof(validator));

    public override string Name => "OptionValidator";

    public override bool IsValid(ValidationContext<T> context, Option<TProperty> value)
    {
        _ = value.IfSome(v => this.ValidateProperty(context, v));

        return true;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "Value is provided however is not valid";

    private void ValidateProperty(ValidationContext<T> context, TProperty value)
    {
        var validationResult = this._validator.Validate(value);
        var chain = new PropertyChain(context.PropertyChain);
        chain.Add(context.PropertyPath);
        foreach (var error in validationResult.Errors)
        {
            var propertyPath = chain.BuildPropertyPath(error.PropertyName);
            context.AddFailure(new ValidationFailure(propertyPath, error.ErrorMessage, error.AttemptedValue));
        }
    }
}
