using Microsoft.Extensions.DependencyInjection;
using TIKSN.Configuration.Validator;

namespace TIKSN.Configuration.ValidationStrategy;

public class MandatoryConfigurationValidationStrategy<T> : ConfigurationValidationStrategyBase<T>
{
    public MandatoryConfigurationValidationStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override IPartialConfigurationValidator<T> GetConfigurationValidator() =>
        this.ServiceProvider.GetRequiredService<IPartialConfigurationValidator<T>>();
}
