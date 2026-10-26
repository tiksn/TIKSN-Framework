using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenFeature;
using OpenFeature.Hosting;
using TIKSN.FeatureManagement;
using TIKSN.Identity;

namespace TIKSN.DependencyInjection;

public static class OpenFeatureBuilderExtensions
{
    public static OpenFeatureBuilder AddConfigurationProvider(
        this OpenFeatureBuilder builder) =>
        AddConfigurationProvider(builder, configureOptions: null);

    public static OpenFeatureBuilder AddConfigurationProvider(
        this OpenFeatureBuilder builder,
        Action<ConfigurationFeatureProviderOptions>? configureOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configureOptions is not null)
        {
            _ = builder.Services.Configure(configureOptions);
        }

        return builder.AddProvider(sp => new ConfigurationFeatureProvider(
            sp.GetRequiredService<IConfiguration>(),
            sp.GetRequiredService<IOptions<ConfigurationFeatureProviderOptions>>()));
    }

    public static OpenFeatureBuilder AddConfigurationProvider(
        this OpenFeatureBuilder builder,
        string domain) =>
        AddConfigurationProvider(builder, domain, configureOptions: null);

    public static OpenFeatureBuilder AddConfigurationProvider(
        this OpenFeatureBuilder builder,
        string domain,
        Action<ConfigurationFeatureProviderOptions>? configureOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        if (configureOptions is not null)
        {
            _ = builder.Services.Configure(configureOptions);
        }

        return builder.AddProvider(domain, (sp, _) => new ConfigurationFeatureProvider(
            sp.GetRequiredService<IConfiguration>(),
            sp.GetRequiredService<IOptions<ConfigurationFeatureProviderOptions>>()));
    }

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
