namespace TIKSN.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrameworkPlatform(
        this IServiceCollection services)
    {
        _ = services.AddFrameworkCore();

        _ = services.AddFrameworkPlatformBase();

        return services;
    }
}
