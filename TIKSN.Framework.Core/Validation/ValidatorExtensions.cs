using FluentValidation;
using LanguageExt;

namespace TIKSN.Validation;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, Option<TProperty>> IfSome<T, TProperty>(
        this IRuleBuilder<T, Option<TProperty>> ruleBuilder,
        IValidator<TProperty> validator)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);
        return ruleBuilder.SetValidator(new OptionValidator<T, TProperty>(validator));
    }
}
