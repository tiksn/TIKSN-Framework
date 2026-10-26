using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;

namespace TIKSN.FeatureManagement;

public sealed class ConfigurationFeatureProvider : FeatureProvider
{
    private const string ProviderName = "TIKSN.ConfigurationFeatureProvider";

    private static readonly Metadata ProviderMetadata = new(ProviderName);

    private readonly IConfiguration configuration;
    private readonly IOptions<ConfigurationFeatureProviderOptions> options;

    public ConfigurationFeatureProvider(
        IConfiguration configuration,
        IOptions<ConfigurationFeatureProviderOptions> options)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public override Metadata GetMetadata() => ProviderMetadata;

    public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(
        string flagKey,
        bool defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flagKey);

        var section = this.GetFlagSection(flagKey);
        if (section.Value is null)
        {
            return Task.FromResult(new ResolutionDetails<bool>(
                flagKey,
                defaultValue,
                ErrorType.FlagNotFound,
                Reason.Default,
                errorMessage: $"Flag '{flagKey}' was not found in configuration."));
        }

        if (bool.TryParse(section.Value, out var parsedValue))
        {
            return Task.FromResult(new ResolutionDetails<bool>(
                flagKey,
                parsedValue,
                ErrorType.None,
                Reason.Static));
        }

        return Task.FromResult(new ResolutionDetails<bool>(
            flagKey,
            defaultValue,
            ErrorType.TypeMismatch,
            Reason.Error,
            errorMessage: $"Flag '{flagKey}' value '{section.Value}' cannot be parsed as a boolean."));
    }

    public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(
        string flagKey,
        double defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flagKey);

        var section = this.GetFlagSection(flagKey);
        if (section.Value is null)
        {
            return Task.FromResult(new ResolutionDetails<double>(
                flagKey,
                defaultValue,
                ErrorType.FlagNotFound,
                Reason.Default,
                errorMessage: $"Flag '{flagKey}' was not found in configuration."));
        }

        if (double.TryParse(section.Value, NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture, out var parsedValue))
        {
            return Task.FromResult(new ResolutionDetails<double>(
                flagKey,
                parsedValue,
                ErrorType.None,
                Reason.Static));
        }

        return Task.FromResult(new ResolutionDetails<double>(
            flagKey,
            defaultValue,
            ErrorType.TypeMismatch,
            Reason.Error,
            errorMessage: $"Flag '{flagKey}' value '{section.Value}' cannot be parsed as a double."));
    }

    public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(
        string flagKey,
        int defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flagKey);

        var section = this.GetFlagSection(flagKey);
        if (section.Value is null)
        {
            return Task.FromResult(new ResolutionDetails<int>(
                flagKey,
                defaultValue,
                ErrorType.FlagNotFound,
                Reason.Default,
                errorMessage: $"Flag '{flagKey}' was not found in configuration."));
        }

        if (int.TryParse(section.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue))
        {
            return Task.FromResult(new ResolutionDetails<int>(
                flagKey,
                parsedValue,
                ErrorType.None,
                Reason.Static));
        }

        return Task.FromResult(new ResolutionDetails<int>(
            flagKey,
            defaultValue,
            ErrorType.TypeMismatch,
            Reason.Error,
            errorMessage: $"Flag '{flagKey}' value '{section.Value}' cannot be parsed as an integer."));
    }

    public override Task<ResolutionDetails<string>> ResolveStringValueAsync(
        string flagKey,
        string defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flagKey);

        var section = this.GetFlagSection(flagKey);
        if (section.Value is null)
        {
            return Task.FromResult(new ResolutionDetails<string>(
                flagKey,
                defaultValue,
                ErrorType.FlagNotFound,
                Reason.Default,
                errorMessage: $"Flag '{flagKey}' was not found in configuration."));
        }

        return Task.FromResult(new ResolutionDetails<string>(
            flagKey,
            section.Value,
            ErrorType.None,
            Reason.Static));
    }

    public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(
        string flagKey,
        Value defaultValue,
        EvaluationContext? context = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flagKey);

        var section = this.GetFlagSection(flagKey);
        var children = section.GetChildren().ToList();
        if (children.Count > 0)
        {
            var structure = ConvertSectionToValue(section);
            return Task.FromResult(new ResolutionDetails<Value>(
                flagKey,
                structure,
                ErrorType.None,
                Reason.Static));
        }

        if (section.Value is not null)
        {
            return Task.FromResult(new ResolutionDetails<Value>(
                flagKey,
                ConvertScalarToValue(section.Value),
                ErrorType.None,
                Reason.Static));
        }

        return Task.FromResult(new ResolutionDetails<Value>(
            flagKey,
            defaultValue,
            ErrorType.FlagNotFound,
            Reason.Default,
            errorMessage: $"Flag '{flagKey}' was not found in configuration."));
    }

    private static Value ConvertScalarToValue(string rawValue)
    {
        if (bool.TryParse(rawValue, out var boolValue))
        {
            return new Value(boolValue);
        }

        if (int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
        {
            return new Value(intValue);
        }

        if (double.TryParse(rawValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture,
                out var doubleValue))
        {
            return new Value(doubleValue);
        }

        return new Value(rawValue);
    }

    private static Value ConvertSectionToValue(IConfigurationSection section)
    {
        var children = section.GetChildren().ToList();
        if (children.Count == 0)
        {
            return section.Value is null ? new Value() : ConvertScalarToValue(section.Value);
        }

        var isIndexedList = true;
        for (var i = 0; i < children.Count; i++)
        {
            if (!int.TryParse(children[i].Key, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedIdx) ||
                parsedIdx != i)
            {
                isIndexedList = false;
                break;
            }
        }

        if (isIndexedList)
        {
            var list = children.ConvertAll(ConvertSectionToValue);
            return new Value(list);
        }

        var builder = Structure.Builder();
        foreach (var child in children)
        {
            _ = builder.Set(child.Key, ConvertSectionToValue(child));
        }

        return new Value(builder.Build());
    }

    private IConfigurationSection GetFlagSection(string flagKey)
    {
        var sectionName = this.options.Value.SectionName;
        return string.IsNullOrWhiteSpace(sectionName)
            ? this.configuration.GetSection(flagKey)
            : this.configuration.GetSection(sectionName).GetSection(flagKey);
    }
}
