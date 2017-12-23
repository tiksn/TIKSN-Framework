using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TIKSN.Localization;

namespace TIKSN.Shell
{
    public class ShellCommandEngine : IShellCommandEngine
    {
        private readonly IConsoleService _consoleService;
        private readonly ILogger<ShellCommandEngine> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly List<Tuple<Type, ShellCommandAttribute, ConstructorInfo, IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>>> commands;

        public ShellCommandEngine(IServiceProvider serviceProvider, ILogger<ShellCommandEngine> logger, IStringLocalizer stringLocalizer, IConsoleService consoleService)
        {
            commands = new List<Tuple<Type, ShellCommandAttribute, ConstructorInfo, IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>>>();

            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _consoleService = consoleService;
            _serviceProvider = serviceProvider;
        }

        public void AddAssembly(Assembly assembly)
        {
            foreach (var definedType in assembly.DefinedTypes)
            {
                TryAddType(definedType.AsType());
            }
        }

        public void AddType(Type type)
        {
            if (commands.Any(item => item.Item1 == type))
                return;

            if (!type.GetInterfaces().Contains(typeof(IShellCommand)))
                throw new ArgumentException(_stringLocalizer.GetRequiredString(LocalizationKeys.Key588506767, type.FullName, typeof(IShellCommand).FullName), nameof(type));

            var commandAttribute = type.GetTypeInfo().GetCustomAttribute<ShellCommandAttribute>();
            if (commandAttribute == null)
                throw new ArgumentException(_stringLocalizer.GetRequiredString(LocalizationKeys.Key491461331, type.FullName, typeof(ShellCommandAttribute).FullName), nameof(type));

            _logger.LogDebug(804856258, $"Checking command name localization for '{type.FullName}' command.");
            commandAttribute.GetName(_stringLocalizer);

            var constructors = type.GetConstructors();
            if (constructors.Length != 1)
                throw new ArgumentException(_stringLocalizer.GetRequiredString(LocalizationKeys.Key225262334, type.FullName), nameof(type));

            var properties = new List<Tuple<ShellCommandParameterAttribute, PropertyInfo>>();
            foreach (var propertyInfo in type.GetProperties())
            {
                var commandParameterAttribute = propertyInfo.GetCustomAttribute<ShellCommandParameterAttribute>();
                if (commandParameterAttribute != null)
                {
                    _logger.LogDebug(804856258, $"Checking string localization for '{type.FullName}' command's '{propertyInfo.Name}' parameter.");
                    commandParameterAttribute.GetName(_stringLocalizer);

                    properties.Add(new Tuple<ShellCommandParameterAttribute, PropertyInfo>(commandParameterAttribute, propertyInfo));
                }
            }

            commands.Add(new Tuple<Type, ShellCommandAttribute, ConstructorInfo, IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>>(
                type, commandAttribute, constructors.Single(), properties));
        }

