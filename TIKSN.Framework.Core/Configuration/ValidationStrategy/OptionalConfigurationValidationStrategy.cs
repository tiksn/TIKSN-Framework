using Microsoft.Extensions.DependencyInjection;
using TIKSN.Configuration.Validator;

namespace TIKSN.Configuration.ValidationStrategy;

public class OptionalConfigurationValidationStrategy<T> : ConfigurationValidationStrategyBase<T>
{
    public OptionalConfigurationValidationStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override IPartialConfigurationValidator<T>? GetConfigurationValidator() =>
        this.ServiceProvider.GetService<IPartialConfigurationValidator<T>>();
}
