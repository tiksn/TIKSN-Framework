using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIKSN.Configuration;
using TIKSN.Configuration.ValidationStrategy;
using TIKSN.Configuration.Validator;

namespace TIKSN.DependencyInjection
{
    public static class PartialConfigurationServiceCollectionExtensions
    {
        public static IServiceCollection ConfigurePartial<TOptions>(this IServiceCollection services,
            IConfiguration config, bool mandatoryValidation = false) where TOptions : class, new()
        {
            if (mandatoryValidation)
            {
                _ = services
                    .AddSingleton<IConfigurationValidationStrategy<TOptions>,
                        MandatoryConfigurationValidationStrategy<TOptions>>();
            }
            else
            {
                _ = services
                    .AddSingleton<IConfigurationValidationStrategy<TOptions>,
                        OptionalConfigurationValidationStrategy<TOptions>>();
            }

            _ = services.AddSingleton<IPartialConfiguration<TOptions>, PartialConfiguration<TOptions>>();

            return services.Configure<TOptions>(config);
        }

        public static IServiceCollection ConfigurePartial<TOptions, TValidator>(this IServiceCollection services,
            IConfiguration config)
            where TOptions : class, new()
            where TValidator : class, IPartialConfigurationValidator<TOptions>
        {
            _ = services.AddSingleton<IPartialConfigurationValidator<TOptions>, TValidator>();

            return services.ConfigurePartial<TOptions>(config, true);
        }
    }
}
