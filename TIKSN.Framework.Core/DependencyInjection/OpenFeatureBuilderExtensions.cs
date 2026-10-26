using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenFeature;
using OpenFeature.Hosting;
using TIKSN.FeatureManagement;
using TIKSN.Identity;
using TIKSN.Licensing;
using TIKSN.Settings;

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

    public static OpenFeatureBuilder AddSettingsProvider(
        this OpenFeatureBuilder builder) =>
        AddSettingsProvider(builder, configureOptions: null);

    public static OpenFeatureBuilder AddSettingsProvider(
        this OpenFeatureBuilder builder,
        Action<SettingsFeatureProviderOptions>? configureOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (configureOptions is not null)
        {
            _ = builder.Services.Configure(configureOptions);
        }

        return builder.AddProvider(sp => new SettingsFeatureProvider(
            sp.GetRequiredService<ISettingsService>(),
            sp.GetRequiredService<IOptions<SettingsFeatureProviderOptions>>()));
    }

    public static OpenFeatureBuilder AddSettingsProvider(
        this OpenFeatureBuilder builder,
        string domain) =>
        AddSettingsProvider(builder, domain, configureOptions: null);

    public static OpenFeatureBuilder AddSettingsProvider(
        this OpenFeatureBuilder builder,
        string domain,
        Action<SettingsFeatureProviderOptions>? configureOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        if (configureOptions is not null)
        {
            _ = builder.Services.Configure(configureOptions);
        }

        return builder.AddProvider(domain, (sp, _) => new SettingsFeatureProvider(
            sp.GetRequiredService<ISettingsService>(),
            sp.GetRequiredService<IOptions<SettingsFeatureProviderOptions>>()));
    }

    public static OpenFeatureBuilder AddSystemLicenseEntitlementsProvider<TEntitlements>(
        this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddProvider(sp => new SystemLicenseEntitlementsFeatureProvider<TEntitlements>(
            sp.GetRequiredService<ISystemLicenseProvider<TEntitlements>>(),
            sp.GetRequiredService<ILicenseFeatureResolver<TEntitlements>>(),
            sp.GetRequiredService<TimeProvider>()));
    }

    public static OpenFeatureBuilder AddSystemLicenseEntitlementsProvider<TEntitlements>(
        this OpenFeatureBuilder builder,
        string domain)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return builder.AddProvider(domain, (sp, _) => new SystemLicenseEntitlementsFeatureProvider<TEntitlements>(
            sp.GetRequiredService<ISystemLicenseProvider<TEntitlements>>(),
            sp.GetRequiredService<ILicenseFeatureResolver<TEntitlements>>(),
            sp.GetRequiredService<TimeProvider>()));
    }

    public static OpenFeatureBuilder AddTenantContextHook<TTenantIdentity>(this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHook(sp =>
            new TenantContextHook<TTenantIdentity>(sp.GetRequiredService<ITenantIdProvider<TTenantIdentity>>()));
    }

    public static OpenFeatureBuilder AddTenantLicenseEntitlementsProvider<TTenantIdentity, TEntitlements>(
        this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddProvider(sp => new TenantLicenseEntitlementsFeatureProvider<TTenantIdentity, TEntitlements>(
            sp.GetRequiredService<ITenantLicenseProvider<TTenantIdentity, TEntitlements>>(),
            sp.GetRequiredService<ITenantIdProvider<TTenantIdentity>>(),
            sp.GetRequiredService<ILicenseFeatureResolver<TEntitlements>>(),
            sp.GetRequiredService<TimeProvider>()));
    }

    public static OpenFeatureBuilder AddTenantLicenseEntitlementsProvider<TTenantIdentity, TEntitlements>(
        this OpenFeatureBuilder builder,
        string domain)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return builder.AddProvider(
            domain,
            (sp, _) => new TenantLicenseEntitlementsFeatureProvider<TTenantIdentity, TEntitlements>(
                sp.GetRequiredService<ITenantLicenseProvider<TTenantIdentity, TEntitlements>>(),
                sp.GetRequiredService<ITenantIdProvider<TTenantIdentity>>(),
                sp.GetRequiredService<ILicenseFeatureResolver<TEntitlements>>(),
                sp.GetRequiredService<TimeProvider>()));
    }

    public static OpenFeatureBuilder AddUserContextHook<TUserIdentity>(this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddHook(sp =>
            new UserContextHook<TUserIdentity>(sp.GetRequiredService<IUserIdProvider<TUserIdentity>>()));
    }

    public static OpenFeatureBuilder AddUserLicenseEntitlementsProvider<TUserIdentity, TEntitlements>(
        this OpenFeatureBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddProvider(sp => new UserLicenseEntitlementsFeatureProvider<TUserIdentity, TEntitlements>(
            sp.GetRequiredService<IUserLicenseProvider<TUserIdentity, TEntitlements>>(),
            sp.GetRequiredService<IUserIdProvider<TUserIdentity>>(),
            sp.GetRequiredService<ILicenseFeatureResolver<TEntitlements>>(),
            sp.GetRequiredService<TimeProvider>()));
    }

    public static OpenFeatureBuilder AddUserLicenseEntitlementsProvider<TUserIdentity, TEntitlements>(
        this OpenFeatureBuilder builder,
        string domain)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return builder.AddProvider(
            domain,
            (sp, _) => new UserLicenseEntitlementsFeatureProvider<TUserIdentity, TEntitlements>(
                sp.GetRequiredService<IUserLicenseProvider<TUserIdentity, TEntitlements>>(),
                sp.GetRequiredService<IUserIdProvider<TUserIdentity>>(),
                sp.GetRequiredService<ILicenseFeatureResolver<TEntitlements>>(),
                sp.GetRequiredService<TimeProvider>()));
    }
}
