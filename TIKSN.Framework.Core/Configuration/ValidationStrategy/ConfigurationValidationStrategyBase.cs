using TIKSN.Configuration.Validator;

namespace TIKSN.Configuration.ValidationStrategy;

public abstract class ConfigurationValidationStrategyBase<T> : IConfigurationValidationStrategy<T>
{
    protected ConfigurationValidationStrategyBase(IServiceProvider serviceProvider) =>
        this.ServiceProvider = serviceProvider;

    protected IServiceProvider ServiceProvider { get; }

    public void Validate(T instance)
    {
        var validator = this.GetConfigurationValidator();

        validator?.ValidateConfiguration(instance);
    }

    protected abstract IPartialConfigurationValidator<T>? GetConfigurationValidator();
}
