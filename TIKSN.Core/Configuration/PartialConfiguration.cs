using Microsoft.Extensions.Options;
using TIKSN.Configuration.ValidationStrategy;

namespace TIKSN.Configuration
{
	public class PartialConfiguration<T> : IPartialConfiguration<T> where T : class, new()
	{
		private readonly IOptions<T> _options;
		private readonly IConfigurationValidationStrategy<T> _configurationValidationStrategy;

		public PartialConfiguration(IOptions<T> options, IConfigurationValidationStrategy<T> configurationValidationStrategy)
		{
			_options = options;
			_configurationValidationStrategy = configurationValidationStrategy;
		}

		public T GetConfiguration()
		{
			var config = _options.Value;

			_configurationValidationStrategy.Validate(config);

			return config;
		}
	}
}
