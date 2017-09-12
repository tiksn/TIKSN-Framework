using Microsoft.Extensions.DependencyInjection;
using System;

namespace TIKSN.Configuration.ValidationStrategy
{
	public class OptionalConfigurationValidationStrategy<T> : ConfigurationValidationStrategyBase<T>
	{
		public OptionalConfigurationValidationStrategy(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override IPartialConfigurationValidator<T> GetConfigurationValidator()
		{
			return _serviceProvider.GetService<IPartialConfigurationValidator<T>>();
		}
	}
}
