using System.ComponentModel;
using LanguageExt;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;
using TIKSN.Licensing;
using static LanguageExt.Prelude;

namespace TIKSN.FeatureManagement;

public abstract class LicenseEntitlementsFeatureProviderBase<TEntitlements> : FeatureProvider
{
    private readonly ILicenseFeatureResolver<TEntitlements> licenseFeatureResolver;
    private readonly TimeProvider timeProvider;

    protected LicenseEntitlementsFeatureProviderBase(
        ILicenseFeatureResolver<TEntitlements> licenseFeatureResolver,
        TimeProvider timeProvider)
    {
        this.licenseFeatureResolver =
            licenseFeatureResolver ?? throw new ArgumentNullException(nameof(licenseFeatureResolver));
        this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(
        string flagKey,
        bool defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        this.ResolveAsync(
            flagKey,
            defaultValue,
            context,
            this.licenseFeatureResolver.ResolveBoolean,
            cancellationToken);

    public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(
        string flagKey,
        double defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        this.ResolveAsync(
            flagKey,
            defaultValue,
            context,
            this.licenseFeatureResolver.ResolveDouble,
            cancellationToken);

    public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(
        string flagKey,
        int defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        this.ResolveAsync(
            flagKey,
            defaultValue,
            context,
            this.licenseFeatureResolver.ResolveInteger,
            cancellationToken);

    public override Task<ResolutionDetails<string>> ResolveStringValueAsync(
        string flagKey,
        string defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        this.ResolveAsync(
            flagKey,
            defaultValue,
            context,
            this.licenseFeatureResolver.ResolveString,
            cancellationToken);

    public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(
        string flagKey,
        Value defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default) =>
        this.ResolveAsync(
            flagKey,
            defaultValue,
            context,
            this.licenseFeatureResolver.ResolveStructure,
            cancellationToken);

    protected static Option<TIdentity> TryExtractIdentityFromContext<TIdentity>(
        EvaluationContext? context,
        string contextKey)
    {
        if (context is null || !context.TryGetValue(contextKey, out var value))
        {
            return Option<TIdentity>.None;
        }

        var rawString = value?.AsString;
        if (string.IsNullOrWhiteSpace(rawString))
        {
            return Option<TIdentity>.None;
        }

        if (rawString is TIdentity stringIdentity)
        {
            return Some(stringIdentity);
        }

        if (typeof(TIdentity) == typeof(Ulid) &&
            Ulid.TryParse(rawString, out var parsedUlid))
        {
            return Some((TIdentity)(object)parsedUlid);
        }

        var converter = TypeDescriptor.GetConverter(typeof(TIdentity));
        if (!converter.CanConvertFrom(typeof(string)))
        {
            return Option<TIdentity>.None;
        }

        try
        {
            var converted = converter.ConvertFromInvariantString(rawString);
            return converted is TIdentity typedIdentity
                ? Some(typedIdentity)
                : Option<TIdentity>.None;
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or NotSupportedException)
        {
            return Option<TIdentity>.None;
        }
    }

    protected abstract Task<Option<License<TEntitlements>>> ResolveLicenseAsync(
        EvaluationContext? context,
        CancellationToken cancellationToken);

    private async Task<ResolutionDetails<T>> ResolveAsync<T>(
        string flagKey,
        T defaultValue,
        EvaluationContext? context,
        Func<TEntitlements, string, Option<T>> resolver,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flagKey);

        var licenseOption = await this.ResolveLicenseAsync(context, cancellationToken).ConfigureAwait(false);
        return licenseOption.Match(
            license =>
            {
                var now = this.timeProvider.GetUtcNow();
                if (now < license.Terms.NotBefore || now > license.Terms.NotAfter)
                {
                    return new ResolutionDetails<T>(
                        flagKey,
                        defaultValue,
                        ErrorType.None,
                        Reason.Disabled,
                        errorMessage: "License is outside its valid time window.");
                }

                return resolver(license.Entitlements, flagKey).Match(
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
                        errorMessage: $"Flag '{flagKey}' was not found in license entitlements."));
            },
            () => new ResolutionDetails<T>(
                flagKey,
                defaultValue,
                ErrorType.FlagNotFound,
                Reason.Default,
                errorMessage: "No active license was found."));
    }
}
