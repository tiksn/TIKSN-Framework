using Microsoft.Extensions.DependencyInjection;
using System;
using TIKSN.Configuration.Validator;

namespace TIKSN.Configuration.ValidationStrategy
{
	public class MandatoryConfigurationValidationStrategy<T> : ConfigurationValidationStrategyBase<T>
	{
		public MandatoryConfigurationValidationStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override IPartialConfigurationValidator<T> GetConfigurationValidator()
		{
			return _serviceProvider.GetRequiredService<IPartialConfigurationValidator<T>>();
		}
	}
}