        public async Task RunAsync()
        {
            while (true)
            {
                var command = _consoleService.ReadLine(_stringLocalizer.GetRequiredString(LocalizationKeys.Key671767216), ConsoleColor.Green);

                if (string.IsNullOrWhiteSpace(command))
                    continue;

                command = NormalizeCommandName(command);

                if (string.Equals(command, _stringLocalizer.GetRequiredString(LocalizationKeys.Key785393579), StringComparison.OrdinalIgnoreCase))
                    break;

                if (string.Equals(command, _stringLocalizer.GetRequiredString(LocalizationKeys.Key427524976), StringComparison.OrdinalIgnoreCase))
                {
                    var helpItems = new List<ShellCommandHelpItem>();

                    helpItems.Add(new ShellCommandHelpItem(NormalizeCommandName(_stringLocalizer.GetRequiredString(LocalizationKeys.Key785393579)), Enumerable.Empty<string>()));
                    helpItems.Add(new ShellCommandHelpItem(NormalizeCommandName(_stringLocalizer.GetRequiredString(LocalizationKeys.Key427524976)), Enumerable.Empty<string>()));

                    foreach (var commandItem in commands)
                    {
                        helpItems.Add(new ShellCommandHelpItem(
                            NormalizeCommandName(commandItem.Item2.GetName(_stringLocalizer)),
                            commandItem.Item4.Select(item => item.Item1.GetName(_stringLocalizer))));
                    }

                    helpItems = helpItems.OrderBy(i => i.CommandName).ToList();

                    _consoleService.WriteObjects(helpItems);
                }
                else
                {
                    var matches = commands.Where(item => string.Equals(command, NormalizeCommandName(item.Item2.GetName(_stringLocalizer)), StringComparison.OrdinalIgnoreCase));

                    switch (matches.Count())
                    {
                        case 0:
                            _consoleService.WriteError(_stringLocalizer.GetRequiredString(LocalizationKeys.Key879318823));
                            break;

                        case 1:
                            await RunCommandAsync(command, matches.Single());
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private void AppendException(StringBuilder messageBuilder, Exception exception)
        {
            messageBuilder.Append(exception.Message);
            messageBuilder.Append(". ");

            if (exception.InnerException != null)
                AppendException(messageBuilder, exception.InnerException);
        }

        private string NormalizeCommandName(string command)
        {
            if (command != null)
            {
                var additionalSeparators = new string[] { "-", "_" };

                var normalizedParts = command.Split(null)
                    .SelectMany(whitespaceSeparatedPart => whitespaceSeparatedPart.Split(additionalSeparators, StringSplitOptions.RemoveEmptyEntries));

                return string.Join(" ", normalizedParts);
            }

            return command;
        }

        private void PrintError(EventId eventId, Exception exception, string message)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.Append(message);
            messageBuilder.Append(". ");

            AppendException(messageBuilder, exception);

            var builtMessage = messageBuilder.ToString();

            _consoleService.WriteError(builtMessage);
            _logger.LogError(eventId, exception, builtMessage);
        }

        private object ReadCommandParameter(Tuple<ShellCommandParameterAttribute, PropertyInfo> property)
        {
            if (property.Item2.PropertyType == typeof(SecureString))
            {
                var secureStringParameter = _consoleService.ReadPasswordLine(property.Item1.GetName(_stringLocalizer), ConsoleColor.Green);
                if (property.Item1.Mandatory && secureStringParameter.Length == 0)
                    return ReadCommandParameter(property);

                return secureStringParameter;
            }

            var stringParameter = _consoleService.ReadLine(property.Item1.GetName(_stringLocalizer), ConsoleColor.Green);

            if (string.IsNullOrEmpty(stringParameter))
            {
                if (property.Item1.Mandatory)
                    return ReadCommandParameter(property);
                return null;
            }

            var typeToConvert = property.Item2.PropertyType;

            if (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeToConvert = Nullable.GetUnderlyingType(typeToConvert);
            }

            return Convert.ChangeType(stringParameter, typeToConvert);
        }

        private async Task RunCommandAsync(string commandName, Tuple<Type, ShellCommandAttribute, ConstructorInfo, IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>> commandInfo)
        {
            using (var commandScope = _serviceProvider.CreateScope())
            {
                try
                {
                    var commandContextStore = commandScope.ServiceProvider.GetRequiredService<IShellCommandContext>() as IShellCommandContextStore;

                    commandContextStore.SetCommandName(commandName);

                    var args = new List<object>();

                    foreach (var parameterInfo in commandInfo.Item3.GetParameters())
                    {
                        args.Add(commandScope.ServiceProvider.GetRequiredService(parameterInfo.ParameterType));
                    }

                    var obj = Activator.CreateInstance(commandInfo.Item1, args.ToArray());

                    foreach (var property in commandInfo.Item4)
                    {
                        var parameter = ReadCommandParameter(property);

                        if (parameter != null)
                            property.Item2.SetValue(obj, parameter);

                        _logger.LogTrace($"Parameter '{property.Item1.GetName(_stringLocalizer)}' has value '{property.Item2.GetValue(obj)}'");
                    }

                    var command = obj as IShellCommand;

                    try
                    {
                        await command.ExecuteAsync();
                    }
#pragma warning disable CC0004 // Catch block cannot be empty
                    catch (ShellCommandSuspendedException) { }
#pragma warning restore CC0004 // Catch block cannot be empty
                    catch (Exception ex)
                    {
                        PrintError(1815744366, ex, _stringLocalizer.GetRequiredString(LocalizationKeys.Key163077375));
                    }
                }
                catch (Exception ex)
                {
                    PrintError(1999436483, ex, _stringLocalizer.GetRequiredString(LocalizationKeys.Key548430597));
                }
            }
        }

        private bool TryAddType(Type type)
        {
            try
            {
                AddType(type);

                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(1955486110, ex, "Failed to add type {0} as command.", type.FullName);
                //_logger.LogDebug(650126203, ex, string.Empty);
                return false;
            }
        }
    }
}