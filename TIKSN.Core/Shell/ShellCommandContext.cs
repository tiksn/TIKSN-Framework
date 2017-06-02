using Microsoft.Extensions.Localization;
using System;
using TIKSN.Localization;

namespace TIKSN.Shell
{
	public class ShellCommandContext : IShellCommandContext, IShellCommandContextStore
	{
		private static readonly Guid NoKey = new Guid("67d58692-e804-4626-a61c-5ec3beb4397e");
		private static readonly Guid NoToAllKey = new Guid("bea642f9-d15e-4e4a-86bd-5576ce526e3b");
		private static readonly Guid ShouldProcessCaptionKey = new Guid("d0885792-d60b-4f76-82a8-c4ceacc1c606");
		private static readonly Guid ShouldProcessQueryKey = new Guid("1840e905-af75-41db-ac90-fb71e309f03f");
		private static readonly Guid SuspendKey = new Guid("a4de1bc6-68af-4363-9ce4-91a13dd2c786");
		private static readonly Guid YesKey = new Guid("0c7f1703-7e67-4d3b-ae6d-4eb9ec3172c4");
		private static readonly Guid YesToAllKey = new Guid("daaa9e93-e303-4cae-a05c-874481769f34");

		private readonly IConsoleService _consoleService;
		private readonly IStringLocalizer _stringLocalizer;

		private string commandName;
		private bool noToAll;
		private bool yesToAll;

		public ShellCommandContext(IStringLocalizer stringLocalizer, IConsoleService consoleService)
		{
			_stringLocalizer = stringLocalizer;
			_consoleService = consoleService;
		}

		public void SetCommandName(string commandName)
		{
			this.commandName = commandName;
		}

		public bool ShouldContinue(string query, string caption)
		{
			var message = $"{caption}{Environment.NewLine}{query}";

			var answer = _consoleService.UserPrompt(message, _stringLocalizer.GetRequiredString(YesKey), _stringLocalizer.GetRequiredString(NoKey), _stringLocalizer.GetRequiredString(SuspendKey));

			switch (answer)
			{
				case 0:
					return true;

				case 1:
					return false;

				case 2:
					throw new ShellCommandSuspendException();

				default:
					throw new NotSupportedException();
			}
		}

		public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
		{
			if (yesToAll)
				return true;

			if (noToAll)
				return false;
			var message = $"{caption}{Environment.NewLine}{query}";

			var answer = _consoleService.UserPrompt(message,
				_stringLocalizer.GetRequiredString(YesKey),
				_stringLocalizer.GetRequiredString(YesToAllKey),
				_stringLocalizer.GetRequiredString(NoKey),
				_stringLocalizer.GetRequiredString(NoToAllKey),
				_stringLocalizer.GetRequiredString(SuspendKey));

			switch (answer)
			{
				case 0:
					return true;

				case 1:
					yesToAll = true;
					return true;

				case 2:
					return false;

				case 3:
					noToAll = true;
					return false;

				case 4:
					throw new ShellCommandSuspendException();

				default:
					throw new NotSupportedException();
			}
		}

		public bool ShouldProcess(string target, string action)
		{
			return ShouldContinue(_stringLocalizer.GetRequiredString(ShouldProcessCaptionKey),
				string.Format(_stringLocalizer.GetRequiredString(ShouldProcessQueryKey), action, target),
				ref yesToAll, ref noToAll);
		}

		public bool ShouldProcess(string target)
		{
			return ShouldProcess(target, commandName);
		}

		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
		{
			return ShouldContinue(verboseWarning, caption, ref yesToAll, ref noToAll);
		}
	}
}
