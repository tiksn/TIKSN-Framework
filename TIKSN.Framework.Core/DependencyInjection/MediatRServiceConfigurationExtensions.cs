using Microsoft.Extensions.DependencyInjection;
using TIKSN.Integration.Messages;

namespace TIKSN.DependencyInjection;

public static class MediatRServiceConfigurationExtensions
{
    public static MediatRServiceConfiguration AddPagingQueryPipelineBehavior(
        this MediatRServiceConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _ = configuration.AddOpenBehavior(typeof(PagingQueryBehavior<,>));
        return configuration;
    }

    public static MediatRServiceConfiguration AddTenantPipelineBehaviors(this MediatRServiceConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _ = configuration.AddOpenBehavior(typeof(TenantRequestBehavior<,,,>));
        return configuration;
    }

    public static MediatRServiceConfiguration AddValidationPipelineBehavior(
        this MediatRServiceConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _ = configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        return configuration;
    }
}
