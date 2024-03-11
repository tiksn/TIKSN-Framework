using System.Globalization;
using System.Reflection;
using System.Security;
using System.Text;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TIKSN.Localization;

namespace TIKSN.Shell;

public partial class ShellCommandEngine : IShellCommandEngine
{
    private readonly List<Tuple<Type, ShellCommandAttribute, ConstructorInfo,
        Seq<Tuple<ShellCommandParameterAttribute, PropertyInfo>>>> commands;

    private readonly IConsoleService consoleService;
    private readonly ILogger<ShellCommandEngine> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IStringLocalizer stringLocalizer;

    public ShellCommandEngine(
        IServiceProvider serviceProvider,
        ILogger<ShellCommandEngine> logger,
        IStringLocalizer stringLocalizer,
        IConsoleService consoleService)
    {
        this.commands = [];
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        this.consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public void AddAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var definedType in assembly.DefinedTypes)
        {
            _ = this.TryAddType(definedType.AsType());
        }
    }

    public void AddType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (this.commands.Exists(item => item.Item1 == type))
        {
            return;
        }

        if (!type.GetInterfaces().Contains(typeof(IShellCommand)))
        {
            throw new ArgumentException(
                this.stringLocalizer.GetRequiredString(LocalizationKeys.Key588506767, type.FullName,
                    typeof(IShellCommand).FullName), nameof(type));
        }

        var commandAttribute = type.GetTypeInfo().GetCustomAttribute<ShellCommandAttribute>() ?? throw new ArgumentException(
                this.stringLocalizer.GetRequiredString(LocalizationKeys.Key491461331, type.FullName,
                    typeof(ShellCommandAttribute).FullName), nameof(type));

        LogCheckingCommandNameLocalization(this.logger, type.FullName);
        _ = commandAttribute.GetName(this.stringLocalizer);

        var constructors = type.GetConstructors();
        if (constructors.Length != 1)
        {
            throw new ArgumentException(
                this.stringLocalizer.GetRequiredString(LocalizationKeys.Key225262334, type.FullName),
                nameof(type));
        }

        var properties = new List<Tuple<ShellCommandParameterAttribute, PropertyInfo>>();
        foreach (var propertyInfo in type.GetProperties())
        {
            var commandParameterAttribute = propertyInfo.GetCustomAttribute<ShellCommandParameterAttribute>();
            if (commandParameterAttribute != null)
            {
                LogCheckingParameterNameLocalization(this.logger, type.FullName, propertyInfo.Name);
                _ = commandParameterAttribute.GetName(this.stringLocalizer);

                properties.Add(
                    new Tuple<ShellCommandParameterAttribute, PropertyInfo>(commandParameterAttribute,
                        propertyInfo));
            }
        }

        this.commands.Add(
            new Tuple<Type, ShellCommandAttribute, ConstructorInfo,
                Seq<Tuple<ShellCommandParameterAttribute, PropertyInfo>>>(
                type, commandAttribute, constructors.Single(), properties.OrderBy(x => x.Item1.Position).ToSeq()));
    }

#pragma warning disable MA0051 // Method is too long
    public async Task RunAsync()
