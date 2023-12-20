using Microsoft.Extensions.Options;
using TIKSN.Configuration.ValidationStrategy;

namespace TIKSN.Configuration;

public class PartialConfiguration<T> : IPartialConfiguration<T> where T : class, new()
{
    private readonly IConfigurationValidationStrategy<T> configurationValidationStrategy;
    private readonly IOptions<T> options;

    public PartialConfiguration(
        IOptions<T> options,
        IConfigurationValidationStrategy<T> configurationValidationStrategy)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.configurationValidationStrategy = configurationValidationStrategy ?? throw new ArgumentNullException(nameof(configurationValidationStrategy));
    }

    public T GetConfiguration()
    {
        var config = this.options.Value;

        this.configurationValidationStrategy.Validate(config);

        return config;
    }
}
