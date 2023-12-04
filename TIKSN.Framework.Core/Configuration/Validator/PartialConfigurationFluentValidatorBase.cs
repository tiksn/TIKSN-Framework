using FluentValidation;

namespace TIKSN.Configuration.Validator;

public abstract class PartialConfigurationFluentValidatorBase<T> : AbstractValidator<T>,
    IPartialConfigurationValidator<T>
{
    public void ValidateConfiguration(T instance)
    {
        try
        {
            this.ValidateAndThrow(instance);
        }
        catch (ValidationException ex)
        {
            throw new ConfigurationValidationException(ex.Message, ex);
        }
    }
}
