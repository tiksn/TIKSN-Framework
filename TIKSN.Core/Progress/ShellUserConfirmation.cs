using Microsoft.Extensions.Localization;
using System;
using TIKSN.Localization;
using TIKSN.Shell;

namespace TIKSN.Progress
{
	public class ShellUserConfirmation : IUserConfirmation
	{
		private static readonly Guid noKey = new Guid(new byte[] { 0x92, 0x86, 0xd5, 0x67, 0x04, 0xe8, 0x26, 0x46, 0xa6, 0x1c, 0x5e, 0xc3, 0xbe, 0xb4, 0x39, 0x7e });
		private static readonly Guid shouldProcessActionOnTargetKey = new Guid(new byte[] { 0x87, 0x72, 0x4b, 0xf1, 0x10, 0xe8, 0x6e, 0x4e, 0x8e, 0xe1, 0x7f, 0xf8, 0x9e, 0xd4, 0x96, 0xe8 });
		private static readonly Guid yesKey = new Guid(new byte[] { 0x03, 0x17, 0x7f, 0x0c, 0x67, 0x7e, 0x3b, 0x4d, 0xae, 0x6d, 0x4e, 0xb9, 0xec, 0x31, 0x72, 0xc4 });
		private readonly IConsoleService _consoleService;
		private readonly IStringLocalizer _stringLocalizer;

		public ShellUserConfirmation(IStringLocalizer stringLocalizer, IConsoleService consoleService)
		{
			_stringLocalizer = stringLocalizer;
			_consoleService = consoleService;
		}

		public bool ShouldProcess(string target)
		{
			throw new NotImplementedException(); //TODO: implement
		}

		public bool ShouldProcess(string target, string action)
		{
			var promptMessage = _stringLocalizer.GetRequiredString(shouldProcessActionOnTargetKey, action, target);

			var answer = _consoleService.UserPrompt(promptMessage, _stringLocalizer.GetRequiredString(yesKey), _stringLocalizer.GetRequiredString(noKey));

			switch (answer)
			{
				case 0:
					return true;

				case 1:
					return false;

				default:
					throw new NotSupportedException();
			}
		}

		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
		{
			throw new NotImplementedException(); //TODO: implement
		}
	}
}