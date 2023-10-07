using Microsoft.Extensions.Options;
using TIKSN.Configuration.ValidationStrategy;

namespace TIKSN.Configuration
{
    public class PartialConfiguration<T> : IPartialConfiguration<T> where T : class, new()
    {
        private readonly IConfigurationValidationStrategy<T> _configurationValidationStrategy;
        private readonly IOptions<T> _options;

        public PartialConfiguration(IOptions<T> options,
            IConfigurationValidationStrategy<T> configurationValidationStrategy)
        {
            this._options = options;
            this._configurationValidationStrategy = configurationValidationStrategy;
        }

        public T GetConfiguration()
        {
            var config = this._options.Value;

            this._configurationValidationStrategy.Validate(config);

            return config;
        }
    }
}
