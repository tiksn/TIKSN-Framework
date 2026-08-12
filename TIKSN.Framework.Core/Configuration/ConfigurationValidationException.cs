using System.Globalization;
using System.Text;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static LanguageExt.Prelude;

namespace TIKSN.Configuration;

public record ConfigurationError(
    Option<Type> OptionsType,
    Option<string> ConfigurationPath,
    Option<string> ConfigurationKey,
    Either<Unit, Option<object>> ConfigurationValue);

public class ConfigurationValidationException : Exception
{
    public ConfigurationValidationException()
    {
    }

    public ConfigurationValidationException(string message) : base(message)
    {
    }

    public ConfigurationValidationException(string message, Exception inner) : base(message, inner)
    {
    }

    public Seq<ConfigurationError> Errors { get; private set; } = LanguageExt.Seq.empty<ConfigurationError>();

    public override string Message
    {
        get
        {
            if (this.Errors.IsEmpty)
            {
                return base.Message;
            }

            var builder = new StringBuilder(base.Message);

            foreach (var e in this.Errors)
            {
                _ = builder.Append(" [");
                var isFirst = true;

                _ = e.OptionsType.IfSome(t =>
                {
                    _ = builder.Append(CultureInfo.InvariantCulture, $"Options Type: '{t.FullName}'");
                    isFirst = false;
                });

                _ = e.ConfigurationPath.IfSome(p =>
                {
                    if (!isFirst)
                    {
                        _ = builder.Append(", ");
                    }

                    _ = builder.Append(CultureInfo.InvariantCulture, $"Path: '{p}'");
                    isFirst = false;
                });

                _ = e.ConfigurationKey.IfSome(k =>
                {
                    if (!isFirst)
                    {
                        _ = builder.Append(", ");
                    }

                    _ = builder.Append(CultureInfo.InvariantCulture, $"Key: '{k}'");
                    isFirst = false;
                });

                var valueString = e.ConfigurationValue.Match(
                    Right: opt => opt.Match(
                        Some: v => $"Value: '{v}'",
                        None: () => "Value is null"
                    ),
                    Left: _ => "Value is *** SENSITIVE ***"
                );

                if (!isFirst)
                {
                    _ = builder.Append(", ");
                }

                _ = builder.Append(valueString);

                _ = builder.Append(']');
            }

            return builder.ToString();
        }
    }

    public ConfigurationValidationException WithConfigurationSection(IConfigurationSection configurationSection)
    {
        ArgumentNullException.ThrowIfNull(configurationSection);
        var objectOption = Optional((object?)configurationSection.Value);
        this.Errors = this.Errors.Add(new ConfigurationError(None, configurationSection.Path, configurationSection.Key,
            Right(objectOption)));
        return this;
    }


    public ConfigurationValidationException WithConfigurationSection<TValue>(IConfigurationSection configurationSection,
        string configurationKey, TValue? configurationValue)
    {
        ArgumentNullException.ThrowIfNull(configurationSection);
        var path = $"{configurationSection.Path}:{configurationKey}";
        var objectOption = Optional(configurationValue).Map(v => (object)v!);
        this.Errors = this.Errors.Add(new ConfigurationError(None, path, configurationKey, Right(objectOption)));
        return this;
    }

    public ConfigurationValidationException WithConfigurationSectionSensitive(
        IConfigurationSection configurationSection)
    {
        ArgumentNullException.ThrowIfNull(configurationSection);
        this.Errors = this.Errors.Add(new ConfigurationError(None, configurationSection.Path, configurationSection.Key,
            Left(unit)));
        return this;
    }

    public ConfigurationValidationException WithConfigurationSectionSensitive(
        IConfigurationSection configurationSection, string configurationKey)
    {
        ArgumentNullException.ThrowIfNull(configurationSection);
        var path = $"{configurationSection.Path}:{configurationKey}";
        this.Errors = this.Errors.Add(new ConfigurationError(None, path, configurationKey, Left(unit)));
        return this;
    }


    public ConfigurationValidationException WithOptions<T, TValue>(IOptions<T> options, string configurationKey,
        TValue? configurationValue) where T : class
    {
        ArgumentNullException.ThrowIfNull(options);
        var objectOption = Optional(configurationValue).Map(v => (object)v!);
        this.Errors = this.Errors.Add(new ConfigurationError(typeof(T), None, configurationKey, Right(objectOption)));
        return this;
    }

    public ConfigurationValidationException WithOptionsSensitive<T>(IOptions<T> options, string configurationKey)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(options);
        this.Errors = this.Errors.Add(new ConfigurationError(typeof(T), None, configurationKey, Left(unit)));
        return this;
    }
}