#pragma warning restore MA0051 // Method is too long
    {
        while (true)
        {
            var command = this.consoleService.ReadLine(
                this.stringLocalizer.GetRequiredString(LocalizationKeys.Key671767216), ConsoleColor.Green);

            if (string.IsNullOrWhiteSpace(command))
            {
                continue;
            }

            command = NormalizeCommandName(command);

            if (string.Equals(command, this.stringLocalizer.GetRequiredString(LocalizationKeys.Key785393579),
                StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (string.Equals(command, this.stringLocalizer.GetRequiredString(LocalizationKeys.Key427524976),
                StringComparison.OrdinalIgnoreCase))
            {
                var helpItems = new List<ShellCommandHelpItem>
                {
                    new(
                    NormalizeCommandName(
                        this.stringLocalizer.GetRequiredString(LocalizationKeys.Key785393579)),
                    []),
                    new(
                    NormalizeCommandName(
                        this.stringLocalizer.GetRequiredString(LocalizationKeys.Key427524976)),
                    []),
                };

                foreach (var commandItem in this.commands)
                {
                    helpItems.Add(new ShellCommandHelpItem(
                        NormalizeCommandName(commandItem.Item2.GetName(this.stringLocalizer)),
                        commandItem.Item4.Select(item => item.Item1.GetName(this.stringLocalizer))));
                }

                helpItems = [.. helpItems.OrderBy(i => i.CommandName, StringComparer.Ordinal)];

                this.consoleService.WriteObjects(helpItems);
            }
            else
            {
                var matches = this.commands
                    .Where(item => string.Equals(command,
                        NormalizeCommandName(item.Item2.GetName(this.stringLocalizer)),
                        StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                switch (matches.Length)
                {
                    case 0:
                        this.consoleService.WriteError(
                            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key879318823));
                        break;

                    case 1:
                        await this.RunCommandAsync(command, matches.Single()).ConfigureAwait(false);
                        break;

                    default:
                        this.consoleService.WriteError(
                            this.stringLocalizer.GetRequiredString(LocalizationKeys.Key650068875));
                        break;
                }
            }
        }
    }

    private static void AppendException(StringBuilder messageBuilder, Exception exception)
    {
        AppendExceptionMessage(messageBuilder, exception);

        if (exception.InnerException != null)
        {
            AppendException(messageBuilder, exception.InnerException);
        }
    }

    private static void AppendExceptionMessage(StringBuilder messageBuilder, Exception exception)
    {
        _ = messageBuilder.Append(exception.Message);
        if (exception.Message.EndsWith('.'))
        {
            _ = messageBuilder.Append(' ');
        }
        else
        {
            _ = messageBuilder.Append(". ");
        }
    }

    [LoggerMessage(
        EventId = 5832719,
        Level = LogLevel.Debug,
        Message = "Checking command name localization for '{TypeFullName}' command")]
    private static partial void LogCheckingCommandNameLocalization(ILogger logger, string typeFullName);

    [LoggerMessage(
        EventId = 5833551,
        Level = LogLevel.Debug,
        Message = "Checking Parameter Name localization for '{TypeFullName}' command's '{PropertyName}' parameter")]
    private static partial void LogCheckingParameterNameLocalization(ILogger logger, string typeFullName, string propertyName);

    [LoggerMessage(
        EventId = 5832597,
        Level = LogLevel.Debug,
        Message = "Failed to add type '{TypeFullName}' as command")]
    private static partial void LogFailedToAddType(ILogger logger, Exception ex, string typeFullName);

    [LoggerMessage(
        EventId = 5834863,
        Level = LogLevel.Error,
        Message = "Log Generic Exception '{ErrorMessage}'")]
    private static partial void LogGenericException(ILogger logger, Exception ex, string errorMessage);

    [LoggerMessage(
        EventId = 5833260,
        Level = LogLevel.Trace,
        Message = "Parameter '{ParameterLocalizedName}' has value '{ParameterValue}'")]
    private static partial void LogParameterNameAndValue(ILogger logger, string parameterLocalizedName, object parameterValue);

    private static string NormalizeCommandName(string command)
    {
        if (command != null)
        {
            var additionalSeparators = new[] { "-", "_" };

            var normalizedParts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(whitespaceSeparatedPart =>
                    whitespaceSeparatedPart.Split(additionalSeparators, StringSplitOptions.RemoveEmptyEntries));

            return string.Join(' ', normalizedParts);
        }

        return command;
    }

    private void PrintError(Exception exception)
    {
        var messageBuilder = new StringBuilder();
        AppendExceptionMessage(messageBuilder, exception);

        AppendException(messageBuilder, exception);

        var builtMessage = messageBuilder.ToString();

        this.consoleService.WriteError(builtMessage);
        LogGenericException(this.logger, exception, builtMessage);
    }

    private object ReadCommandParameter(Tuple<ShellCommandParameterAttribute, PropertyInfo> property)
    {
        if (property.Item2.PropertyType == typeof(SecureString))
        {
            var secureStringParameter =
                this.consoleService.ReadPasswordLine(property.Item1.GetName(this.stringLocalizer),
                    ConsoleColor.Green);
            if (property.Item1.Mandatory && secureStringParameter.Length == 0)
            {
                return this.ReadCommandParameter(property);
            }

            return secureStringParameter;
        }

        var stringParameter =
            this.consoleService.ReadLine(property.Item1.GetName(this.stringLocalizer), ConsoleColor.Green);

        if (string.IsNullOrEmpty(stringParameter))
        {
            if (property.Item1.Mandatory)
            {
                return this.ReadCommandParameter(property);
            }

            return null;
        }

        var typeToConvert = property.Item2.PropertyType;

        if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            typeToConvert = Nullable.GetUnderlyingType(typeToConvert);
        }

        return Convert.ChangeType(stringParameter, typeToConvert, CultureInfo.CurrentCulture);
    }

    private async Task RunCommandAsync(string commandName,
        Tuple<Type, ShellCommandAttribute, ConstructorInfo,
            Seq<Tuple<ShellCommandParameterAttribute, PropertyInfo>>> commandInfo)
    {
        var commandScope = this.serviceProvider.CreateAsyncScope();
        await using (commandScope.ConfigureAwait(false))
        {
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                var commandContextStore =
                    commandScope.ServiceProvider.GetRequiredService<IShellCommandContext>() as
                        IShellCommandContextStore;

                commandContextStore.SetCommandName(commandName);

                var args = new List<object>();

                foreach (var parameterInfo in commandInfo.Item3.GetParameters())
                {
                    args.Add(commandScope.ServiceProvider.GetRequiredService(parameterInfo.ParameterType));
                }

                var obj = Activator.CreateInstance(commandInfo.Item1, args);

                foreach (var property in commandInfo.Item4)
                {
                    var parameter = this.ReadCommandParameter(property);

                    if (parameter != null)
                    {
                        property.Item2.SetValue(obj, parameter);
                    }

                    LogParameterNameAndValue(this.logger, property.Item1.GetName(this.stringLocalizer), property.Item2.GetValue(obj));
                }

                var command = obj as IShellCommand;

                try
                {
                    using (var cancellationTokenSource = new CancellationTokenSource())
                    using (this.consoleService.RegisterCancellation(cancellationTokenSource))
                    {
                        await command.ExecuteAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                    }
                }
                catch (ShellCommandSuspendedException ex)
                {
                    this.PrintError(ex);
                }
                catch (Exception ex)
                {
                    this.PrintError(ex);
                }
            }
            catch (Exception ex)
            {
                this.PrintError(ex);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }

    private bool TryAddType(Type type)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            this.AddType(type);

            return true;
        }
        catch (Exception ex)
        {
            LogFailedToAddType(this.logger, ex, type.FullName);
            return false;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}
