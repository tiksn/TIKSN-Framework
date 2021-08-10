using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TIKSN.Localization;

namespace TIKSN.Shell
{
    public class ShellCommandEngine : IShellCommandEngine
    {
        private readonly IConsoleService _consoleService;
        private readonly ILogger<ShellCommandEngine> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStringLocalizer _stringLocalizer;

        private readonly List<Tuple<Type, ShellCommandAttribute, ConstructorInfo,
            IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>>> commands;

        public ShellCommandEngine(IServiceProvider serviceProvider, ILogger<ShellCommandEngine> logger,
            IStringLocalizer stringLocalizer, IConsoleService consoleService)
        {
            this.commands =
                new List<Tuple<Type, ShellCommandAttribute, ConstructorInfo,
                    IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>>>();

            this._logger = logger;
            this._stringLocalizer = stringLocalizer;
            this._consoleService = consoleService;
            this._serviceProvider = serviceProvider;
        }

        public void AddAssembly(Assembly assembly)
        {
            foreach (var definedType in assembly.DefinedTypes)
            {
                _ = this.TryAddType(definedType.AsType());
            }
        }

        public void AddType(Type type)
        {
            if (this.commands.Any(item => item.Item1 == type))
            {
                return;
            }

            if (!type.GetInterfaces().Contains(typeof(IShellCommand)))
            {
                throw new ArgumentException(
                    this._stringLocalizer.GetRequiredString(LocalizationKeys.Key588506767, type.FullName,
                        typeof(IShellCommand).FullName), nameof(type));
            }

            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<ShellCommandAttribute>();
            if (commandAttribute == null)
            {
                throw new ArgumentException(
                    this._stringLocalizer.GetRequiredString(LocalizationKeys.Key491461331, type.FullName,
                        typeof(ShellCommandAttribute).FullName), nameof(type));
            }

            this._logger.LogDebug(804856258, $"Checking command name localization for '{type.FullName}' command.");
            _ = commandAttribute.GetName(this._stringLocalizer);

            var constructors = type.GetConstructors();
            if (constructors.Length != 1)
            {
                throw new ArgumentException(
                    this._stringLocalizer.GetRequiredString(LocalizationKeys.Key225262334, type.FullName),
                    nameof(type));
            }

            var properties = new List<Tuple<ShellCommandParameterAttribute, PropertyInfo>>();
            foreach (var propertyInfo in type.GetProperties())
            {
                var commandParameterAttribute = propertyInfo.GetCustomAttribute<ShellCommandParameterAttribute>();
                if (commandParameterAttribute != null)
                {
                    this._logger.LogDebug(804856258,
                        $"Checking string localization for '{type.FullName}' command's '{propertyInfo.Name}' parameter.");
                    _ = commandParameterAttribute.GetName(this._stringLocalizer);

                    properties.Add(
                        new Tuple<ShellCommandParameterAttribute, PropertyInfo>(commandParameterAttribute,
                            propertyInfo));
                }
            }

            this.commands.Add(
                new Tuple<Type, ShellCommandAttribute, ConstructorInfo,
                    IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>>(
                    type, commandAttribute, constructors.Single(), properties));
        }

