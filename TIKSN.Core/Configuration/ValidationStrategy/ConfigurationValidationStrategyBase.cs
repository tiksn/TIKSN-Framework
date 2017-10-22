using System;
using TIKSN.Configuration.Validator;

namespace TIKSN.Configuration.ValidationStrategy
{
    public abstract class ConfigurationValidationStrategyBase<T> : IConfigurationValidationStrategy<T>
    {
        protected readonly IServiceProvider _serviceProvider;

        protected ConfigurationValidationStrategyBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Validate(T instance)
        {
            var validator = GetConfigurationValidator();

            if (validator != null)
                validator.ValidateConfiguration(instance);
        }

        protected abstract IPartialConfigurationValidator<T> GetConfigurationValidator();
    }
}