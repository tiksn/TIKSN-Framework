using LanguageExt;
using Microsoft.Extensions.Options;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;
using TIKSN.Settings;

namespace TIKSN.FeatureManagement;

public sealed class SettingsFeatureProvider : FeatureProvider
{
    private const string ProviderName = "TIKSN.SettingsFeatureProvider";

    private static readonly Metadata ProviderMetadata = new(ProviderName);

    private readonly IOptions<SettingsFeatureProviderOptions> options;
    private readonly ISettingsService settingsService;

    public SettingsFeatureProvider(
        ISettingsService settingsService,
        IOptions<SettingsFeatureProviderOptions> options)
    {
        this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public override Metadata GetMetadata() => ProviderMetadata;

    public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(
        string flagKey,
        bool defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(this.ResolvePrimitive(flagKey, defaultValue));

    public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(
        string flagKey,
        double defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(this.ResolvePrimitive(flagKey, defaultValue));

    public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(
        string flagKey,
        int defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(this.ResolvePrimitive(flagKey, defaultValue));

    public override Task<ResolutionDetails<string>> ResolveStringValueAsync(
        string flagKey,
        string defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(this.ResolvePrimitive(flagKey, defaultValue));

    public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(
        string flagKey,
        Value defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flagKey);

        try
        {
            var settingOption = this.LookupSetting<string>(this.GetSettingName(flagKey));
            return Task.FromResult(settingOption.Match(
                some => new ResolutionDetails<Value>(
                    flagKey,
                    new Value(some),
                    ErrorType.None,
                    Reason.Static),
                () => new ResolutionDetails<Value>(
                    flagKey,
                    defaultValue,
                    ErrorType.FlagNotFound,
                    Reason.Default,
                    errorMessage: $"Flag '{flagKey}' was not found in settings.")));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(new ResolutionDetails<Value>(
                flagKey,
                defaultValue,
                ErrorType.TypeMismatch,
                Reason.Error,
                errorMessage: ex.Message));
        }
        catch (NotSupportedException ex)
        {
            return Task.FromResult(new ResolutionDetails<Value>(
                flagKey,
                defaultValue,
                ErrorType.TypeMismatch,
                Reason.Error,
                errorMessage: ex.Message));
        }
    }

    private string GetSettingName(string flagKey)
    {
        var prefix = this.options.Value.KeyPrefix;
        return string.IsNullOrEmpty(prefix) ? flagKey : string.Concat(prefix, flagKey);
    }

    private Option<T> LookupSetting<T>(string settingName) =>
        this.options.Value.LookupScope switch
        {
            SettingsLookupScope.LocalOnly => this.settingsService.GetLocalSetting<T>(settingName),
            SettingsLookupScope.RoamingOnly => this.settingsService.GetRoamingSetting<T>(settingName),
            SettingsLookupScope.LocalThenRoaming => this.settingsService.GetLocalSetting<T>(settingName) ||
                this.settingsService.GetRoamingSetting<T>(settingName),
            SettingsLookupScope.RoamingThenLocal => this.settingsService.GetRoamingSetting<T>(settingName) ||
                this.settingsService.GetLocalSetting<T>(settingName),
            _ => Option<T>.None,
        };

    private ResolutionDetails<T> ResolvePrimitive<T>(string flagKey, T defaultValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flagKey);

        try
        {
            var settingOption = this.LookupSetting<T>(this.GetSettingName(flagKey));
            return settingOption.Match(
                some => new ResolutionDetails<T>(
                    flagKey,
                    some,
                    ErrorType.None,
                    Reason.Static),
                () => new ResolutionDetails<T>(
                    flagKey,
                    defaultValue,
                    ErrorType.FlagNotFound,
                    Reason.Default,
                    errorMessage: $"Flag '{flagKey}' was not found in settings."));
        }
        catch (InvalidOperationException ex)
        {
            return new ResolutionDetails<T>(
                flagKey,
                defaultValue,
                ErrorType.TypeMismatch,
                Reason.Error,
                errorMessage: ex.Message);
        }
        catch (NotSupportedException ex)
        {
            return new ResolutionDetails<T>(
                flagKey,
                defaultValue,
                ErrorType.TypeMismatch,
                Reason.Error,
                errorMessage: ex.Message);
        }
    }
}