        public async Task RunAsync()
        {
            while (true)
            {
                var command = this._consoleService.ReadLine(
                    this._stringLocalizer.GetRequiredString(LocalizationKeys.Key671767216), ConsoleColor.Green);

                if (string.IsNullOrWhiteSpace(command))
                {
                    continue;
                }

                command = NormalizeCommandName(command);

                if (string.Equals(command, this._stringLocalizer.GetRequiredString(LocalizationKeys.Key785393579),
                    StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (string.Equals(command, this._stringLocalizer.GetRequiredString(LocalizationKeys.Key427524976),
                    StringComparison.OrdinalIgnoreCase))
                {
                    var helpItems = new List<ShellCommandHelpItem>
                    {
                        new ShellCommandHelpItem(
                        NormalizeCommandName(
                            this._stringLocalizer.GetRequiredString(LocalizationKeys.Key785393579)),
                        Enumerable.Empty<string>()),
                        new ShellCommandHelpItem(
                        NormalizeCommandName(
                            this._stringLocalizer.GetRequiredString(LocalizationKeys.Key427524976)),
                        Enumerable.Empty<string>())
                    };

                    foreach (var commandItem in this.commands)
                    {
                        helpItems.Add(new ShellCommandHelpItem(
                            NormalizeCommandName(commandItem.Item2.GetName(this._stringLocalizer)),
                            commandItem.Item4.Select(item => item.Item1.GetName(this._stringLocalizer))));
                    }

                    helpItems = helpItems.OrderBy(i => i.CommandName).ToList();

                    this._consoleService.WriteObjects(helpItems);
                }
                else
                {
                    var matches = this.commands.Where(item => string.Equals(command,
                        NormalizeCommandName(item.Item2.GetName(this._stringLocalizer)),
                        StringComparison.OrdinalIgnoreCase));

                    switch (matches.Count())
                    {
                        case 0:
                            this._consoleService.WriteError(
                                this._stringLocalizer.GetRequiredString(LocalizationKeys.Key879318823));
                            break;

                        case 1:
                            await this.RunCommandAsync(command, matches.Single()).ConfigureAwait(false);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void AppendExceptionMessage(StringBuilder messageBuilder, Exception exception)
        {
            _ = messageBuilder.Append(exception.Message);
            if (exception.Message.EndsWith(".", StringComparison.OrdinalIgnoreCase))
            {
                _ = messageBuilder.Append(' ');
            }
            else
            {
                _ = messageBuilder.Append(". ");
            }
        }

        private void AppendException(StringBuilder messageBuilder, Exception exception)
        {
            AppendExceptionMessage(messageBuilder, exception);

            if (exception.InnerException != null)
            {
                this.AppendException(messageBuilder, exception.InnerException);
            }
        }

        private static string NormalizeCommandName(string command)
        {
            if (command != null)
            {
                var additionalSeparators = new[] { "-", "_" };

                var normalizedParts = command.Split(null)
                    .SelectMany(whitespaceSeparatedPart =>
                        whitespaceSeparatedPart.Split(additionalSeparators, StringSplitOptions.RemoveEmptyEntries));

                return string.Join(" ", normalizedParts);
            }

            return command;
        }

        private void PrintError(EventId eventId, Exception exception)
        {
            var messageBuilder = new StringBuilder();
            AppendExceptionMessage(messageBuilder, exception);

            this.AppendException(messageBuilder, exception);

            var builtMessage = messageBuilder.ToString();

            this._consoleService.WriteError(builtMessage);
            this._logger.LogError(eventId, exception, builtMessage);
        }

        private object ReadCommandParameter(Tuple<ShellCommandParameterAttribute, PropertyInfo> property)
        {
            if (property.Item2.PropertyType == typeof(SecureString))
            {
                var secureStringParameter =
                    this._consoleService.ReadPasswordLine(property.Item1.GetName(this._stringLocalizer),
                        ConsoleColor.Green);
                if (property.Item1.Mandatory && secureStringParameter.Length == 0)
                {
                    return this.ReadCommandParameter(property);
                }

                return secureStringParameter;
            }

            var stringParameter =
                this._consoleService.ReadLine(property.Item1.GetName(this._stringLocalizer), ConsoleColor.Green);

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

            return Convert.ChangeType(stringParameter, typeToConvert);
        }

        private async Task RunCommandAsync(string commandName,
            Tuple<Type, ShellCommandAttribute, ConstructorInfo,
                IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>> commandInfo)
        {
            using var commandScope = this._serviceProvider.CreateScope();
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

                var obj = Activator.CreateInstance(commandInfo.Item1, args.ToArray());

                foreach (var property in commandInfo.Item4)
                {
                    var parameter = this.ReadCommandParameter(property);

                    if (parameter != null)
                    {
                        property.Item2.SetValue(obj, parameter);
                    }

                    this._logger.LogTrace(
                        $"Parameter '{property.Item1.GetName(this._stringLocalizer)}' has value '{property.Item2.GetValue(obj)}'");
                }

                var command = obj as IShellCommand;

                try
                {
                    using (var cancellationTokenSource = new CancellationTokenSource())
                    using (this._consoleService.RegisterCancellation(cancellationTokenSource))
                    {
                        await command.ExecuteAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                    }
                }
#pragma warning disable CC0004 // Catch block cannot be empty
                catch (ShellCommandSuspendedException) { }
#pragma warning restore CC0004 // Catch block cannot be empty
                catch (Exception ex)
                {
                    this.PrintError(1815744366, ex);
                }
            }
            catch (Exception ex)
            {
                this.PrintError(1999436483, ex);
            }
        }

        private bool TryAddType(Type type)
        {
            try
            {
                this.AddType(type);

                return true;
            }
            catch (Exception)
            {
                //_logger.LogError(1955486110, ex, "Failed to add type {0} as command.", type.FullName);
                //_logger.LogDebug(650126203, ex, string.Empty);
                return false;
            }
        }
    }
}
