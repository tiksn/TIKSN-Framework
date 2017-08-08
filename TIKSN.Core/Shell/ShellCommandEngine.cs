using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using TIKSN.Localization;

namespace TIKSN.Shell
{
	public class ShellCommandEngine : IShellCommandEngine
	{
		private static readonly Guid CommandNameKey = new Guid(new byte[] { 0x82, 0x34, 0xf8, 0xb7, 0x7b, 0x59, 0x4b, 0x43, 0xbf, 0x18, 0x1a, 0xbd, 0xee, 0x64, 0x1e, 0x34 });
		private static readonly Guid CommandNotFoundKey = new Guid(new byte[] { 0xaf, 0x9b, 0x80, 0x0b, 0xc8, 0xfb, 0x94, 0x45, 0x92, 0xf4, 0x07, 0xcf, 0xc4, 0x27, 0x01, 0x0c });
		private static readonly Guid ExecutedWithExceptionKey = new Guid(new byte[] { 0x2e, 0x76, 0x49, 0x96, 0x41, 0xd4, 0xa0, 0x4f, 0xb7, 0xd5, 0xe9, 0xa3, 0xe4, 0x06, 0xaa, 0x04 });
		private static readonly Guid ExitKey = new Guid(new byte[] { 0xac, 0x10, 0x5e, 0x02, 0xd0, 0xe0, 0x2c, 0x44, 0xb8, 0x47, 0xe6, 0x72, 0xf2, 0x9b, 0x0a, 0xc5 });
		private static readonly Guid HelpKey = new Guid(new byte[] { 0x82, 0x9f, 0x32, 0x8c, 0x54, 0x73, 0x49, 0x41, 0x90, 0x57, 0x0c, 0xd4, 0x34, 0xb3, 0xdf, 0xf0 });
		private static readonly Guid MustImplementCommandmentAttributeKey = new Guid(new byte[] { 0xcf, 0x08, 0x4b, 0x58, 0x6c, 0x38, 0x8a, 0x46, 0xab, 0x33, 0xca, 0xc0, 0x03, 0x0e, 0x8c, 0x9c });
		private static readonly Guid NotCommandmentKey = new Guid(new byte[] { 0xe2, 0xbd, 0x22, 0xc5, 0x65, 0xfb, 0x0c, 0x42, 0x8c, 0x86, 0xd8, 0x5e, 0xcd, 0xcd, 0x79, 0x0a });
		private static readonly Guid NotOneConstructorKey = new Guid(new byte[] { 0xd5, 0x8e, 0xe9, 0x6f, 0xc7, 0x53, 0xf6, 0x48, 0xa2, 0xd4, 0x15, 0xc2, 0x2f, 0x41, 0xcb, 0xe8 });
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
				throw new ArgumentException(_stringLocalizer.GetRequiredString(NotCommandmentKey, type.FullName, typeof(IShellCommand).FullName), nameof(type));

			var commandAttribute = type.GetTypeInfo().GetCustomAttribute<ShellCommandAttribute>();
			if (commandAttribute == null)
				throw new ArgumentException(_stringLocalizer.GetRequiredString(MustImplementCommandmentAttributeKey, type.FullName, typeof(ShellCommandAttribute).FullName), nameof(type));

			_logger.LogDebug(804856258, $"Checking command name localization for '{type.FullName}' command.");
			commandAttribute.GetName(_stringLocalizer);

			var constructors = type.GetConstructors();
			if (constructors.Length != 1)
				throw new ArgumentException(_stringLocalizer.GetRequiredString(NotOneConstructorKey, type.FullName), nameof(type));

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
				var command = _consoleService.ReadLine(_stringLocalizer.GetRequiredString(CommandNameKey), ConsoleColor.Green);

				if (string.IsNullOrWhiteSpace(command))
					continue;

				command = NormalizeCommandName(command);

				if (string.Equals(command, _stringLocalizer.GetRequiredString(ExitKey), StringComparison.OrdinalIgnoreCase))
					break;

				if (string.Equals(command, _stringLocalizer.GetRequiredString(HelpKey), StringComparison.OrdinalIgnoreCase))
				{
					var helpItems = new List<ShellCommandHelpItem>();

					helpItems.Add(new ShellCommandHelpItem(NormalizeCommandName(_stringLocalizer.GetRequiredString(ExitKey)), Enumerable.Empty<string>()));
					helpItems.Add(new ShellCommandHelpItem(NormalizeCommandName(_stringLocalizer.GetRequiredString(HelpKey)), Enumerable.Empty<string>()));

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
							_consoleService.WriteError(_stringLocalizer.GetRequiredString(CommandNotFoundKey));
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

		private object ReadCommandParameter(Tuple<ShellCommandParameterAttribute, PropertyInfo> property)
		{
			if (property.Item2.PropertyType == typeof(SecureString))
			{
				var secureStringParameter = _consoleService.ReadPasswordLine(property.Item1.GetName(_stringLocalizer), ConsoleColor.Green);
				if (secureStringParameter.Length == 0)
				{
					if (property.Item1.Mandatory)
						return ReadCommandParameter(property);
					return null;
				}
			}

			var stringParameter = _consoleService.ReadLine(property.Item1.GetName(_stringLocalizer), ConsoleColor.Green);

			if (string.IsNullOrEmpty(stringParameter))
			{
				if (property.Item1.Mandatory)
					return ReadCommandParameter(property);
				return null;
			}

			return Convert.ChangeType(stringParameter, property.Item2.PropertyType);
		}

		private async Task RunCommandAsync(string commandName, Tuple<Type, ShellCommandAttribute, ConstructorInfo, IEnumerable<Tuple<ShellCommandParameterAttribute, PropertyInfo>>> commandInfo)
		{
			using (var commandScope = _serviceProvider.CreateScope())
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
				catch (ShellCommandSuspendedException) { }
				catch (Exception ex)
				{
					_consoleService.WriteError(_stringLocalizer.GetRequiredString(ExecutedWithExceptionKey));
					_logger.LogError(1815744366, ex, _stringLocalizer.GetRequiredString(ExecutedWithExceptionKey));
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