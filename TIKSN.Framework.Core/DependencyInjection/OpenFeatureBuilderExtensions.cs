using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenFeature;
using OpenFeature.Hosting;
using TIKSN.FeatureManagement;
using TIKSN.Identity;

namespace TIKSN.DependencyInjection;

public static class OpenFeatureBuilderExtensions
{
    public static OpenFeatureBuilder AddFrameworkContextHooks(this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder
            .AddGlobalizationContextHook()
            .AddHostEnvironmentContextHook()
            .AddServiceIdentityContextHook();
    }

    public static OpenFeatureBuilder AddGlobalizationContextHook(this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHook(_ => new GlobalizationContextHook());
    }

    public static OpenFeatureBuilder AddHostEnvironmentContextHook(this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHook(sp => new HostEnvironmentContextHook(sp.GetService<IHostEnvironment>()));
    }

    public static OpenFeatureBuilder AddServiceIdentityContextHook(this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHook(sp => new ServiceIdentityContextHook(sp.GetService<ServiceIdentity>()));
    }

    public static OpenFeatureBuilder AddTenantContextHook<TTenantIdentity>(this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHook(sp =>
            new TenantContextHook<TTenantIdentity>(sp.GetRequiredService<ITenantIdProvider<TTenantIdentity>>()));
    }

    public static OpenFeatureBuilder AddUserContextHook<TUserIdentity>(this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHook(sp =>
            new UserContextHook<TUserIdentity>(sp.GetRequiredService<IUserIdProvider<TUserIdentity>>()));
    }
}
