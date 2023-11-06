namespace TIKSN.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddFrameworkPlatformBase(
        this IServiceCollection services)
    {
        _ = services.AddFrameworkCore();

        return services;
    }
}
